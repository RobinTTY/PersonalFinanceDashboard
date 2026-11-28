using System.Text.Json;
using RobinTTY.PersonalFinanceDashboard.API.Tests.TestUtilities;
using RobinTTY.PersonalFinanceDashboard.TestUtilities;

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
        Assert.Equal("Weekly shopping", transaction.GetProperty("description").GetString());
        Assert.Equal("BOOKED", transaction.GetProperty("status").GetString());
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
    public async Task UpdateTransaction_BlankValuesRemoveCategoryAndNotes()
    {
        var transactionId = await CreateTransactionForNewAccount();
        const string mutation = """
                                mutation($input: UpdateTransactionInput!) {
                                  updateTransaction(input: $input) { transaction { category notes } }
                                }
                                """;
        await _client.PostGraphQl(mutation,
            new { input = new { id = transactionId, category = "Groceries", notes = "Weekly shopping" } });

        var response = await _client.PostGraphQl(mutation, new { input = new { id = transactionId, category = " " } });

        var transaction = response.Data("updateTransaction", "transaction");
        Assert.Equal(JsonValueKind.Null, transaction.GetProperty("category").ValueKind);
        Assert.Equal(JsonValueKind.Null, transaction.GetProperty("notes").ValueKind);
    }

    [Fact]
    public async Task UpdateTransaction_UnknownIdReturnsNotFound()
    {
        var response = await _client.PostGraphQl("""
                                                 mutation($input: UpdateTransactionInput!) {
                                                   updateTransaction(input: $input) { transaction { id } }
                                                 }
                                                 """,
            new { input = new { id = Guid.NewGuid() } });

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

    [Theory]
    [InlineData(-1, "TRANSACTION_SYNCHRONIZED")]
    [InlineData(-100, null)]
    public async Task DeleteTransaction_OfASynchronizedAccountIsOnlyAllowedOnceItsAccessExpired(
        int requestAgeInDays, string? errorCode)
    {
        // Otherwise the synchronization would recreate it
        var account = TestData.Account(Guid.NewGuid());
        var transaction = TestData.Transaction(Guid.NewGuid(), account, -10m);
        account.Transactions.Add(transaction);
        await factory.Seed(TestData.Request(Guid.NewGuid(), DateTime.UtcNow.AddDays(requestAgeInDays), account));

        var response = await _client.PostGraphQl("""
                                                 mutation($id: UUID!) {
                                                   deleteTransaction(input: { transactionId: $id }) { deleteResponse { deletedId } }
                                                 }
                                                 """, new { id = transaction.Id });

        if (errorCode is null)
            Assert.Equal(transaction.Id.ToString(),
                response.Data("deleteTransaction", "deleteResponse", "deletedId").GetString());
        else
            Assert.Equal(errorCode, response.ErrorCode());
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
                             createTransaction(input: $input) {
                               transaction { id amount description status bankAccount { id } }
                             }
                           }
                           """, new
        {
            input = new
            {
                accountId,
                amount = -12.5m,
                currency = "EUR",
                payee = "Supermarket",
                description = "Weekly shopping",
                valueDate = "2026-11-10T00:00:00Z"
            }
        });
}
