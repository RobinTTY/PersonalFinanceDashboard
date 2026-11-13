using RobinTTY.PersonalFinanceDashboard.API.Tests.TestUtilities;

namespace RobinTTY.PersonalFinanceDashboard.API.Tests;

public class BankAccountMutationTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task UpdateBankAccount_ChangesNameAndDescription()
    {
        var accountId = await CreateBankAccount(_client);

        var response = await _client.PostGraphQl("""
                                                 mutation($input: UpdateBankAccountInput!) {
                                                   updateBankAccount(input: $input) { bankAccount { name description balance } }
                                                 }
                                                 """,
            new { input = new { id = accountId, name = "Savings", description = "Emergency fund" } });

        var account = response.Data("updateBankAccount", "bankAccount");
        Assert.Equal("Savings", account.GetProperty("name").GetString());
        Assert.Equal("Emergency fund", account.GetProperty("description").GetString());
        Assert.Equal(100m, account.GetProperty("balance").GetDecimal());
    }

    [Fact]
    public async Task UpdateBankAccount_UnknownIdReturnsNotFound()
    {
        var response = await _client.PostGraphQl("""
                                                 mutation($input: UpdateBankAccountInput!) {
                                                   updateBankAccount(input: $input) { bankAccount { id } }
                                                 }
                                                 """, new { input = new { id = Guid.NewGuid(), name = "Savings" } });

        Assert.Equal("BANK_ACCOUNT_NOT_FOUND", response.ErrorCode());
    }

    [Fact]
    public async Task DeleteBankAccount_ReturnsTheDeletedIdAndThenNotFound()
    {
        var accountId = await CreateBankAccount(_client);
        const string mutation = """
                                mutation($id: UUID!) {
                                  deleteBankAccount(input: { bankAccountId: $id }) { deleteResponse { deletedId } }
                                }
                                """;

        var deleted = await _client.PostGraphQl(mutation, new { id = accountId });
        var deletedAgain = await _client.PostGraphQl(mutation, new { id = accountId });

        Assert.Equal(accountId, deleted.Data("deleteBankAccount", "deleteResponse", "deletedId").GetString());
        Assert.Equal("BANK_ACCOUNT_NOT_FOUND", deletedAgain.ErrorCode());
    }

    [Fact]
    public async Task DeleteBankAccount_AlsoDeletesItsTransactions()
    {
        var accountId = await CreateBankAccount(_client);
        var transactionId = await TransactionMutationTests.CreateTransaction(_client, accountId);

        var deleted = await _client.PostGraphQl("""
                                                mutation($id: UUID!) {
                                                  deleteBankAccount(input: { bankAccountId: $id }) { deleteResponse { deletedId } }
                                                }
                                                """, new { id = accountId });
        var deletedTransaction = await _client.PostGraphQl("""
                                                           mutation($id: UUID!) {
                                                             deleteTransaction(input: { transactionId: $id }) { deleteResponse { deletedId } }
                                                           }
                                                           """, new { id = transactionId });

        Assert.Equal(accountId, deleted.Data("deleteBankAccount", "deleteResponse", "deletedId").GetString());
        Assert.Equal("TRANSACTION_NOT_FOUND", deletedTransaction.ErrorCode());
    }

    /// <summary>
    /// Creates a manual bank account and returns its id.
    /// </summary>
    internal static async Task<string> CreateBankAccount(HttpClient client)
    {
        var response = await client.PostGraphQl("""
                                                mutation($input: CreateBankAccountInput!) {
                                                  createBankAccount(input: $input) { bankAccount { id } }
                                                }
                                                """,
            new { input = new { name = "Checking", balance = 100m, currency = "EUR" } });

        return response.Data("createBankAccount", "bankAccount", "id").GetString()!;
    }
}