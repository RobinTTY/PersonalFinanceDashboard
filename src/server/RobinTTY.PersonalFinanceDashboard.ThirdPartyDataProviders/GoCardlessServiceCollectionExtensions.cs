using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Options;
using Polly;
using RobinTTY.NordigenApiClient;
using RobinTTY.NordigenApiClient.Models;
using RobinTTY.PersonalFinanceDashboard.Core.DataProviders;

namespace RobinTTY.PersonalFinanceDashboard.ThirdPartyDataProviders;

/// <summary>
/// Provides extensions to register GoCardless in an <see cref="IServiceCollection"/>.
/// </summary>
public static class GoCardlessServiceCollectionExtensions
{
    /// <summary>
    /// The name of the <see cref="HttpClient"/> that requests the GoCardless API.
    /// </summary>
    public const string HttpClientName = "GoCardless";

    /// <param name="services">The service collection to which to add the services.</param>
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Registers GoCardless as an <see cref="IBankDataProvider"/>. Missing credentials stop the application
        /// on startup.
        /// </summary>
        /// <param name="configSectionPath">The configuration section the <see cref="GoCardlessConfiguration"/>
        /// is read from.</param>
        /// <returns>A reference to the <see cref="IServiceCollection"/> after the operation has completed.</returns>
        public IServiceCollection AddGoCardless(string configSectionPath)
        {
            services.AddOptions<GoCardlessConfiguration>()
                .BindConfiguration(configSectionPath)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddTransient<NoResponseHandler>();
            services.AddHttpClient(HttpClientName)
                // The client lives as long as the NordigenClient, which keeps the access tokens. Its connections are
                // renewed instead of its handler, so it still picks up DNS changes.
                .ConfigurePrimaryHttpMessageHandler(() =>
                    new SocketsHttpHandler { PooledConnectionLifetime = TimeSpan.FromMinutes(15) })
                .SetHandlerLifetime(Timeout.InfiniteTimeSpan)
                // The resilience handler enforces the timeouts
                .ConfigureHttpClient(client => client.Timeout = Timeout.InfiniteTimeSpan)
                .AddHttpMessageHandler<NoResponseHandler>()
                .AddResilienceHandler(HttpClientName, ConfigureResilience);

            return services
                .AddSingleton(provider =>
                {
                    var config = provider.GetRequiredService<IOptions<GoCardlessConfiguration>>().Value;
                    return new NordigenClientCredentials(config.SecretId, config.SecretKey);
                })
                .AddSingleton(provider => new NordigenClient(
                    provider.GetRequiredService<IHttpClientFactory>().CreateClient(HttpClientName),
                    provider.GetRequiredService<NordigenClientCredentials>()))
                .AddSingleton<GoCardlessBankDataProvider>()
                // Resolved through the concrete type, so it can be replaced (e.g. in tests) by registering that type
                .AddSingleton<IBankDataProvider>(provider =>
                    provider.GetRequiredService<GoCardlessBankDataProvider>());
        }
    }

    /// <summary>
    /// Retries requests that failed temporarily, and limits how long a request may take.
    /// </summary>
    private static void ConfigureResilience(ResiliencePipelineBuilder<HttpResponseMessage> pipeline)
    {
        // Retrieving transactions can take a while, as GoCardless requests them from the bank
        pipeline.AddTimeout(TimeSpan.FromMinutes(3));

        var retry = new HttpRetryStrategyOptions
        {
            MaxRetryAttempts = 3,
            BackoffType = DelayBackoffType.Exponential,
            UseJitter = true,
            Delay = TimeSpan.FromSeconds(1),
            // Rate limits reset after hours, the synchronization schedules those requests again itself
            ShouldHandle = arguments => ValueTask.FromResult(
                arguments.Outcome.Result?.StatusCode != HttpStatusCode.TooManyRequests &&
                HttpClientResiliencePredicates.IsTransient(arguments.Outcome))
        };
        // E.g. creating an authentication request twice would leave an unused one at GoCardless
        retry.DisableForUnsafeHttpMethods();
        pipeline.AddRetry(retry);

        pipeline.AddTimeout(TimeSpan.FromMinutes(1));
    }
}
