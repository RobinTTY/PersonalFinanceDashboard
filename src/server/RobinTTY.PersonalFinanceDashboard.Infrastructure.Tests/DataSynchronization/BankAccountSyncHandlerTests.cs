using Microsoft.EntityFrameworkCore;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Tests.TestUtilities;
using RobinTTY.PersonalFinanceDashboard.TestUtilities;
using static RobinTTY.PersonalFinanceDashboard.TestUtilities.GoCardlessJson;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Tests.DataSynchronization;

public sealed class BankAccountSyncHandlerTests : IDisposable
{
    private static readonly Guid AccountId = Guid.Parse("a1000000-0000-0000-0000-000000000001");
    private static readonly Guid OtherAccountId = Guid.Parse("a2000000-0000-0000-0000-000000000002");
    private static readonly Guid ThirdAccountId = Guid.Parse("a3000000-0000-0000-0000-000000000003");
    private const string MaskedPan = "123456XXXXXX1234";
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
        Assert.Equal("Giro account", account.ProviderName);
        Assert.Equal(InstitutionId, account.AssociatedInstitution?.ExternalId);

        await using var dbContext = _env.CreateDbContext();
        Assert.Equal(1, await dbContext.BankingInstitutions.CountAsync(i => i.ExternalId == InstitutionId));
    }

    [Fact]
    public async Task RenamedAccount_KeepsItsNameWhenItsDetailsAreFetched()
    {
        var renamed = TestData.Account(AccountId, currency: null);
        renamed.Name = "Household";
        renamed.Description = "Shared expenses";
        await _env.Seed(TestData.Institution(InstitutionId),
            TestData.Request(Guid.NewGuid(), _env.UtcNow.AddDays(-1), renamed));
        _env.GoCardless.Respond(BalancesPath(AccountId), Balances(50m));
        _env.GoCardless.Respond(AccountPath(AccountId), Account(AccountId, InstitutionId));
        _env.GoCardless.Respond(DetailsPath(AccountId), Details(AccountId, "EUR", "Giro account"));

        Assert.True(await _env.SyncBankAccounts());

        var account = await GetAccount();
        Assert.Equal("Household", account.Name);
        Assert.Equal("Shared expenses", account.Description);
        Assert.Equal("Giro account", account.ProviderName);
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

    [Fact]
    public async Task FailedAccount_IsOnlyRetriedAfterADelay()
    {
        // No response is configured for the other account, so its request fails
        await _env.Seed(TestData.Request(Guid.NewGuid(), _env.UtcNow.AddDays(-1),
            TestData.Account(AccountId, currency: "EUR"), TestData.Account(OtherAccountId, currency: "EUR")));
        _env.GoCardless.Respond(BalancesPath(AccountId), Balances(50m));

        await _env.SyncBankAccounts();
        await _env.SyncBankAccounts();
        Assert.Equal(1, _env.GoCardless.RequestCount(BalancesPath(OtherAccountId)));

        // The failure is recorded for the account that failed, not the one that succeeded
        var failed =
            await _env.GetRetrievalMetadata(ThirdPartyDataType.BankAccounts, await GetAccountId(OtherAccountId));
        Assert.Equal(1, failed.ConsecutiveFailures);
        var succeeded = await _env.GetRetrievalMetadata(ThirdPartyDataType.BankAccounts, await GetAccountId(AccountId));
        Assert.Equal(0, succeeded.ConsecutiveFailures);

        _env.Time.Advance(TimeSpan.FromHours(1));
        await _env.SyncBankAccounts();
        Assert.Equal(2, _env.GoCardless.RequestCount(BalancesPath(OtherAccountId)));
        Assert.Equal(1, _env.GoCardless.RequestCount(BalancesPath(AccountId)));
    }

    [Fact]
    public async Task ReconnectedAccount_TakesThePlaceOfTheExpiredOneWithTheSameIban()
    {
        // The expired account's transactions and settings are kept, the reconnect created a placeholder
        var expired = TestData.Account(OtherAccountId);
        expired.Iban = "DE00123456789";
        expired.IncludeInAnalytics = false;
        var expiredRequest = TestData.Request(Guid.NewGuid(), _env.UtcNow.AddDays(-100), expired);
        await _env.Seed(expiredRequest, TestData.Transaction(Guid.NewGuid(), expired, -10m));
        var newRequest = TestData.Request(Guid.NewGuid(), _env.UtcNow.AddDays(-1),
            TestData.Account(AccountId, currency: null));
        await _env.Seed(newRequest);
        RespondWithAccount(AccountId);

        Assert.True(await _env.SyncBankAccounts());

        await using var dbContext = _env.CreateDbContext();
        var account = await dbContext.BankAccounts
            .Include(a => a.AssociatedAuthenticationRequests)
            .SingleAsync();
        Assert.Equal(expired.Id, account.Id);
        Assert.Equal(AccountId.ToString(), account.ExternalId);
        Assert.False(account.IncludeInAnalytics);
        Assert.Equal("EUR", account.Currency);
        Assert.Equal(new DateTime(2026, 11, 10),
            (await _env.GetRetrievalMetadata(ThirdPartyDataType.Transactions, account.Id)).RetrieveFrom);
        Assert.Equal(new[] { expiredRequest.Id, newRequest.Id }.Order(),
            account.AssociatedAuthenticationRequests.Select(r => r.Id).Order());
        Assert.Equal(1, await dbContext.Transactions.CountAsync());
    }

    [Fact]
    public async Task AccountWithTheSameIbanThatIsStillSynced_IsNotTakenOver()
    {
        var other = TestData.Account(OtherAccountId);
        other.Iban = "DE00123456789";
        await _env.Seed(TestData.Request(Guid.NewGuid(), _env.UtcNow.AddDays(-30), other),
            TestData.Request(Guid.NewGuid(), _env.UtcNow.AddDays(-1), TestData.Account(AccountId, currency: null)));
        RespondWithAccount(AccountId);
        _env.GoCardless.Respond(BalancesPath(OtherAccountId), Balances(10m));

        Assert.True(await _env.SyncBankAccounts());

        await using var dbContext = _env.CreateDbContext();
        Assert.Equal([AccountId.ToString(), OtherAccountId.ToString()],
            await dbContext.BankAccounts.Select(a => a.ExternalId).OrderBy(id => id).ToListAsync());
    }

    [Theory]
    [InlineData("1234567890", null)]
    [InlineData(null, MaskedPan)]
    public async Task ReconnectedAccountWithoutIban_TakesThePlaceOfTheExpiredOneWithTheSameBbanOrCard(
        string? bban, string? maskedPan)
    {
        var institution = TestData.Institution(InstitutionId);
        var expired = ExpiredAccount(OtherAccountId, institution, bban, maskedPan);
        await _env.Seed(institution, TestData.Request(Guid.NewGuid(), _env.UtcNow.AddDays(-100), expired),
            TestData.Request(Guid.NewGuid(), _env.UtcNow.AddDays(-1), TestData.Account(AccountId, currency: null)));
        RespondWithAccount(AccountId, iban: null, bban, maskedPan);

        Assert.True(await _env.SyncBankAccounts());

        await using var dbContext = _env.CreateDbContext();
        var account = await dbContext.BankAccounts.SingleAsync();
        Assert.Equal(expired.Id, account.Id);
        Assert.Equal(AccountId.ToString(), account.ExternalId);
    }

    [Fact]
    public async Task CardWithTheSameMaskedNumberAtAnotherInstitution_IsNotTakenOver()
    {
        var otherInstitution = TestData.Institution("OTHERBANK_OTHERDEFF");
        await _env.Seed(otherInstitution,
            TestData.Request(Guid.NewGuid(), _env.UtcNow.AddDays(-100),
                ExpiredAccount(OtherAccountId, otherInstitution, maskedPan: MaskedPan)),
            TestData.Request(Guid.NewGuid(), _env.UtcNow.AddDays(-1), TestData.Account(AccountId, currency: null)));
        RespondWithAccount(AccountId, iban: null, maskedPan: MaskedPan);

        Assert.True(await _env.SyncBankAccounts());

        await using var dbContext = _env.CreateDbContext();
        Assert.Equal(2, await dbContext.BankAccounts.CountAsync());
    }

    [Fact]
    public async Task TwoExpiredCardsWithTheSameMaskedNumber_AreNotTakenOver()
    {
        // It's unclear which one the new connection replaces
        var institution = TestData.Institution(InstitutionId);
        await _env.Seed(institution,
            TestData.Request(Guid.NewGuid(), _env.UtcNow.AddDays(-100),
                ExpiredAccount(OtherAccountId, institution, maskedPan: MaskedPan),
                ExpiredAccount(ThirdAccountId, institution, maskedPan: MaskedPan)),
            TestData.Request(Guid.NewGuid(), _env.UtcNow.AddDays(-1), TestData.Account(AccountId, currency: null)));
        RespondWithAccount(AccountId, iban: null, maskedPan: MaskedPan);

        Assert.True(await _env.SyncBankAccounts());

        await using var dbContext = _env.CreateDbContext();
        Assert.Equal(3, await dbContext.BankAccounts.CountAsync());
    }

    [Fact]
    public async Task AccountWithoutIdentifiers_GetsItsDetailsUntilOneIsKnown()
    {
        // E.g. a card account synced before masked card numbers were stored
        var account = TestData.Account(AccountId);
        account.Iban = null;
        await _env.Seed(TestData.Request(Guid.NewGuid(), _env.UtcNow.AddDays(-1), account));
        RespondWithAccount(AccountId, iban: null, maskedPan: MaskedPan);

        await _env.SyncBankAccounts();
        await _env.SyncBankAccounts(force: true);

        Assert.Equal(MaskedPan, (await GetAccount()).MaskedPan);
        Assert.Equal(1, _env.GoCardless.RequestCount(DetailsPath(AccountId)));
        Assert.Equal(2, _env.GoCardless.RequestCount(BalancesPath(AccountId)));
    }

    [Fact]
    public async Task ManualAccount_IsNotFetched()
    {
        var account = TestData.ManualAccount();
        await _env.Seed(account);

        Assert.True(await _env.SyncBankAccounts(account.Id, force: true));

        Assert.Equal(0, _env.GoCardless.TotalRequestCount);
    }

    private void RespondWithAccount(Guid accountId, string? iban = "DE00123456789", string? bban = null,
        string? maskedPan = null)
    {
        _env.GoCardless.Respond(BalancesPath(accountId), Balances(50m));
        _env.GoCardless.Respond(AccountPath(accountId), Account(accountId, InstitutionId));
        _env.GoCardless.Respond(DetailsPath(accountId),
            Details(accountId, "EUR", "Giro account", iban, bban, maskedPan));
    }

    private static BankAccount ExpiredAccount(Guid externalId, BankingInstitution institution, string? bban = null,
        string? maskedPan = null)
    {
        var account = TestData.Account(externalId);
        account.Iban = null;
        account.Bban = bban;
        account.MaskedPan = maskedPan;
        account.AssociatedInstitution = institution;
        return account;
    }

    private async Task<Guid> GetAccountId(Guid accountExternalId)
    {
        await using var dbContext = _env.CreateDbContext();
        var account = await dbContext.BankAccounts.SingleAsync(account =>
            account.ExternalId == accountExternalId.ToString());
        return account.Id!.Value;
    }

    private async Task<BankAccount> GetAccount()
    {
        await using var dbContext = _env.CreateDbContext();
        return await dbContext.BankAccounts
            .Include(account => account.AssociatedInstitution)
            .SingleAsync(account => account.ExternalId == AccountId.ToString());
    }
}
