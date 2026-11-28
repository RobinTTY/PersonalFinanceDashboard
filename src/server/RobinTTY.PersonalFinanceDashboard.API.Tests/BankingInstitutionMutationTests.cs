using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RobinTTY.PersonalFinanceDashboard.API.Tests.TestUtilities;
using RobinTTY.PersonalFinanceDashboard.Infrastructure;
using RobinTTY.PersonalFinanceDashboard.TestUtilities;

namespace RobinTTY.PersonalFinanceDashboard.API.Tests;

public class BankingInstitutionMutationTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task DeleteBankingInstitution_KeepsItsAccounts()
    {
        var institutionId = Guid.Parse(await CreateBankingInstitution());
        var accountId = Guid.Parse(await BankAccountMutationTests.CreateBankAccount(_client));
        await WithDbContext(async dbContext =>
        {
            var account = await dbContext.BankAccounts.SingleAsync(a => a.Id == accountId);
            account.AssociatedInstitution = await dbContext.BankingInstitutions.SingleAsync(i => i.Id == institutionId);
            await dbContext.SaveChangesAsync();
        });

        var deleted = await _client.PostGraphQl("""
                                                mutation($id: UUID!) {
                                                  deleteBankingInstitution(input: { bankingInstitutionId: $id }) { deleteResponse { deletedId } }
                                                }
                                                """, new { id = institutionId });

        Assert.Equal(institutionId.ToString(),
            deleted.Data("deleteBankingInstitution", "deleteResponse", "deletedId").GetString());
        await WithDbContext(async dbContext =>
        {
            var account = await dbContext.BankAccounts.Include(a => a.AssociatedInstitution)
                .SingleAsync(a => a.Id == accountId);
            Assert.Null(account.AssociatedInstitution);
        });
    }

    [Fact]
    public async Task DeleteBankingInstitution_OfADataSourceIsRefused()
    {
        // The synchronization would recreate it
        var institution = await factory.Seed(TestData.Institution(Guid.NewGuid().ToString()));

        var response = await _client.PostGraphQl("""
                                                 mutation($id: UUID!) {
                                                   deleteBankingInstitution(input: { bankingInstitutionId: $id }) { deleteResponse { deletedId } }
                                                 }
                                                 """, new { id = institution.Id });

        Assert.Equal("BANKING_INSTITUTION_SYNCHRONIZED", response.ErrorCode());
        await WithDbContext(async dbContext =>
            Assert.True(await dbContext.BankingInstitutions.AnyAsync(i => i.Id == institution.Id)));
    }

    private async Task<string> CreateBankingInstitution()
    {
        var response = await _client.PostGraphQl("""
                                                 mutation($input: CreateBankingInstitutionInput!) {
                                                   createBankingInstitution(input: $input) { bankingInstitution { id } }
                                                 }
                                                 """,
            new
            {
                input = new
                {
                    name = "Test Bank",
                    bic = "TESTDEFF",
                    logoUri = "https://localhost/logo.png",
                    countries = new[] { "DE" }
                }
            });

        return response.Data("createBankingInstitution", "bankingInstitution", "id").GetString()!;
    }

    private async Task WithDbContext(Func<ApplicationDbContext, Task> action)
    {
        using var scope = factory.Services.CreateScope();
        await action(scope.ServiceProvider.GetRequiredService<ApplicationDbContext>());
    }
}
