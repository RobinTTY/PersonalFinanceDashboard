using Microsoft.EntityFrameworkCore;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Tests.TestUtilities;
using static RobinTTY.PersonalFinanceDashboard.Infrastructure.Tests.TestUtilities.GoCardlessJson;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Tests.DataSynchronization;

public class BankAccountSyncHandlerTests : IDisposable
{
    private static readonly Guid AccountId = Guid.Parse("a1000000-0000-0000-0000-000000000001");
    private const string InstitutionId = "TESTBANK_TESTDEFF";

    private readonly SyncTestEnvironment _env = new();

    public void Dispose() => _env.Dispose();

    [Fact]
    public async Task AccountWithKnownCurrency_OnlyTheBalanceIsFetched()
    {
        await _env.Seed(TestData.Request(Guid.NewGuid(), _env.UtcNow.AddDays(-1),
            TestData.Account(AccountId, currency: "EUR", balance: 10m)));
        _env.GoCardless.Respond(BalancesPath(AccountId), Balances(123.45m));

        Assert.True(await _env.SyncBankAccounts());

        Assert.Equal(123.45m, (await GetAccount()).Balance);
        Assert.Equal(0, _env.GoCardless.RequestCount(DetailsPath(AccountId)));
    }

    [Fact]
    public async Task AccountWithoutCurrency_GetsItsDetailsAndTheExistingInstitution()
    {
        await _env.Seed(TestData.Institution(InstitutionId),
            TestData.Request(Guid.NewGuid(), _env.UtcNow.AddDays(-1), TestData.Account(AccountId, currency: null)));
        _env.GoCardless.Respond(BalancesPath(AccountId), Balances(50m));
        _env.GoCardless.Respond(AccountPath(AccountId), Account(AccountId, InstitutionId));
        _env.GoCardless.Respond(DetailsPath(AccountId), Details(AccountId, "EUR", "Giro account"));

        Assert.True(await _env.SyncBankAccounts());

        var account = await GetAccount();
        Assert.Equal("EUR", account.Currency);
        Assert.Equal("Giro account", account.Name);
        Assert.Equal(InstitutionId, account.AssociatedInstitution?.ThirdPartyId);

        await using var dbContext = _env.CreateDbContext();
        Assert.Equal(1, await dbContext.BankingInstitutions.CountAsync(i => i.ThirdPartyId == InstitutionId));
    }

    [Fact]
    public async Task AccountOfTwoActiveAuthRequests_IsFetchedOnce()
    {
        // E.g. after renewing the consent for a bank
        var account = TestData.Account(AccountId);
        await _env.Seed(TestData.Request(Guid.NewGuid(), _env.UtcNow.AddDays(-30), account),
            TestData.Request(Guid.NewGuid(), _env.UtcNow.AddDays(-1), account));
        _env.GoCardless.Respond(BalancesPath(AccountId), Balances(50m));

        Assert.True(await _env.SyncBankAccounts());

        Assert.Equal(1, _env.GoCardless.RequestCount(BalancesPath(AccountId)));
    }

    [Fact]
    public async Task AccountsOfAuthRequestsOlderThan90Days_AreNotFetched()
    {
        await _env.Seed(TestData.Request(Guid.NewGuid(), _env.UtcNow.AddDays(-91), TestData.Account(AccountId)));

        Assert.True(await _env.SyncBankAccounts());

        Assert.Equal(0, _env.GoCardless.RequestCount(BalancesPath(AccountId)));
    }

    private async Task<BankAccount> GetAccount()
    {
        await using var dbContext = _env.CreateDbContext();
        return await dbContext.BankAccounts
            .Include(account => account.AssociatedInstitution)
            .SingleAsync(account => account.ThirdPartyId == AccountId);
    }
}