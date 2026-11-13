using Microsoft.EntityFrameworkCore;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Tests.TestUtilities;
using static RobinTTY.PersonalFinanceDashboard.Infrastructure.Tests.TestUtilities.GoCardlessJson;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Tests.DataSynchronization;

public class BankingInstitutionSyncHandlerTests : IDisposable
{
    private readonly SyncTestEnvironment _env = new();

    public void Dispose() => _env.Dispose();

    [Fact]
    public async Task EmptyDatabase_AllInstitutionsAreAdded()
    {
        _env.GoCardless.Respond(InstitutionsPath, Institutions(("BANK_A", "Bank A"), ("BANK_B", "Bank B")));

        Assert.True(await _env.SyncBankingInstitutions());

        Assert.Equal(["BANK_A", "BANK_B"], (await GetInstitutions()).Select(i => i.ThirdPartyId).Order());
    }

    [Fact]
    public async Task ExistingInstitutions_AreUpdatedAddedAndRemoved()
    {
        await _env.Seed(TestData.Institution("BANK_A"), TestData.Institution("BANK_OLD"));
        _env.GoCardless.Respond(InstitutionsPath, Institutions(("BANK_A", "Renamed Bank A"), ("BANK_NEW", "New Bank")));

        await _env.SyncBankingInstitutions();

        var institutions = await GetInstitutions();
        Assert.Equal(["BANK_A", "BANK_NEW"], institutions.Select(i => i.ThirdPartyId).Order());
        Assert.Equal("Renamed Bank A", institutions.Single(i => i.ThirdPartyId == "BANK_A").Name);
    }

    [Fact]
    public async Task ManuallyCreatedInstitutions_AreKept()
    {
        await _env.Seed(TestData.Institution("BANK_A"), TestData.Institution(string.Empty));
        _env.GoCardless.Respond(InstitutionsPath, Institutions(("BANK_A", "Bank A")));

        await _env.SyncBankingInstitutions();

        Assert.Contains(await GetInstitutions(), institution => institution.ThirdPartyId == string.Empty);
    }

    [Fact]
    public async Task RemovedInstitution_IsUnlinkedFromItsAccounts()
    {
        var account = TestData.Account(Guid.NewGuid());
        account.AssociatedInstitution = TestData.Institution("BANK_OLD");
        await _env.Seed(TestData.Institution("BANK_A"), account);
        _env.GoCardless.Respond(InstitutionsPath, Institutions(("BANK_A", "Bank A")));

        Assert.True(await _env.SyncBankingInstitutions());

        await using var dbContext = _env.CreateDbContext();
        var storedAccount = await dbContext.BankAccounts.Include(a => a.AssociatedInstitution).SingleAsync();
        Assert.Null(storedAccount.AssociatedInstitution);
        Assert.DoesNotContain(await GetInstitutions(), institution => institution.ThirdPartyId == "BANK_OLD");
    }

    [Fact]
    public async Task FreshInstitutions_AreOnlyFetchedAgainAfterSevenDays()
    {
        _env.GoCardless.Respond(InstitutionsPath, Institutions(("BANK_A", "Bank A")));

        await _env.SyncBankingInstitutions();
        await _env.SyncBankingInstitutions();
        Assert.Equal(1, _env.GoCardless.RequestCount(InstitutionsPath));

        _env.Time.Advance(TimeSpan.FromDays(7));
        await _env.SyncBankingInstitutions();
        Assert.Equal(2, _env.GoCardless.RequestCount(InstitutionsPath));
    }

    [Fact]
    public async Task FailedRequest_KeepsTheExistingInstitutions()
    {
        // No response is configured, so the request fails
        await _env.Seed(TestData.Institution("BANK_A"));

        Assert.False(await _env.SyncBankingInstitutions());

        Assert.Single(await GetInstitutions());
    }

    private async Task<List<BankingInstitution>> GetInstitutions()
    {
        await using var dbContext = _env.CreateDbContext();
        return await dbContext.BankingInstitutions.ToListAsync();
    }
}