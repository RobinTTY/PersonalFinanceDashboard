using System.Text.Json;
using RobinTTY.PersonalFinanceDashboard.API.Tests.TestUtilities;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Tests.TestUtilities;
using static RobinTTY.PersonalFinanceDashboard.Infrastructure.Tests.TestUtilities.GoCardlessJson;

namespace RobinTTY.PersonalFinanceDashboard.API.Tests;

public class BankAccountQueryTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    // Only the sync status is selected, so the account id it's looked up by has to be projected anyway
    private const string SyncStatusQuery = """
                                           query($id: UUID!) {
                                             bankAccount(accountId: $id) { syncStatus { state lastError nextRetry } }
                                           }
                                           """;

    [Fact]
    public async Task BankAccount_IsReadWithoutRetrievingItFromGoCardless()
    {
        var externalId = Guid.NewGuid();
        var account = TestData.Account(externalId);
        await factory.Seed(TestData.Request(Guid.NewGuid(), DateTime.UtcNow.AddDays(-1), account));

        await _client.PostGraphQl("query($id: UUID!) { bankAccount(accountId: $id) { name } }",
            new { id = account.Id });

        Assert.Equal(0, factory.GoCardless.RequestCount(BalancesPath(externalId)));
    }

    [Fact]
    public async Task SyncStatus_OfAManualAccountIsNotSynced()
    {
        var account = await factory.Seed(TestData.ManualAccount());

        var response = await _client.PostGraphQl(SyncStatusQuery, new { id = account.Id });

        var status = response.Data("bankAccount", "syncStatus");
        Assert.Equal("NOT_SYNCED", status.GetProperty("state").GetString());
        Assert.Equal(JsonValueKind.Null, status.GetProperty("lastError").ValueKind);
    }

    [Fact]
    public async Task SyncStatus_OfAnAccountThatFailedToSyncIsRetrying()
    {
        // No GoCardless response is configured, so syncing the account fails
        var account = TestData.Account(Guid.NewGuid());
        await factory.Seed(TestData.Request(Guid.NewGuid(), DateTime.UtcNow.AddDays(-1), account));
        await _client.PostGraphQl("mutation { synchronizeBankAccountData { boolean } }");

        var response = await _client.PostGraphQl(SyncStatusQuery, new { id = account.Id });

        var status = response.Data("bankAccount", "syncStatus");
        Assert.Equal("RETRYING", status.GetProperty("state").GetString());
        Assert.Contains("Not found", status.GetProperty("lastError").GetString());
        Assert.Equal(JsonValueKind.String, status.GetProperty("nextRetry").ValueKind);
    }

    [Fact]
    public async Task SyncStatus_OfAnAccountWithAnExpiredAuthenticationNeedsReauthentication()
    {
        var account = TestData.Account(Guid.NewGuid());
        await factory.Seed(TestData.Request(Guid.NewGuid(), DateTime.UtcNow.AddDays(-91), account));

        var response = await _client.PostGraphQl(SyncStatusQuery, new { id = account.Id });

        Assert.Equal("NEEDS_REAUTHENTICATION",
            response.Data("bankAccount", "syncStatus").GetProperty("state").GetString());
    }
}