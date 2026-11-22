using RobinTTY.PersonalFinanceDashboard.API.Tests.TestUtilities;
using RobinTTY.PersonalFinanceDashboard.TestUtilities;
using static RobinTTY.PersonalFinanceDashboard.TestUtilities.GoCardlessJson;

namespace RobinTTY.PersonalFinanceDashboard.API.Tests;

public class AuthenticationRequestQueryTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private const string StatusQuery = """
                                       query($id: UUID!, $refresh: Boolean!) {
                                         authenticationRequest(authenticationId: $id, refresh: $refresh) { status }
                                       }
                                       """;

    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task AuthenticationRequest_IsOnlyRetrievedFromGoCardlessWhenRefreshed()
    {
        var externalId = Guid.NewGuid();
        var request = await factory.Seed(TestData.Request(externalId, DateTime.UtcNow));
        factory.GoCardless.Respond(RequisitionPath(externalId), RequisitionJson(externalId, "LN", DateTime.UtcNow));

        await _client.PostGraphQl(StatusQuery, new { id = request.Id, refresh = false });
        Assert.Equal(0, factory.GoCardless.RequestCount(RequisitionPath(externalId)));

        var response = await _client.PostGraphQl(StatusQuery, new { id = request.Id, refresh = true });
        Assert.Equal(1, factory.GoCardless.RequestCount(RequisitionPath(externalId)));
        Assert.Equal("ACTIVE", response.Data("authenticationRequest", "status").GetString());
    }

    [Fact]
    public async Task AccessExpiresAt_Is90DaysAfterTheRequestWasCreated()
    {
        // Only the expiry is selected, so the creation time it's calculated from has to be projected anyway
        var createdAt = new DateTime(2026, 10, 1, 12, 0, 0, DateTimeKind.Utc);
        var request = await factory.Seed(TestData.Request(Guid.NewGuid(), createdAt));

        var response = await _client.PostGraphQl("""
                                                 query($id: UUID!) {
                                                   authenticationRequest(authenticationId: $id) { accessExpiresAt }
                                                 }
                                                 """, new { id = request.Id });

        var expiresAt = response.Data("authenticationRequest", "accessExpiresAt").GetDateTime();
        Assert.Equal(createdAt.AddDays(90), expiresAt.ToUniversalTime());
    }

    [Fact]
    public async Task AuthenticationRequests_AreListedWithTheirAccounts()
    {
        var account = TestData.Account(Guid.NewGuid());
        var request = await factory.Seed(TestData.Request(Guid.NewGuid(), DateTime.UtcNow, account));

        var response = await _client.PostGraphQl("query { authenticationRequests { id associatedAccounts { id } } }");

        var listed = Assert.Single(response.Data("authenticationRequests").EnumerateArray(),
            node => node.GetProperty("id").GetString() == request.Id.ToString());
        var listedAccount = Assert.Single(listed.GetProperty("associatedAccounts").EnumerateArray());
        Assert.Equal(account.Id.ToString(), listedAccount.GetProperty("id").GetString());
    }
}