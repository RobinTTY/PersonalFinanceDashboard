using System.Text.Json;
using RobinTTY.PersonalFinanceDashboard.API.Tests.TestUtilities;

namespace RobinTTY.PersonalFinanceDashboard.API.Tests;

public class TransactionMutationTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task CreateTransaction_IsLinkedToTheAccount()
    {
        var accountId = await BankAccountMutationTests.CreateBankAccount(_client);

        var response = await CreateTransaction(accountId);

        var transaction = response.Data("createTransaction", "transaction");
        Assert.Equal(-12.5m, transaction.GetProperty("amount").GetDecimal());
        Assert.Equal(accountId, transaction.GetProperty("bankAccount").GetProperty("id").GetString());
    }

    [Fact]
    public async Task CreateTransaction_UnknownAccountReturnsNotFound()
    {
        var response = await CreateTransaction(Guid.NewGuid().ToString());

        Assert.Equal("BANK_ACCOUNT_NOT_FOUND", response.ErrorCode());
    }

    [Fact]
    public async Task UpdateTransaction_ChangesCategoryAndNotes()
    {
        var transactionId = await CreateTransactionForNewAccount();

        var response = await _client.PostGraphQl("""
                                                 mutation($input: UpdateTransactionInput!) {
                                                   updateTransaction(input: $input) { transaction { category notes amount } }
                                                 }
                                                 """,
            new { input = new { id = transactionId, category = "Groceries", notes = "Weekly shopping" } });

        var transaction = response.Data("updateTransaction", "transaction");
        Assert.Equal("Groceries", transaction.GetProperty("category").GetString());
        Assert.Equal("Weekly shopping", transaction.GetProperty("notes").GetString());
        Assert.Equal(-12.5m, transaction.GetProperty("amount").GetDecimal());
    }

    [Fact]
    public async Task UpdateTransaction_UnknownIdReturnsNotFound()
    {
        var response = await _client.PostGraphQl("""
                                                 mutation($input: UpdateTransactionInput!) {
                                                   updateTransaction(input: $input) { transaction { id } }
                                                 }
                                                 """,
            new { input = new { id = Guid.NewGuid(), category = "", notes = "" } });

        Assert.Equal("TRANSACTION_NOT_FOUND", response.ErrorCode());
    }

    [Fact]
    public async Task DeleteTransaction_ReturnsTheDeletedIdAndThenNotFound()
    {
        var transactionId = await CreateTransactionForNewAccount();
        const string mutation = """
                                mutation($id: UUID!) {
                                  deleteTransaction(input: { transactionId: $id }) { deleteResponse { deletedId } }
                                }
                                """;

        var deleted = await _client.PostGraphQl(mutation, new { id = transactionId });
        var deletedAgain = await _client.PostGraphQl(mutation, new { id = transactionId });

        Assert.Equal(transactionId, deleted.Data("deleteTransaction", "deleteResponse", "deletedId").GetString());
        Assert.Equal("TRANSACTION_NOT_FOUND", deletedAgain.ErrorCode());
    }

    private async Task<string> CreateTransactionForNewAccount()
    {
        var accountId = await BankAccountMutationTests.CreateBankAccount(_client);
        return await CreateTransaction(_client, accountId);
    }

    /// <summary>
    /// Creates a transaction for the given account and returns its id.
    /// </summary>
    internal static async Task<string> CreateTransaction(HttpClient client, string accountId)
    {
        var response = await PostCreateTransaction(client, accountId);
        return response.Data("createTransaction", "transaction", "id").GetString()!;
    }

    private Task<JsonElement> CreateTransaction(string accountId) => PostCreateTransaction(_client, accountId);

    private static Task<JsonElement> PostCreateTransaction(HttpClient client, string accountId) =>
        client.PostGraphQl("""
                           mutation($input: CreateTransactionInput!) {
                             createTransaction(input: $input) { transaction { id amount bankAccount { id } } }
                           }
                           """, new
        {
            input = new
            {
                accountId, amount = -12.5m, currency = "EUR", category = "", notes = "",
                payee = "Supermarket", valueDate = "2026-11-10T00:00:00Z"
            }
        });
}