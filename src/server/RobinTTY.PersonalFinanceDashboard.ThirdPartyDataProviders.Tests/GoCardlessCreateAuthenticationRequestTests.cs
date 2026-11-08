using System.Net;
using System.Text;
using Microsoft.Extensions.Logging.Abstractions;
using RobinTTY.NordigenApiClient;
using RobinTTY.NordigenApiClient.Models;
using RobinTTY.NordigenApiClient.Models.Jwt;

namespace RobinTTY.PersonalFinanceDashboard.ThirdPartyDataProviders.Tests;

public class GoCardlessCreateAuthenticationRequestTests
{
    private static readonly Uri RedirectUri = new("https://localhost/");

    [Fact]
    public async Task FieldError_IsReturnedWhenSummaryIsMissing()
    {
        var service = CreateService(HttpStatusCode.BadRequest, """
                                                               {
                                                                 "institution_id": { "summary": "Unknown Institution ID", "detail": "Get Institution IDs from /institutions/" },
                                                                 "status_code": 400
                                                               }
                                                               """);

        var response = await service.CreateAuthenticationRequest("UNKNOWN", RedirectUri);

        Assert.False(response.IsSuccessful);
        Assert.Equal("Unknown Institution ID", response.Error!.Summary);
        Assert.Equal("Get Institution IDs from /institutions/", response.Error.Detail);
    }

    [Fact]
    public async Task TopLevelError_IsReturnedWhenPresent()
    {
        var service = CreateService(HttpStatusCode.Forbidden, """
                                                              { "summary": "Authentication failed", "detail": "No active account found", "status_code": 403 }
                                                              """);

        var response = await service.CreateAuthenticationRequest("SANDBOXFINANCE_SFIN0000", RedirectUri);

        Assert.False(response.IsSuccessful);
        Assert.Equal("Authentication failed", response.Error!.Summary);
    }

    private static GoCardlessDataProviderService CreateService(HttpStatusCode statusCode, string body)
    {
        // A valid token pair keeps the client from requesting a new token
        var token = CreateUnsignedToken(DateTimeOffset.UtcNow.AddDays(1));
        var client = new NordigenClient(new HttpClient(new StubHandler(statusCode, body)),
            new NordigenClientCredentials("secret-id", "secret-key"), new JsonWebTokenPair(token, token));

        return new GoCardlessDataProviderService(client, NullLogger<GoCardlessDataProviderService>.Instance);
    }

    private static string CreateUnsignedToken(DateTimeOffset expires)
    {
        static string Encode(string json) =>
            Convert.ToBase64String(Encoding.UTF8.GetBytes(json)).TrimEnd('=').Replace('+', '-').Replace('/', '_');

        return $"{Encode("""{"alg":"none","typ":"JWT"}""")}.{Encode($$"""{"exp":{{expires.ToUnixTimeSeconds()}}}""")}.";
    }

    private class StubHandler(HttpStatusCode statusCode, string body) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken) =>
            Task.FromResult(new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(body, Encoding.UTF8, "application/json")
            });
    }
}