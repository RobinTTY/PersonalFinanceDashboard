using RobinTTY.PersonalFinanceDashboard.API.Tests.TestUtilities;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Tests.TestUtilities;
using static RobinTTY.PersonalFinanceDashboard.Infrastructure.Tests.TestUtilities.GoCardlessJson;

namespace RobinTTY.PersonalFinanceDashboard.API.Tests;

public class AuthenticationMutationTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private const string CreateMutation = """
                                          mutation($institutionId: UUID!, $redirectUri: String!) {
                                            createAuthenticationRequest(input: { institutionId: $institutionId, redirectUri: $redirectUri }) {
                                              authenticationRequest { id }
                                            }
                                          }
                                          """;

    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task CreateAuthenticationRequest_InvalidRedirectUriReturnsAnError()
    {
        var institution = await factory.Seed(TestData.Institution("SANDBOXFINANCE_SFIN0000"));
        // The factory is shared within this class, so other tests may have called GoCardless already
        var requestsBefore = factory.GoCardless.TotalRequestCount;

        var response = await _client.PostGraphQl(CreateMutation,
            new { institutionId = institution.Id, redirectUri = "not a uri" });

        Assert.Equal("INVALID_REDIRECT_URI", response.ErrorCode());
        Assert.Equal(requestsBefore, factory.GoCardless.TotalRequestCount);
    }

    [Fact]
    public async Task CreateAuthenticationRequest_ReturnsTheCreatedRequest()
    {
        var institution = await factory.Seed(TestData.Institution("SANDBOX_CREATED"));
        var requisitionId = Guid.NewGuid();
        factory.GoCardless.Respond(RequisitionsPath, RequisitionJson(requisitionId, "CR", DateTime.UtcNow));

        var response = await _client.PostGraphQl("""
                                                 mutation($institutionId: UUID!, $redirectUri: String!) {
                                                   createAuthenticationRequest(input: { institutionId: $institutionId, redirectUri: $redirectUri }) {
                                                     authenticationRequest { source externalId }
                                                   }
                                                 }
                                                 """,
            new { institutionId = institution.Id, redirectUri = "https://localhost/" });

        var request = response.Data("createAuthenticationRequest", "authenticationRequest");
        Assert.Equal("GO_CARDLESS", request.GetProperty("source").GetString());
        Assert.Equal(requisitionId.ToString(), request.GetProperty("externalId").GetString());
    }

    [Fact]
    public async Task CreateAuthenticationRequest_StoresTheInstitution()
    {
        var institution = await factory.Seed(TestData.Institution("SANDBOX_STORED"));
        factory.GoCardless.Respond(RequisitionsPath, RequisitionJson(Guid.NewGuid(), "CR", DateTime.UtcNow));
        var created = await _client.PostGraphQl(CreateMutation,
            new { institutionId = institution.Id, redirectUri = "https://localhost/" });
        var requestId = created.Data("createAuthenticationRequest", "authenticationRequest", "id").GetGuid();

        var response = await _client.PostGraphQl("""
                                                 query($id: UUID!) {
                                                   authenticationRequest(authenticationId: $id) { institution { id } }
                                                 }
                                                 """, new { id = requestId });

        Assert.Equal(institution.Id,
            response.Data("authenticationRequest", "institution", "id").GetGuid());
    }

    [Fact]
    public async Task CreateAuthenticationRequest_GoCardlessErrorIsReturned()
    {
        var institution = await factory.Seed(TestData.Institution("SANDBOX_FAILING"));
        // Clears a response another test in this class may have configured, so GoCardless answers with a 404
        factory.GoCardless.Remove(RequisitionsPath);

        var response = await _client.PostGraphQl(CreateMutation,
            new { institutionId = institution.Id, redirectUri = "https://localhost/" });

        Assert.Equal("CREATE_AUTHENTICATION_REQUEST_FAILED", response.ErrorCode());
        Assert.StartsWith("Not found", response.ErrorMessage());
    }

    [Fact]
    public async Task CreateAuthenticationRequest_UnknownInstitutionReturnsNotFound()
    {
        var requestsBefore = factory.GoCardless.TotalRequestCount;

        var response = await _client.PostGraphQl(CreateMutation,
            new { institutionId = Guid.NewGuid(), redirectUri = "https://localhost/" });

        Assert.Equal("BANKING_INSTITUTION_NOT_FOUND", response.ErrorCode());
        Assert.Equal(requestsBefore, factory.GoCardless.TotalRequestCount);
    }

    [Fact]
    public async Task CreateAuthenticationRequest_ManualInstitutionIsNotConnectable()
    {
        var institution = await factory.Seed(TestData.ManualInstitution());
        var requestsBefore = factory.GoCardless.TotalRequestCount;

        var response = await _client.PostGraphQl(CreateMutation,
            new { institutionId = institution.Id, redirectUri = "https://localhost/" });

        Assert.Equal("INSTITUTION_NOT_CONNECTABLE", response.ErrorCode());
        Assert.Equal(requestsBefore, factory.GoCardless.TotalRequestCount);
    }

    [Fact]
    public async Task DeleteAuthenticationRequest_UnknownIdReturnsAnError()
    {
        var response = await _client.PostGraphQl("""
                                                 mutation($id: UUID!) {
                                                   deleteAuthenticationRequest(input: { authenticationId: $id }) { deleteResponse { deletedId } }
                                                 }
                                                 """, new { id = Guid.NewGuid() });

        Assert.Equal("DELETE_AUTHENTICATION_REQUEST_FAILED", response.ErrorCode());
    }
}