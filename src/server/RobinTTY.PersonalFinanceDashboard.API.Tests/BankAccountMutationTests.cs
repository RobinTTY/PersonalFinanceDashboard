using System.Text.Json;
using RobinTTY.PersonalFinanceDashboard.API.Tests.TestUtilities;
using RobinTTY.PersonalFinanceDashboard.TestUtilities;

namespace RobinTTY.PersonalFinanceDashboard.API.Tests;

public class BankAccountMutationTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task CreateBankAccount_IsManual()
    {
        var response = await _client.PostGraphQl("""
                                                 mutation($input: CreateBankAccountInput!) {
                                                   createBankAccount(input: $input) { bankAccount { source externalId } }
                                                 }
                                                 """, new { input = new { name = "Cash" } });

        var account = response.Data("createBankAccount", "bankAccount");
        Assert.Equal("MANUAL", account.GetProperty("source").GetString());
        Assert.Equal(JsonValueKind.Null, account.GetProperty("externalId").ValueKind);
    }

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
    public async Task UpdateBankAccount_OfASynchronizedAccountReplacesTheProviderNameUntilCleared()
    {
        var account = TestData.Account(Guid.NewGuid());
        account.ProviderDescription = "Giro";
        await factory.Seed(account);
        const string rename = """
                              mutation($input: UpdateBankAccountInput!) {
                                updateBankAccount(input: $input) { bankAccount { id } }
                              }
                              """;
        // Only the resolved fields are selected, so the provider's values have to be projected anyway
        const string query = "query($id: UUID!) { bankAccount(accountId: $id) { name description } }";

        var synchronized = (await _client.PostGraphQl(query, new { id = account.Id })).Data("bankAccount");
        await _client.PostGraphQl(rename,
            new { input = new { id = account.Id, name = "Household", description = "Shared expenses" } });
        var renamed = (await _client.PostGraphQl(query, new { id = account.Id })).Data("bankAccount");
        await _client.PostGraphQl(rename, new { input = new { id = account.Id, name = "", description = "" } });
        var cleared = (await _client.PostGraphQl(query, new { id = account.Id })).Data("bankAccount");

        Assert.Equal(("Account", "Giro"), NameAndDescription(synchronized));
        Assert.Equal(("Household", "Shared expenses"), NameAndDescription(renamed));
        Assert.Equal(("Account", "Giro"), NameAndDescription(cleared));
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
    public async Task SetBankAccountIncludeInAnalytics_UpdatesTheAccount()
    {
        var accountId = await CreateBankAccount(_client);

        var response = await _client.PostGraphQl("""
                                                 mutation($id: UUID!) {
                                                   setBankAccountIncludeInAnalytics(input: { bankAccountId: $id, includeInAnalytics: false }) {
                                                     bankAccount { includeInAnalytics }
                                                   }
                                                 }
                                                 """, new { id = accountId });

        var account = response.Data("setBankAccountIncludeInAnalytics", "bankAccount");
        Assert.False(account.GetProperty("includeInAnalytics").GetBoolean());
    }

    [Fact]
    public async Task SetBankAccountIncludeInAnalytics_UnknownIdReturnsNotFound()
    {
        var response = await _client.PostGraphQl("""
                                                 mutation($id: UUID!) {
                                                   setBankAccountIncludeInAnalytics(input: { bankAccountId: $id, includeInAnalytics: false }) {
                                                     bankAccount { id }
                                                   }
                                                 }
                                                 """, new { id = Guid.NewGuid() });

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

    private static (string?, string?) NameAndDescription(JsonElement account) =>
        (account.GetProperty("name").GetString(), account.GetProperty("description").GetString());
}
