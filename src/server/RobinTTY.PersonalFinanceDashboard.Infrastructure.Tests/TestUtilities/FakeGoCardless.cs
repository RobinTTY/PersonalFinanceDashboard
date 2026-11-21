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
    private sealed record FakeResponse(
        HttpStatusCode StatusCode,
        string Json,
        IReadOnlyDictionary<string, string>? Headers = null);

    private static readonly FakeResponse NotFound = new(HttpStatusCode.NotFound,
        """{"summary":"Not found","detail":"No fake response configured","status_code":404}""");

    private readonly ConcurrentDictionary<string, FakeResponse> _responses = new();
    private readonly ConcurrentQueue<string> _requestedPaths = new();
    private TaskCompletionSource? _heldResponses;

    /// <summary>
    /// Sets the response for requests whose path ends with <paramref name="path"/>, e.g. "requisitions/".
    /// </summary>
    public void Respond(string path, string json) => _responses[path] = new FakeResponse(HttpStatusCode.OK, json);

    /// <summary>
    /// Answers requests whose path ends with <paramref name="path"/> with a 429, as if the rate limit of the
    /// accounts endpoint was exceeded.
    /// </summary>
    /// <param name="path">The path to answer.</param>
    /// <param name="resetAfter">The time until the rate limit resets.</param>
    public void RespondRateLimited(string path, TimeSpan resetAfter) =>
        _responses[path] = new FakeResponse(HttpStatusCode.TooManyRequests,
            """{"summary":"Rate limit exceeded","detail":"Too many requests","status_code":429}""",
            new Dictionary<string, string>
            {
                ["HTTP_X_RATELIMIT_ACCOUNT_SUCCESS_RESET"] = ((int)resetAfter.TotalSeconds).ToString()
            });

    /// <summary>
    /// Removes the response for <paramref name="path"/>, so requests to it get a 404 again.
    /// </summary>
    public void Remove(string path) => _responses.TryRemove(path, out _);

    /// <summary>
    /// Holds back all responses until the returned action is called, so requests stay in flight.
    /// </summary>
    public Action HoldResponses()
    {
        var heldResponses = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        _heldResponses = heldResponses;
        return () =>
        {
            _heldResponses = null;
            heldResponses.SetResult();
        };
    }

    public int RequestCount(string path) => _requestedPaths.Count(requested => requested.EndsWith(path));

    public int TotalRequestCount => _requestedPaths.Count;

    /// <summary>
    /// The body of the last request that had one.
    /// </summary>
    public string? LastRequestBody { get; private set; }

    public GoCardlessBankDataProvider CreateService()
    {
        // A valid token pair keeps the client from requesting a new token
        var token = CreateUnsignedToken(DateTimeOffset.UtcNow.AddDays(1));
        var client = new NordigenClient(new HttpClient(this, disposeHandler: false),
            new NordigenClientCredentials("secret-id", "secret-key"), new JsonWebTokenPair(token, token));

        return new GoCardlessBankDataProvider(client, NullLogger<GoCardlessBankDataProvider>.Instance);
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var path = request.RequestUri!.AbsolutePath;
        _requestedPaths.Enqueue(path);
        if (request.Content != null)
            LastRequestBody = await request.Content.ReadAsStringAsync(cancellationToken);
        if (_heldResponses is { } heldResponses)
            await heldResponses.Task.WaitAsync(cancellationToken);

        var response = _responses.FirstOrDefault(response => path.EndsWith(response.Key)).Value ?? NotFound;
        return CreateResponse(response);
    }

    private static HttpResponseMessage CreateResponse(FakeResponse response)
    {
        var message = new HttpResponseMessage(response.StatusCode)
        {
            Content = new StringContent(response.Json, Encoding.UTF8, "application/json")
        };
        foreach (var (name, value) in response.Headers ?? new Dictionary<string, string>())
            message.Headers.Add(name, value);

        return message;
    }

    private static string CreateUnsignedToken(DateTimeOffset expires)
    {
        static string Encode(string json) =>
            Convert.ToBase64String(Encoding.UTF8.GetBytes(json)).TrimEnd('=').Replace('+', '-').Replace('/', '_');

        return $"{Encode("""{"alg":"none","typ":"JWT"}""")}.{Encode($$"""{"exp":{{expires.ToUnixTimeSeconds()}}}""")}.";
    }
}