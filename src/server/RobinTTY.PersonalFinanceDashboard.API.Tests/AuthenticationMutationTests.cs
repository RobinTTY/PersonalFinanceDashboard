using RobinTTY.PersonalFinanceDashboard.API.Tests.TestUtilities;

namespace RobinTTY.PersonalFinanceDashboard.API.Tests;

public class AuthenticationMutationTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private const string CreateMutation = """
                                          mutation($institutionId: String!, $redirectUri: String!) {
                                            createAuthenticationRequest(input: { institutionId: $institutionId, redirectUri: $redirectUri }) {
                                              authenticationRequest { id }
                                            }
                                          }
                                          """;

    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task CreateAuthenticationRequest_InvalidRedirectUriReturnsAnError()
    {
        // The factory is shared within this class, so other tests may have called GoCardless already
        var requestsBefore = factory.GoCardless.TotalRequestCount;

        var response = await _client.PostGraphQl(CreateMutation,
            new { institutionId = "SANDBOXFINANCE_SFIN0000", redirectUri = "not a uri" });

        Assert.Equal("INVALID_REDIRECT_URI", response.ErrorCode());
        Assert.Equal(requestsBefore, factory.GoCardless.TotalRequestCount);
    }

    [Fact]
    public async Task CreateAuthenticationRequest_GoCardlessErrorIsReturned()
    {
        // The fake GoCardless API has no response configured, so it answers with a 404
        var response = await _client.PostGraphQl(CreateMutation,
            new { institutionId = "SANDBOXFINANCE_SFIN0000", redirectUri = "https://localhost/" });

        Assert.Equal("CREATE_AUTHENTICATION_REQUEST_FAILED", response.ErrorCode());
        Assert.StartsWith("Not found", response.ErrorMessage());
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