using System.Collections.Concurrent;
using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using RobinTTY.NordigenApiClient;

namespace RobinTTY.PersonalFinanceDashboard.ThirdPartyDataProviders.Tests;

/// <summary>
/// Tests the HTTP client registered by <c>AddGoCardless</c>, with the network replaced by
/// <see cref="ScriptedHandler"/>.
/// </summary>
public sealed class GoCardlessHttpClientTests : IDisposable
{
    private const string Url = "https://bankaccountdata.gocardless.com/api/v2/institutions/";

    private readonly ScriptedHandler _network = new();
    private readonly ServiceProvider _services;

    public GoCardlessHttpClientTests()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["GoCardless:SecretId"] = "secret-id",
                ["GoCardless:SecretKey"] = "secret-key"
            })
            .Build();
        var services = new ServiceCollection()
            .AddSingleton<IConfiguration>(configuration)
            .AddSingleton(typeof(ILogger<>), typeof(NullLogger<>))
            .AddGoCardless("GoCardless");
        services.AddHttpClient(GoCardlessServiceCollectionExtensions.HttpClientName)
            .ConfigurePrimaryHttpMessageHandler(() => _network);
        _services = services.BuildServiceProvider();
    }

    public void Dispose() => _services.Dispose();

    [Fact]
    public async Task TemporaryServerError_IsRetried()
    {
        _network.Respond(HttpStatusCode.ServiceUnavailable, HttpStatusCode.OK);

        var response = await CreateClient().GetAsync(Url, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(2, _network.RequestCount);
    }

    [Fact]
    public async Task RateLimitedRequest_IsNotRetried()
    {
        // The rate limit resets after hours, the synchronization schedules the request again itself
        _network.Respond(HttpStatusCode.TooManyRequests);

        var response = await CreateClient().GetAsync(Url, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);
        Assert.Equal(1, _network.RequestCount);
    }

    [Fact]
    public async Task FailedPost_IsNotRetried()
    {
        // E.g. creating an authentication request, which could otherwise be created twice
        _network.Respond(HttpStatusCode.ServiceUnavailable);

        var response = await CreateClient().PostAsync(Url, null, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
        Assert.Equal(1, _network.RequestCount);
    }

    [Fact]
    public async Task RequestWithoutResponse_IsReportedAsAFailedRequest()
    {
        // Instead of an exception that would abort the synchronization
        _network.FailWithoutResponse();

        var response = await _services.GetRequiredService<GoCardlessBankDataProvider>()
            .GetBankingInstitutions(cancellationToken: TestContext.Current.CancellationToken);

        Assert.False(response.IsSuccessful);
        Assert.Contains("No response from GoCardless", response.Error.Message);
    }

    [Fact]
    public void NordigenClient_IsShared()
    {
        // It keeps the access tokens, which would otherwise be requested again
        using var first = _services.CreateScope();
        using var second = _services.CreateScope();

        Assert.Same(first.ServiceProvider.GetRequiredService<NordigenClient>(),
            second.ServiceProvider.GetRequiredService<NordigenClient>());
    }

    private HttpClient CreateClient() => _services.GetRequiredService<IHttpClientFactory>()
        .CreateClient(GoCardlessServiceCollectionExtensions.HttpClientName);

    /// <summary>
    /// Answers requests with the configured status codes in order, repeating the last one.
    /// </summary>
    private sealed class ScriptedHandler : HttpMessageHandler
    {
        private readonly ConcurrentQueue<HttpStatusCode> _statusCodes = new();
        private HttpStatusCode _lastStatusCode = HttpStatusCode.OK;
        private bool _failWithoutResponse;
        private int _requestCount;

        public int RequestCount => _requestCount;

        public void Respond(params HttpStatusCode[] statusCodes)
        {
            foreach (var statusCode in statusCodes)
                _statusCodes.Enqueue(statusCode);
        }

        public void FailWithoutResponse() => _failWithoutResponse = true;

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            Interlocked.Increment(ref _requestCount);
            if (_failWithoutResponse)
                throw new HttpRequestException("No connection could be made");

            if (_statusCodes.TryDequeue(out var statusCode))
                _lastStatusCode = statusCode;
            return Task.FromResult(new HttpResponseMessage(_lastStatusCode)
            {
                Content = new StringContent("{}"),
                RequestMessage = request
            });
        }
    }
}
