using System.Text.Json;
using RobinTTY.PersonalFinanceDashboard.API.Tests.TestUtilities;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.TestUtilities;
using static RobinTTY.PersonalFinanceDashboard.TestUtilities.GoCardlessJson;

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
    public async Task BankAccounts_AreFilteredAndPagedByName()
    {
        var prefix = Guid.NewGuid().ToString();
        foreach (var name in new[] { "C", "A", "B" })
            await factory.Seed(new BankAccount(DataSource.Manual, null, name: $"{prefix} {name}", currency: "EUR"));
        const string query = """
                             query($prefix: String!, $after: String) {
                               bankAccounts(first: 2, after: $after, where: { name: { startsWith: $prefix } }) {
                                 nodes { name }
                                 pageInfo { hasNextPage endCursor }
                               }
                             }
                             """;

        var firstPage = (await _client.PostGraphQl(query, new { prefix, after = (string?)null })).Data("bankAccounts");
        var endCursor = firstPage.GetProperty("pageInfo").GetProperty("endCursor").GetString();
        var secondPage = (await _client.PostGraphQl(query, new { prefix, after = endCursor })).Data("bankAccounts");

        Assert.Equal([$"{prefix} A", $"{prefix} B"], Names(firstPage));
        Assert.True(firstPage.GetProperty("pageInfo").GetProperty("hasNextPage").GetBoolean());
        Assert.Equal([$"{prefix} C"], Names(secondPage));
        Assert.False(secondPage.GetProperty("pageInfo").GetProperty("hasNextPage").GetBoolean());
    }

    [Fact]
    public async Task BankAccounts_AreFilteredByIncludeInAnalytics()
    {
        var included = await factory.Seed(TestData.ManualAccount());
        var excludedAccount = TestData.ManualAccount();
        excludedAccount.IncludeInAnalytics = false;
        var excluded = await factory.Seed(excludedAccount);

        var response = await _client.PostGraphQl(
            "query { bankAccounts(first: 50, where: { includeInAnalytics: { eq: true } }) { nodes { id } } }");

        var ids = response.Data("bankAccounts", "nodes").EnumerateArray()
            .Select(node => node.GetProperty("id").GetString()).ToList();
        Assert.Contains(included.Id.ToString(), ids);
        Assert.DoesNotContain(excluded.Id.ToString(), ids);
    }

    [Fact]
    public async Task BankAccounts_IncludeTheirTransactions()
    {
        var account = TestData.ManualAccount();
        account.Transactions.Add(TestData.Transaction(Guid.NewGuid(), account, -12.5m));
        await factory.Seed(account);

        var response = await _client.PostGraphQl(
            "query($id: UUID!) { bankAccounts(where: { id: { eq: $id } }) { nodes { transactions { amount } } } }",
            new { id = account.Id });

        var node = Assert.Single(response.Data("bankAccounts", "nodes").EnumerateArray());
        var transaction = Assert.Single(node.GetProperty("transactions").EnumerateArray());
        Assert.Equal(-12.5m, transaction.GetProperty("amount").GetDecimal());
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

    private static List<string?> Names(JsonElement page) =>
        page.GetProperty("nodes").EnumerateArray().Select(node => node.GetProperty("name").GetString()).ToList();
}