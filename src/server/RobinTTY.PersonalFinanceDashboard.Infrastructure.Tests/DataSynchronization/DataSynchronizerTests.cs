using Microsoft.EntityFrameworkCore;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Tests.TestUtilities;
using RobinTTY.PersonalFinanceDashboard.TestUtilities;
using static RobinTTY.PersonalFinanceDashboard.TestUtilities.GoCardlessJson;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Tests.DataSynchronization;

public sealed class DataSynchronizerTests : IDisposable
{
    private static readonly Guid RequestId = Guid.Parse("e1000000-0000-0000-0000-000000000001");
    private static readonly Guid AccountId = Guid.Parse("a1000000-0000-0000-0000-000000000001");
    private static readonly Guid TransactionId = Guid.Parse("70000000-0000-0000-0000-000000000001");

    private readonly SyncTestEnvironment _env = new();

    public void Dispose() => _env.Dispose();

    [Fact]
    public async Task SynchronizeAll_RetrievesTheDetailsAndTransactionsOfNewlyLinkedAccounts()
    {
        _env.GoCardless.Respond(RequisitionsPath,
            Requisitions(Requisition(RequestId, "LN", _env.UtcNow.AddDays(-1), AccountId)));
        _env.GoCardless.Respond(AccountPath(AccountId), Account(AccountId, "SANDBOXFINANCE_SFIN0000"));
        _env.GoCardless.Respond(DetailsPath(AccountId), Details(AccountId, "EUR", "Giro account"));
        _env.GoCardless.Respond(BalancesPath(AccountId), Balances(42m));
        _env.GoCardless.Respond(TransactionsPath(AccountId), Transactions((TransactionId, -10m)));

        await _env.Synchronize(synchronizer => synchronizer.SynchronizeAll());

        await using var dbContext = _env.CreateDbContext();
        var account = await dbContext.BankAccounts.Include(account => account.Transactions).SingleAsync();
        Assert.Equal(42m, account.Balance);
        Assert.Equal(-10m, account.Transactions.Single().Amount);
    }

    [Fact]
    public async Task ConcurrentSynchronizations_RetrieveTheDataOnlyOnce()
    {
        await SeedActiveRequest();
        _env.GoCardless.Respond(TransactionsPath(AccountId), Transactions((TransactionId, -10m)));

        // The second synchronization starts while the first one is still waiting for GoCardless
        var releaseResponses = _env.GoCardless.HoldResponses();
        var first = SynchronizeTransactions();
        var second = SynchronizeTransactions();
        releaseResponses();

        Assert.All(await Task.WhenAll(first, second), Assert.True);
        Assert.Equal(1, _env.GoCardless.RequestCount(TransactionsPath(AccountId)));
        await using var dbContext = _env.CreateDbContext();
        Assert.Single(await dbContext.Transactions.ToListAsync());
    }

    [Fact]
    public async Task SynchronizationThatRetrievedData_NotifiesOnce()
    {
        await SeedActiveRequest();
        _env.GoCardless.Respond(BalancesPath(AccountId), Balances(42m));
        _env.GoCardless.Respond(TransactionsPath(AccountId), Transactions((TransactionId, -10m)));

        await _env.Synchronize(synchronizer => synchronizer.SynchronizeAll());

        Assert.Equal(1, _env.Notifier.Count);
    }

    [Fact]
    public async Task FailedRetrieval_AlsoNotifies()
    {
        // The sync status of the account changes
        await SeedActiveRequest();

        await SynchronizeTransactions();

        Assert.Equal(1, _env.Notifier.Count);
    }

    [Fact]
    public async Task SynchronizationWithNothingDue_DoesNotNotify()
    {
        await SeedActiveRequest();
        _env.GoCardless.Respond(TransactionsPath(AccountId), Transactions((TransactionId, -10m)));
        await SynchronizeTransactions();

        await SynchronizeTransactions();

        Assert.Equal(1, _env.Notifier.Count);
    }

    [Fact]
    public async Task RefreshAuthenticationRequest_SynchronizesTheAccountsOfAnActiveRequest()
    {
        var requestId = await SeedActiveRequest();
        _env.GoCardless.Respond(RequisitionPath(RequestId),
            RequisitionJson(RequestId, "LN", _env.UtcNow.AddDays(-1), AccountId));
        _env.GoCardless.Respond(BalancesPath(AccountId), Balances(42m));

        Assert.True(await _env.Synchronize(synchronizer => synchronizer.RefreshAuthenticationRequest(requestId)));

        Assert.Equal(1, _env.GoCardless.RequestCount(BalancesPath(AccountId)));
        Assert.Equal(0, _env.GoCardless.RequestCount(TransactionsPath(AccountId)));
    }

    [Fact]
    public async Task RefreshAuthenticationRequest_OfAnActiveRequest_RequestsASynchronizationForTheTransactions()
    {
        var requestId = await SeedActiveRequest();
        _env.GoCardless.Respond(RequisitionPath(RequestId),
            RequisitionJson(RequestId, "LN", _env.UtcNow.AddDays(-1), AccountId));

        await _env.Synchronize(synchronizer => synchronizer.RefreshAuthenticationRequest(requestId));

        Assert.True(IsSynchronizationRequested());
    }

    [Fact]
    public async Task RefreshAuthenticationRequest_OfARequestWaitingForAccess_RequestsNoSynchronization()
    {
        var request = TestData.Request(RequestId, _env.UtcNow);
        await _env.Seed(request);
        _env.GoCardless.Respond(RequisitionPath(RequestId), RequisitionJson(RequestId, "CR", _env.UtcNow));

        await _env.Synchronize(synchronizer => synchronizer.RefreshAuthenticationRequest(request.Id!.Value));

        Assert.False(IsSynchronizationRequested());
    }

    [Fact]
    public async Task CancelledSynchronization_StopsWithoutRecordingAFailedRetrieval()
    {
        await SeedActiveRequest();
        _env.GoCardless.Respond(TransactionsPath(AccountId), Transactions((TransactionId, -10m)));
        _env.GoCardless.HoldResponses();
        using var cancellation = new CancellationTokenSource();

        var synchronization = _env.Synchronize(synchronizer =>
            synchronizer.Synchronize(ThirdPartyDataType.Transactions, cancellationToken: cancellation.Token));
        // Cancels while GoCardless is being asked
        var deadline = DateTime.UtcNow + TimeSpan.FromSeconds(5);
        while (_env.GoCardless.TotalRequestCount == 0 && DateTime.UtcNow < deadline)
            await Task.Delay(10);
        await cancellation.CancelAsync();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            synchronization.WaitAsync(TimeSpan.FromSeconds(5)));
        await using var dbContext = _env.CreateDbContext();
        Assert.False(await dbContext.ThirdPartyDataRetrievalMetadata.AnyAsync(metadata =>
            metadata.DataType == ThirdPartyDataType.Transactions));
        Assert.Equal(0, _env.Notifier.Count);
    }

    private Task<bool> SynchronizeTransactions() =>
        _env.Synchronize(synchronizer => synchronizer.Synchronize(ThirdPartyDataType.Transactions));

    private async Task<Guid> SeedActiveRequest()
    {
        var request = TestData.Request(RequestId, _env.UtcNow.AddDays(-1), TestData.Account(AccountId));
        await _env.Seed(request);
        return request.Id!.Value;
    }

    private bool IsSynchronizationRequested() => _env.Trigger.WaitForRequest(CancellationToken.None).IsCompleted;
}