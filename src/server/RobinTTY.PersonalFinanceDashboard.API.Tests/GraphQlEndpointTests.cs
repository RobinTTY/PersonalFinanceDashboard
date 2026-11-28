using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using RobinTTY.PersonalFinanceDashboard.API.Tests.TestUtilities;

namespace RobinTTY.PersonalFinanceDashboard.API.Tests;

/// <summary>
/// The tests run outside of Development, like a deployed API.
/// </summary>
public class GraphQlEndpointTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Introspection_IsRejected()
    {
        var response = await _client.PostGraphQl("{ __schema { queryType { name } } }");

        Assert.True(response.TryGetProperty("errors", out _));
        Assert.False(response.TryGetProperty("data", out var data) && data.ValueKind != JsonValueKind.Null);
    }

    [Fact]
    public async Task TypeNames_CanStillBeQueried()
    {
        // Apollo Client adds them to every query
        var response = await _client.PostGraphQl("{ __typename }");

        Assert.Equal("Query", response.Data("__typename").GetString());
    }

    [Fact]
    public async Task Ide_IsNotServed()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/graphql");
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));

        var response = await _client.SendAsync(request, TestContext.Current.CancellationToken);

        Assert.NotEqual("text/html", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Schema_IsNotServed()
    {
        var response = await _client.GetAsync("/graphql?sdl", TestContext.Current.CancellationToken);

        Assert.False(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task RequestForAnotherHost_IsRejected()
    {
        // E.g. a website whose domain was pointed at this machine (DNS rebinding)
        var request = new HttpRequestMessage(HttpMethod.Get, "/health");
        request.Headers.Host = "attacker.example";

        var response = await _client.SendAsync(request, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
