using System.Text.Json;
using RobinTTY.PersonalFinanceDashboard.API.Tests.TestUtilities;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.TestUtilities;

namespace RobinTTY.PersonalFinanceDashboard.API.Tests;

public class TransactionQueryTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private const string TransactionsByAccountQuery = """
                                                      query($id: UUID!, $first: Int!, $after: String) {
                                                        transactionsByAccountId(accountId: $id, first: $first, after: $after) {
                                                          nodes { amount }
                                                          pageInfo { hasNextPage endCursor }
                                                        }
                                                      }
                                                      """;

    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task TransactionsByAccountId_OnlyReturnsTheTransactionsOfTheAccount()
    {
        var account = await SeedAccountWithTransactions(-10m, -20m);
        await SeedAccountWithTransactions(-30m);

        var response = await _client.PostGraphQl(TransactionsByAccountQuery,
            new { id = account.Id, first = 10, after = (string?)null });

        Assert.Equal([-10m, -20m], Amounts(response.Data("transactionsByAccountId")));
    }

    [Fact]
    public async Task TransactionsByAccountId_ArePagedNewestFirst()
    {
        var account = await SeedAccountWithTransactions(-10m, -20m, -30m);

        var firstPage = (await _client.PostGraphQl(TransactionsByAccountQuery,
            new { id = account.Id, first = 2, after = (string?)null })).Data("transactionsByAccountId");
        var endCursor = firstPage.GetProperty("pageInfo").GetProperty("endCursor").GetString();
        var secondPage = (await _client.PostGraphQl(TransactionsByAccountQuery,
            new { id = account.Id, first = 2, after = endCursor })).Data("transactionsByAccountId");

        Assert.Equal([-10m, -20m], Amounts(firstPage));
        Assert.True(firstPage.GetProperty("pageInfo").GetProperty("hasNextPage").GetBoolean());
        Assert.Equal([-30m], Amounts(secondPage));
        Assert.False(secondPage.GetProperty("pageInfo").GetProperty("hasNextPage").GetBoolean());
    }

    [Fact]
    public async Task TransactionsByAccountId_PagesSameDayTransactionsOnce()
    {
        var account = TestData.ManualAccount();
        account.Transactions.AddRange(Enumerable.Range(1, 5)
            .Select(amount => TestData.Transaction(Guid.NewGuid(), account, amount)));
        await factory.Seed(account);

        var amounts = new List<decimal>();
        string? endCursor = null;
        do
        {
            var page = (await _client.PostGraphQl(TransactionsByAccountQuery,
                new { id = account.Id, first = 2, after = endCursor })).Data("transactionsByAccountId");
            amounts.AddRange(Amounts(page));
            var pageInfo = page.GetProperty("pageInfo");
            endCursor = pageInfo.GetProperty("hasNextPage").GetBoolean()
                ? pageInfo.GetProperty("endCursor").GetString()
                : null;
        } while (endCursor != null);

        Assert.Equal([1m, 2m, 3m, 4m, 5m], amounts.Order());
    }

    [Fact]
    public async Task Transactions_IncludeTheirAccount()
    {
        var account = await SeedAccountWithTransactions(-10m);

        var response = await _client.PostGraphQl("query { transactions { id bankAccount { id } } }");

        var transaction = Assert.Single(response.Data("transactions").EnumerateArray(),
            node => node.GetProperty("id").GetString() == account.Transactions[0].Id.ToString());
        Assert.Equal(account.Id.ToString(), transaction.GetProperty("bankAccount").GetProperty("id").GetString());
    }

    /// <summary>
    /// Seeds an account whose transactions are a day apart, the first one being the newest.
    /// </summary>
    private async Task<BankAccount> SeedAccountWithTransactions(params decimal[] amounts)
    {
        var account = TestData.ManualAccount();
        account.Transactions.AddRange(amounts.Select((amount, index) => TestData.Transaction(Guid.NewGuid(), account,
            amount, valueDate: new DateTime(2026, 11, 10).AddDays(-index))));
        return await factory.Seed(account);
    }

    private static List<decimal> Amounts(JsonElement page) =>
        page.GetProperty("nodes").EnumerateArray().Select(node => node.GetProperty("amount").GetDecimal()).ToList();
}
