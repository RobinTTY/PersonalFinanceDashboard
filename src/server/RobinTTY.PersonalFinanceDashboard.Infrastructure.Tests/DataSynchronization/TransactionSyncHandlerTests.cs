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
        Assert.Equal(-12m, transactions.Single(t => t.ThirdPartyId == TransactionId).Amount);
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
    public async Task SameThirdPartyIdInTwoAccounts_CreatesATransactionForEachAccount()
    {
        // Third party ids are only unique per account
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

    private Task SeedAccountsWithActiveRequest(params Guid[] accountIds) =>
        _env.Seed(TestData.Request(Guid.NewGuid(), _env.UtcNow.AddDays(-1),
            accountIds.Select(id => TestData.Account(id)).ToArray()));

    private async Task<List<Transaction>> GetTransactions(Guid accountThirdPartyId)
    {
        await using var dbContext = _env.CreateDbContext();
        return await dbContext.Transactions
            .Where(transaction => transaction.BankAccount!.ThirdPartyId == accountThirdPartyId)
            .ToListAsync();
    }
}