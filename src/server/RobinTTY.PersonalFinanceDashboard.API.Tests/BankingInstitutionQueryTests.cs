using System.Text.Json;
using RobinTTY.PersonalFinanceDashboard.API.Tests.TestUtilities;
using RobinTTY.PersonalFinanceDashboard.Core.Models;

namespace RobinTTY.PersonalFinanceDashboard.API.Tests;

public class BankingInstitutionQueryTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task BankingInstitution_IsLookedUpById()
    {
        var institution = await factory.Seed(Institution("DE"));

        var response = await _client.PostGraphQl(
            "query($id: UUID!) { bankingInstitution(institutionId: $id) { name countries } }",
            new { id = institution.Id });

        var result = response.Data("bankingInstitution");
        Assert.Equal(institution.Name, result.GetProperty("name").GetString());
        Assert.Equal("DE", Assert.Single(result.GetProperty("countries").EnumerateArray()).GetString());
    }

    [Fact]
    public async Task BankingInstitution_UnknownIdReturnsNull()
    {
        var response = await _client.PostGraphQl(
            "query($id: UUID!) { bankingInstitution(institutionId: $id) { name } }", new { id = Guid.NewGuid() });

        Assert.Equal(JsonValueKind.Null, response.Data("bankingInstitution").ValueKind);
    }

    [Fact]
    public async Task BankingInstitutions_AreFilteredByCountry()
    {
        var french = await factory.Seed(Institution("FR"));
        var german = await factory.Seed(Institution("DE"));

        var response = await _client.PostGraphQl(
            """query { bankingInstitutions(countryCode: "FR", first: 50) { nodes { id } } }""");

        var ids = response.Data("bankingInstitutions", "nodes").EnumerateArray()
            .Select(node => node.GetProperty("id").GetString()).ToList();
        Assert.Contains(french.Id.ToString(), ids);
        Assert.DoesNotContain(german.Id.ToString(), ids);
    }

    [Fact]
    public async Task BankingInstitutions_ArePaged()
    {
        await factory.Seed(Institution("DE"));
        await factory.Seed(Institution("DE"));

        var response = await _client.PostGraphQl(
            "query { bankingInstitutions(first: 1) { nodes { id } pageInfo { hasNextPage } } }");

        var page = response.Data("bankingInstitutions");
        Assert.Single(page.GetProperty("nodes").EnumerateArray());
        Assert.True(page.GetProperty("pageInfo").GetProperty("hasNextPage").GetBoolean());
    }

    private static BankingInstitution Institution(string country) =>
        new(DataSource.Manual, null, "TESTDEFF", $"Bank {Guid.NewGuid()}", new Uri("https://localhost/logo.png"),
            [country]);
}
