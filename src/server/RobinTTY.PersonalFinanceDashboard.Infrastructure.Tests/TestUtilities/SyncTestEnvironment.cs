using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Time.Testing;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Repositories;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Services;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Services.DataSynchronization;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Tests.TestUtilities;

/// <summary>
/// An in-memory SQLite database, a fake GoCardless API and a fake clock for testing the sync handlers.
/// </summary>
internal sealed class SyncTestEnvironment : IDisposable
{
    private readonly SqliteConnection _connection;

    public FakeGoCardless GoCardless { get; } = new();
    public FakeTimeProvider Time { get; } = new(new DateTimeOffset(2026, 11, 13, 12, 0, 0, TimeSpan.Zero));
    public DateTime UtcNow => Time.GetUtcNow().UtcDateTime;

    public SyncTestEnvironment()
    {
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

    public Task<bool> SyncTransactions(Guid? accountId = null, bool force = false) =>
        Run(dbContext => new TransactionSyncHandler(dbContext, GoCardless.CreateService(),
                CreateMetadataService(dbContext), Time, NullLogger<TransactionSyncHandler>.Instance)
            .SynchronizeData(accountId, force));

    public Task<bool> SyncBankAccounts(Guid? accountId = null, bool force = false) =>
        Run(dbContext => new BankAccountSyncHandler(dbContext, GoCardless.CreateService(),
                CreateMetadataService(dbContext), Time, NullLogger<BankAccountSyncHandler>.Instance)
            .SynchronizeData(accountId, force));

    public Task<bool> SyncAuthenticationRequests(Guid? requestId = null, bool force = false) =>
        Run(dbContext => new AuthenticationRequestSyncHandler(dbContext, GoCardless.CreateService(),
                CreateMetadataService(dbContext), NullLogger<AuthenticationRequestSyncHandler>.Instance)
            .SynchronizeData(requestId, force));

    public Task<bool> SyncBankingInstitutions(Guid? institutionId = null, bool force = false) =>
        Run(dbContext => new BankingInstitutionSyncHandler(dbContext, GoCardless.CreateService(),
                CreateMetadataService(dbContext), NullLogger<BankingInstitutionSyncHandler>.Instance)
            .SynchronizeData(institutionId, force));

    public AuthenticationRequestRepository CreateAuthenticationRequestRepository(ApplicationDbContext dbContext) =>
        new(dbContext, GoCardless.CreateService(),
            new AuthenticationRequestSyncHandler(dbContext, GoCardless.CreateService(),
                CreateMetadataService(dbContext), NullLogger<AuthenticationRequestSyncHandler>.Instance),
            NullLogger<AuthenticationRequestRepository>.Instance);

    public ThirdPartyDataRetrievalMetadataService CreateMetadataService(ApplicationDbContext dbContext) =>
        new(new ThirdPartyDataRetrievalMetadataRepository(dbContext, Time), Time);

    public void Dispose() => _connection.Dispose();

    // Each sync gets its own context, like a request in the API, so assertions read what was saved
    private async Task<bool> Run(Func<ApplicationDbContext, Task<bool>> sync)
    {
        await using var dbContext = CreateDbContext();
        return await sync(dbContext);
    }
}