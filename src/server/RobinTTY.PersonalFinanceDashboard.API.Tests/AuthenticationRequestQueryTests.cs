using RobinTTY.PersonalFinanceDashboard.API.Tests.TestUtilities;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Tests.TestUtilities;

namespace RobinTTY.PersonalFinanceDashboard.API.Tests;

public class AuthenticationRequestQueryTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

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
}