using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Time.Testing;
using RobinTTY.PersonalFinanceDashboard.Core.DataProviders;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Repositories;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Services;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Services.DataSynchronization;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Services.DataSynchronization.Interfaces;
using RobinTTY.PersonalFinanceDashboard.TestUtilities;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Tests.TestUtilities;

/// <summary>
/// An in-memory SQLite database, a fake GoCardless API and a fake clock for testing the sync handlers.
/// </summary>
internal sealed class SyncTestEnvironment : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly bool _useFakeProvider;

    public FakeGoCardless GoCardless { get; } = new();
    public FakeBankDataProvider Provider { get; } = new();
    public FakeTimeProvider Time { get; } = new(new DateTimeOffset(2026, 11, 13, 12, 0, 0, TimeSpan.Zero));
    public DateTime UtcNow => Time.GetUtcNow().UtcDateTime;
    public DataSynchronizationLock SyncLock { get; } = new();
    public DataSynchronizationTrigger Trigger { get; } = new();
    public CountingNotifier Notifier { get; } = new();

    /// <param name="useFakeProvider">Whether the handlers get their data from <see cref="Provider"/> instead
    /// of <see cref="GoCardless"/>.</param>
    public SyncTestEnvironment(bool useFakeProvider = false)
    {
        _useFakeProvider = useFakeProvider;

        // The in-memory database lives as long as the connection is open
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        using var dbContext = CreateDbContext();
        dbContext.Database.EnsureCreated();
    }

    public ApplicationDbContext CreateDbContext() =>
        new(new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlite(_connection).Options);

    public async Task Seed(params object[] entities)
    {
        await using var dbContext = CreateDbContext();
        dbContext.AddRange(entities);
        await dbContext.SaveChangesAsync();
    }

    public Task<bool> SyncTransactions(bool force = false, CancellationToken cancellationToken = default) =>
        Run(dbContext => CreateTransactionSyncHandler(dbContext, CreateMetadataService(dbContext))
            .SynchronizeData(force, cancellationToken));

    public Task<bool> SyncBankAccounts(Guid? accountId = null, bool force = false) =>
        Run(dbContext => CreateBankAccountSyncHandler(dbContext, CreateMetadataService(dbContext))
            .SynchronizeData(accountId, force));

    public Task<bool> SyncAuthenticationRequests(Guid? requestId = null, bool force = false) =>
        Run(dbContext => CreateAuthenticationRequestSyncHandler(dbContext, CreateMetadataService(dbContext))
            .SynchronizeData(requestId, force));

    public Task<bool> SyncBankingInstitutions(bool force = false) =>
        Run(dbContext => CreateBankingInstitutionSyncHandler(dbContext, CreateMetadataService(dbContext))
            .SynchronizeData(force));

    /// <summary>
    /// Runs <paramref name="synchronize"/> with a <see cref="DataSynchronizer"/> that has its own context.
    /// </summary>
    public Task<bool> Synchronize(Func<IDataSynchronizer, Task<bool>> synchronize) =>
        Run(dbContext => synchronize(CreateSynchronizer(dbContext)));

    public AuthenticationRequestRepository CreateAuthenticationRequestRepository(ApplicationDbContext dbContext) =>
        new(dbContext, CreateProviders(), NullLogger<AuthenticationRequestRepository>.Instance);

    public async Task<ThirdPartyDataRetrievalMetadata> GetRetrievalMetadata(ThirdPartyDataType dataType,
        Guid? synchronizationEntityId = null)
    {
        await using var dbContext = CreateDbContext();
        return await dbContext.ThirdPartyDataRetrievalMetadata.SingleAsync(metadata =>
            metadata.DataType == dataType && metadata.SynchronizationEntityId == synchronizationEntityId);
    }

    public BankDataProviderRegistry CreateProviders() =>
        new([_useFakeProvider ? Provider : GoCardless.CreateService()]);

    public ThirdPartyDataRetrievalMetadataService CreateMetadataService(ApplicationDbContext dbContext) =>
        new(new ThirdPartyDataRetrievalMetadataRepository(dbContext), Time);

    public void Dispose()
    {
        _connection.Dispose();
        SyncLock.Dispose();
    }

    // Like in the API, the handlers of a synchronization share their metadata service
    private DataSynchronizer CreateSynchronizer(ApplicationDbContext dbContext)
    {
        var metadataService = CreateMetadataService(dbContext);
        return new DataSynchronizer(dbContext, SyncLock, Trigger,
            CreateBankingInstitutionSyncHandler(dbContext, metadataService),
            CreateAuthenticationRequestSyncHandler(dbContext, metadataService),
            CreateBankAccountSyncHandler(dbContext, metadataService),
            CreateTransactionSyncHandler(dbContext, metadataService),
            metadataService, Notifier, Time);
    }

    private TransactionSyncHandler CreateTransactionSyncHandler(ApplicationDbContext dbContext,
        ThirdPartyDataRetrievalMetadataService metadataService) =>
        new(dbContext, CreateProviders(), metadataService, Time, NullLogger<TransactionSyncHandler>.Instance);

    private BankAccountSyncHandler CreateBankAccountSyncHandler(ApplicationDbContext dbContext,
        ThirdPartyDataRetrievalMetadataService metadataService) =>
        new(dbContext, CreateProviders(), metadataService, Time, NullLogger<BankAccountSyncHandler>.Instance);

    private AuthenticationRequestSyncHandler CreateAuthenticationRequestSyncHandler(ApplicationDbContext dbContext,
        ThirdPartyDataRetrievalMetadataService metadataService) =>
        new(dbContext, CreateProviders(), metadataService, Time,
            NullLogger<AuthenticationRequestSyncHandler>.Instance);

    private BankingInstitutionSyncHandler CreateBankingInstitutionSyncHandler(ApplicationDbContext dbContext,
        ThirdPartyDataRetrievalMetadataService metadataService) =>
        new(dbContext, CreateProviders(), metadataService, NullLogger<BankingInstitutionSyncHandler>.Instance);

    // Each sync gets its own context, like a request in the API, so assertions read what was saved
    private async Task<bool> Run(Func<ApplicationDbContext, Task<bool>> sync)
    {
        await using var dbContext = CreateDbContext();
        return await sync(dbContext);
    }
}

/// <summary>
/// Counts the notifications about synchronized data.
/// </summary>
internal sealed class CountingNotifier : IDataSynchronizationNotifier
{
    private int _count;

    public int Count => _count;

    public Task NotifyDataSynchronized()
    {
        Interlocked.Increment(ref _count);
        return Task.CompletedTask;
    }
}
