using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RobinTTY.NordigenApiClient;
using RobinTTY.NordigenApiClient.Models;
using RobinTTY.PersonalFinanceDashboard.Core.DataProviders;

namespace RobinTTY.PersonalFinanceDashboard.ThirdPartyDataProviders;

/// <summary>
/// Provides extensions to register GoCardless in an <see cref="IServiceCollection"/>.
/// </summary>
public static class GoCardlessServiceCollectionExtensions
{
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

            services.AddHttpClient();

            return services
                .AddSingleton(provider =>
                {
                    var config = provider.GetRequiredService<IOptions<GoCardlessConfiguration>>().Value;
                    return new NordigenClientCredentials(config.SecretId, config.SecretKey);
                })
                .AddSingleton<NordigenClient>()
                .AddSingleton<GoCardlessBankDataProvider>()
                // Resolved through the concrete type, so it can be replaced (e.g. in tests) by registering that type
                .AddSingleton<IBankDataProvider>(provider =>
                    provider.GetRequiredService<GoCardlessBankDataProvider>());
        }
    }
}