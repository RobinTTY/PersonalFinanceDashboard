using AutofacSerilogIntegration;
using RobinTTY.PersonalFinanceDashboard.API.Models;
using RobinTTY.PersonalFinanceDashboard.ThirdPartyDataProviders;
using System.Net.Http;
using RobinTTY.NordigenApiClient.Models;

namespace RobinTTY.PersonalFinanceDashboard.API.Utility;

/// <summary>
/// Container managing dependency resolution.
/// </summary>
public class ApplicationModule : Module
{
    /// <summary>
    /// Adds registrations to the dependency container.
    /// </summary>
    /// <param name="builder">The passed down <see cref="ContainerBuilder"/>.</param>
    protected override void Load(ContainerBuilder builder)
    {
        var appConfig = AppConfigurationManager.AppConfiguration;

        builder.RegisterLogger(LoggerManager.GetDefaultLogger());
        builder.Register(c => c.Resolve<IHttpClientFactory>().CreateClient());
        builder.Register(_ => new NordigenClientCredentials(appConfig.NordigenApi.SecretId, appConfig.NordigenApi.SecretKey));
        builder.RegisterType<GoCardlessDataProvider>().SingleInstance();
        builder.RegisterType<Query>().SingleInstance();
    }
}