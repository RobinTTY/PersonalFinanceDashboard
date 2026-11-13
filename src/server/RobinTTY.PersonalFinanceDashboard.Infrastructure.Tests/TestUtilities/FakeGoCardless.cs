using System.Collections.Concurrent;
using System.Net;
using System.Text;
using Microsoft.Extensions.Logging.Abstractions;
using RobinTTY.NordigenApiClient;
using RobinTTY.NordigenApiClient.Models;
using RobinTTY.NordigenApiClient.Models.Jwt;
using RobinTTY.PersonalFinanceDashboard.ThirdPartyDataProviders;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Tests.TestUtilities;

/// <summary>
/// Answers GoCardless API requests with configured responses and records which endpoints were called.
/// Requests without a configured response get a 404.
/// </summary>
internal sealed class FakeGoCardless : HttpMessageHandler
{
    private readonly ConcurrentDictionary<string, string> _responses = new();
    private readonly ConcurrentQueue<string> _requestedPaths = new();

    /// <summary>
    /// Sets the response for requests whose path ends with <paramref name="path"/>, e.g. "requisitions/".
    /// </summary>
    public void Respond(string path, string json) => _responses[path] = json;

    public int RequestCount(string path) => _requestedPaths.Count(requested => requested.EndsWith(path));

    public int TotalRequestCount => _requestedPaths.Count;

    public GoCardlessDataProviderService CreateService()
    {
        // A valid token pair keeps the client from requesting a new token
        var token = CreateUnsignedToken(DateTimeOffset.UtcNow.AddDays(1));
        var client = new NordigenClient(new HttpClient(this, disposeHandler: false),
            new NordigenClientCredentials("secret-id", "secret-key"), new JsonWebTokenPair(token, token));

        return new GoCardlessDataProviderService(client, NullLogger<GoCardlessDataProviderService>.Instance);
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var path = request.RequestUri!.AbsolutePath;
        _requestedPaths.Enqueue(path);

        var json = _responses.FirstOrDefault(response => path.EndsWith(response.Key)).Value;
        return Task.FromResult(json != null
            ? CreateResponse(HttpStatusCode.OK, json)
            : CreateResponse(HttpStatusCode.NotFound,
                """{"summary":"Not found","detail":"No fake response configured","status_code":404}"""));
    }

    private static HttpResponseMessage CreateResponse(HttpStatusCode statusCode, string json) =>
        new(statusCode) { Content = new StringContent(json, Encoding.UTF8, "application/json") };

    private static string CreateUnsignedToken(DateTimeOffset expires)
    {
        static string Encode(string json) =>
            Convert.ToBase64String(Encoding.UTF8.GetBytes(json)).TrimEnd('=').Replace('+', '-').Replace('/', '_');

        return $"{Encode("""{"alg":"none","typ":"JWT"}""")}.{Encode($$"""{"exp":{{expires.ToUnixTimeSeconds()}}}""")}.";
    }
}