using Microsoft.EntityFrameworkCore;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Tests.TestUtilities;
using RobinTTY.PersonalFinanceDashboard.TestUtilities;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Tests;

public sealed class SourcedEntityTests : IDisposable
{
    private readonly SyncTestEnvironment _env = new();

    public void Dispose() => _env.Dispose();

    [Theory]
    [InlineData(DataSource.Manual, "external-id")]
    [InlineData(DataSource.GoCardless, null)]
    public void Constructor_RejectsSourceAndExternalIdMismatch(DataSource source, string? externalId)
    {
        Assert.Throws<ArgumentException>(() => new BankAccount(source, externalId));
    }

    [Fact]
    public async Task Database_RejectsExternalEntityWithoutExternalId()
    {
        var account = TestData.ManualAccount();
        account.Source = DataSource.GoCardless;

        await Assert.ThrowsAsync<DbUpdateException>(() => _env.Seed(account));
    }

    [Fact]
    public async Task Database_AllowsTheSameExternalIdFromDifferentSources()
    {
        // Simulates a second provider using the same id as GoCardless
        var otherSourceAccount = TestData.Account(Guid.Parse("a1000000-0000-0000-0000-000000000001"));
        await _env.Seed(TestData.Account(Guid.Parse("a1000000-0000-0000-0000-000000000001")));
        await using (var dbContext = _env.CreateDbContext())
        {
            await dbContext.Database.ExecuteSqlRawAsync("UPDATE \"BankAccounts\" SET \"Source\" = 'Other'");
        }

        await _env.Seed(otherSourceAccount);

        await using var verifyContext = _env.CreateDbContext();
        Assert.Equal(2, await verifyContext.BankAccounts.CountAsync());
    }

    [Fact]
    public async Task Database_AllowsMultipleManualEntities()
    {
        await _env.Seed(TestData.ManualAccount(), TestData.ManualAccount());

        await using var dbContext = _env.CreateDbContext();
        Assert.Equal(2, await dbContext.BankAccounts.CountAsync(account => account.Source == DataSource.Manual));
    }
}
