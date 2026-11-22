using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Services;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Tests.TestUtilities;
using RobinTTY.PersonalFinanceDashboard.TestUtilities;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Tests.Services;

public sealed class BankAccountSyncStatusServiceTests : IDisposable
{
    private readonly SyncTestEnvironment _env = new();

    public void Dispose() => _env.Dispose();

    [Fact]
    public async Task ManualAccount_IsNotSynced()
    {
        var account = TestData.ManualAccount();
        await _env.Seed(account);

        Assert.Equal(new SyncStatus(SyncState.NotSynced), await GetSyncStatus(account));
    }

    [Fact]
    public async Task AccountWithSuccessfulRetrievals_IsUpToDateSinceTheOlderOne()
    {
        var account = await SeedAccountWithRequest(_env.UtcNow.AddDays(-1));
        await _env.Seed(Metadata(account, ThirdPartyDataType.BankAccounts, _env.UtcNow.AddHours(-1)),
            Metadata(account, ThirdPartyDataType.Transactions, _env.UtcNow.AddHours(-3)));

        Assert.Equal(new SyncStatus(SyncState.UpToDate, _env.UtcNow.AddHours(-3)), await GetSyncStatus(account));
    }

    [Fact]
    public async Task AccountWhoseTransactionsWereNeverRetrieved_HasNoLastSuccessfulSync()
    {
        var account = await SeedAccountWithRequest(_env.UtcNow.AddDays(-1));
        await _env.Seed(Metadata(account, ThirdPartyDataType.BankAccounts, _env.UtcNow.AddHours(-1)));

        Assert.Equal(new SyncStatus(SyncState.UpToDate), await GetSyncStatus(account));
    }

    [Fact]
    public async Task AccountWithAFailedRetrieval_IsRetryingWithTheError()
    {
        var account = await SeedAccountWithRequest(_env.UtcNow.AddDays(-1));
        var failed = Metadata(account, ThirdPartyDataType.Transactions, _env.UtcNow.AddDays(-2));
        failed.ConsecutiveFailures = 2;
        failed.LastError = "Rate limit exceeded";
        failed.NextRetryTime = _env.UtcNow.AddHours(2);
        await _env.Seed(Metadata(account, ThirdPartyDataType.BankAccounts, _env.UtcNow.AddHours(-1)), failed);

        Assert.Equal(new SyncStatus(SyncState.Retrying, _env.UtcNow.AddDays(-2), "Rate limit exceeded",
            _env.UtcNow.AddHours(2)), await GetSyncStatus(account));
    }

    [Fact]
    public async Task AccountWithoutAnActiveAuthentication_NeedsReauthentication()
    {
        var account = await SeedAccountWithRequest(_env.UtcNow.AddDays(-91));

        Assert.Equal(SyncState.NeedsReauthentication, (await GetSyncStatus(account)).State);
    }

    [Fact]
    public async Task UnknownAccount_HasNoStatus()
    {
        var statuses = await GetSyncStatuses(Guid.NewGuid());

        Assert.Empty(statuses);
    }

    private async Task<BankAccount> SeedAccountWithRequest(DateTime requestCreatedAt)
    {
        var account = TestData.Account(Guid.NewGuid());
        await _env.Seed(TestData.Request(Guid.NewGuid(), requestCreatedAt, account));
        return account;
    }

    private static ThirdPartyDataRetrievalMetadata Metadata(BankAccount account, ThirdPartyDataType dataType,
        DateTime lastRetrievalTime) =>
        new()
        {
            DataType = dataType,
            DataSource = DataSource.GoCardless,
            SynchronizationEntityId = account.Id,
            LastRetrievalTime = lastRetrievalTime,
            RetrievalInterval = TimeSpan.FromDays(1)
        };

    private async Task<SyncStatus> GetSyncStatus(BankAccount account) =>
        (await GetSyncStatuses(account.Id!.Value))[account.Id!.Value];

    private async Task<IReadOnlyDictionary<Guid, SyncStatus>> GetSyncStatuses(params Guid[] accountIds)
    {
        await using var dbContext = _env.CreateDbContext();
        return await new BankAccountSyncStatusService(dbContext, _env.Time).GetSyncStatuses(accountIds);
    }
}
