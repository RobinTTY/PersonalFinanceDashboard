using Microsoft.EntityFrameworkCore;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Tests.TestUtilities;
using static RobinTTY.PersonalFinanceDashboard.Infrastructure.Tests.TestUtilities.GoCardlessJson;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Tests.DataSynchronization;

public class TransactionSyncHandlerTests : IDisposable
{
    private static readonly Guid AccountId = Guid.Parse("a1000000-0000-0000-0000-000000000001");
    private static readonly Guid OtherAccountId = Guid.Parse("a2000000-0000-0000-0000-000000000002");
    private static readonly Guid TransactionId = Guid.Parse("70000000-0000-0000-0000-000000000001");
    private static readonly Guid OtherTransactionId = Guid.Parse("70000000-0000-0000-0000-000000000002");

    private readonly SyncTestEnvironment _env = new();

    public void Dispose() => _env.Dispose();

    [Fact]
    public async Task NewTransactions_AreAddedToTheirAccount()
    {
        await SeedAccountsWithActiveRequest(AccountId);
        _env.GoCardless.Respond(TransactionsPath(AccountId),
            Transactions((TransactionId, -10m), (OtherTransactionId, 25m)));

        Assert.True(await _env.SyncTransactions());

        var transactions = await GetTransactions(AccountId);
        Assert.Equal([-10m, 25m], transactions.Select(t => t.Amount).Order());
    }

    [Fact]
    public async Task SyncedAgain_ExistingTransactionsAreUpdatedInsteadOfDuplicated()
    {
        await SeedAccountsWithActiveRequest(AccountId);
        _env.GoCardless.Respond(TransactionsPath(AccountId), Transactions((TransactionId, -10m)));
        await _env.SyncTransactions();

        _env.GoCardless.Respond(TransactionsPath(AccountId),
            Transactions((TransactionId, -12m), (OtherTransactionId, 5m)));
        await _env.SyncTransactions(force: true);

        var transactions = await GetTransactions(AccountId);
        Assert.Equal(2, transactions.Count);
        Assert.Equal(-12m, transactions.Single(t => t.ExternalId == TransactionId.ToString()).Amount);
    }

    [Fact]
    public async Task SyncedAgain_UserEnteredCategoryAndNotesAreKept()
    {
        var account = TestData.Account(AccountId);
        await _env.Seed(TestData.Request(Guid.NewGuid(), _env.UtcNow.AddDays(-1), account),
            TestData.Transaction(TransactionId, account, -10m, category: "Groceries", notes: "Weekly shopping"));
        _env.GoCardless.Respond(TransactionsPath(AccountId), Transactions((TransactionId, -11m)));

        await _env.SyncTransactions();

        var transaction = (await GetTransactions(AccountId)).Single();
        Assert.Equal(-11m, transaction.Amount);
        Assert.Equal("Groceries", transaction.Category);
        Assert.Equal("Weekly shopping", transaction.Notes);
    }

    [Fact]
    public async Task SameExternalIdInTwoAccounts_CreatesATransactionForEachAccount()
    {
        // External ids are only unique per account
        await SeedAccountsWithActiveRequest(AccountId, OtherAccountId);
        _env.GoCardless.Respond(TransactionsPath(AccountId), Transactions((TransactionId, -10m)));
        _env.GoCardless.Respond(TransactionsPath(OtherAccountId), Transactions((TransactionId, -20m)));

        await _env.SyncTransactions();

        Assert.Equal(-10m, (await GetTransactions(AccountId)).Single().Amount);
        Assert.Equal(-20m, (await GetTransactions(OtherAccountId)).Single().Amount);
    }

    [Fact]
    public async Task FreshTransactions_AreOnlyFetchedAgainAfterOneDay()
    {
        await SeedAccountsWithActiveRequest(AccountId);
        _env.GoCardless.Respond(TransactionsPath(AccountId), Transactions((TransactionId, -10m)));

        await _env.SyncTransactions();
        await _env.SyncTransactions();
        Assert.Equal(1, _env.GoCardless.RequestCount(TransactionsPath(AccountId)));

        _env.Time.Advance(TimeSpan.FromDays(1));
        await _env.SyncTransactions();
        Assert.Equal(2, _env.GoCardless.RequestCount(TransactionsPath(AccountId)));
    }

    [Fact]
    public async Task ConcurrentSyncs_RetrieveTheTransactionsOnlyOnce()
    {
        await SeedAccountsWithActiveRequest(AccountId);
        _env.GoCardless.Respond(TransactionsPath(AccountId), Transactions((TransactionId, -10m)));

        // The second sync starts while the first one is still waiting for GoCardless
        var releaseResponses = _env.GoCardless.HoldResponses();
        var first = _env.SyncTransactions();
        var second = _env.SyncTransactions();
        releaseResponses();

        Assert.All(await Task.WhenAll(first, second), Assert.True);
        Assert.Equal(1, _env.GoCardless.RequestCount(TransactionsPath(AccountId)));
        Assert.Single(await GetTransactions(AccountId));
    }

    [Fact]
    public async Task AccountsOfAuthRequestsOlderThan90Days_AreNotSynced()
    {
        await _env.Seed(TestData.Request(Guid.NewGuid(), _env.UtcNow.AddDays(-91), TestData.Account(AccountId)));

        Assert.True(await _env.SyncTransactions());

        Assert.Equal(0, _env.GoCardless.RequestCount(TransactionsPath(AccountId)));
    }

    [Fact]
    public async Task FailedRequestForOneAccount_DoesNotStopTheOthers()
    {
        // No response is configured for the other account, so its request fails
        await SeedAccountsWithActiveRequest(AccountId, OtherAccountId);
        _env.GoCardless.Respond(TransactionsPath(AccountId), Transactions((TransactionId, -10m)));

        Assert.True(await _env.SyncTransactions());

        Assert.Single(await GetTransactions(AccountId));
        Assert.Empty(await GetTransactions(OtherAccountId));
    }

    [Fact]
    public async Task FailedRequest_IsRetriedWithAnIncreasingDelay()
    {
        await SeedAccountsWithActiveRequest(AccountId);

        await _env.SyncTransactions();
        await _env.SyncTransactions();
        Assert.Equal(1, _env.GoCardless.RequestCount(TransactionsPath(AccountId)));

        // The first retry happens after an hour, the second one two hours later
        _env.Time.Advance(TimeSpan.FromMinutes(59));
        await _env.SyncTransactions();
        Assert.Equal(1, _env.GoCardless.RequestCount(TransactionsPath(AccountId)));

        _env.Time.Advance(TimeSpan.FromMinutes(1));
        await _env.SyncTransactions();
        Assert.Equal(2, _env.GoCardless.RequestCount(TransactionsPath(AccountId)));

        _env.Time.Advance(TimeSpan.FromHours(1));
        await _env.SyncTransactions();
        Assert.Equal(2, _env.GoCardless.RequestCount(TransactionsPath(AccountId)));

        _env.Time.Advance(TimeSpan.FromHours(1));
        await _env.SyncTransactions();
        Assert.Equal(3, _env.GoCardless.RequestCount(TransactionsPath(AccountId)));
    }

    [Fact]
    public async Task FailedRequest_IsRecordedUntilARetrySucceeds()
    {
        await SeedAccountsWithActiveRequest(AccountId);
        var accountId = await GetAccountId(AccountId);

        await _env.SyncTransactions();

        var failed = await _env.GetRetrievalMetadata(ThirdPartyDataType.Transactions, accountId);
        Assert.Equal(1, failed.ConsecutiveFailures);
        Assert.Contains("Not found", failed.LastError);
        Assert.Equal(_env.UtcNow.AddHours(1), failed.NextRetryTime);

        _env.Time.Advance(TimeSpan.FromHours(1));
        _env.GoCardless.Respond(TransactionsPath(AccountId), Transactions((TransactionId, -10m)));
        await _env.SyncTransactions();

        var succeeded = await _env.GetRetrievalMetadata(ThirdPartyDataType.Transactions, accountId);
        Assert.Equal(0, succeeded.ConsecutiveFailures);
        Assert.Null(succeeded.LastError);
        Assert.Null(succeeded.NextRetryTime);
        Assert.Equal(_env.UtcNow, succeeded.LastRetrievalTime);
    }

    [Fact]
    public async Task RateLimitedRequest_IsRetriedOnceTheRateLimitResets()
    {
        await SeedAccountsWithActiveRequest(AccountId);
        _env.GoCardless.RespondRateLimited(TransactionsPath(AccountId), TimeSpan.FromHours(3));

        await _env.SyncTransactions();
        _env.Time.Advance(TimeSpan.FromHours(2));
        await _env.SyncTransactions();
        Assert.Equal(1, _env.GoCardless.RequestCount(TransactionsPath(AccountId)));

        _env.Time.Advance(TimeSpan.FromHours(1));
        await _env.SyncTransactions();
        Assert.Equal(2, _env.GoCardless.RequestCount(TransactionsPath(AccountId)));
    }

    [Fact]
    public async Task ForcedSync_IgnoresTheRetryDelay()
    {
        await SeedAccountsWithActiveRequest(AccountId);

        await _env.SyncTransactions();
        await _env.SyncTransactions(force: true);

        Assert.Equal(2, _env.GoCardless.RequestCount(TransactionsPath(AccountId)));
    }

    [Fact]
    public async Task ReconnectedAccount_SkipsTransactionsTheEarlierConnectionRetrieved()
    {
        // The earlier connection retrieved the transaction under another id, the bank's id stays the same
        var account = TestData.Account(AccountId);
        var earlier = TestData.Transaction(Guid.NewGuid(), account, -10m);
        earlier.BankTransactionId = $"bank-{TransactionId}";
        await _env.Seed(TestData.Request(Guid.NewGuid(), _env.UtcNow.AddDays(-1), account), earlier);
        await _env.Seed(new ThirdPartyDataRetrievalMetadata
        {
            DataType = ThirdPartyDataType.Transactions,
            DataSource = DataSource.GoCardless,
            SynchronizationEntityId = account.Id,
            LastRetrievalTime = DateTime.MinValue,
            RetrievalInterval = TimeSpan.FromDays(1),
            RetrieveFrom = new DateTime(2026, 11, 10)
        });
        _env.GoCardless.Respond(TransactionsPath(AccountId),
            Transactions((TransactionId, -10m), (OtherTransactionId, 25m)));

        await _env.SyncTransactions();
        await _env.SyncTransactions(force: true);

        var transactions = await GetTransactions(AccountId);
        Assert.Equal(new[] { earlier.ExternalId, OtherTransactionId.ToString() }.Order(),
            transactions.Select(t => t.ExternalId).Order());
    }

    [Fact]
    public async Task ManualAccount_IsNotSynced()
    {
        var account = TestData.ManualAccount();
        await _env.Seed(account);

        Assert.True(await _env.SyncTransactions(account.Id, force: true));

        Assert.Equal(0, _env.GoCardless.TotalRequestCount);
    }

    private Task SeedAccountsWithActiveRequest(params Guid[] accountIds) =>
        _env.Seed(TestData.Request(Guid.NewGuid(), _env.UtcNow.AddDays(-1),
            accountIds.Select(id => TestData.Account(id)).ToArray()));

    private async Task<Guid> GetAccountId(Guid accountExternalId)
    {
        await using var dbContext = _env.CreateDbContext();
        var account = await dbContext.BankAccounts.SingleAsync(account =>
            account.ExternalId == accountExternalId.ToString());
        return account.Id!.Value;
    }

    private async Task<List<Transaction>> GetTransactions(Guid accountExternalId)
    {
        await using var dbContext = _env.CreateDbContext();
        return await dbContext.Transactions
            .Where(transaction => transaction.BankAccount!.ExternalId == accountExternalId.ToString())
            .ToListAsync();
    }
}