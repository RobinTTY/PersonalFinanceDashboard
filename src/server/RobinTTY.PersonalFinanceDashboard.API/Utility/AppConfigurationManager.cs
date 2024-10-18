using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RobinTTY.PersonalFinanceDashboard.Api.Utility;

namespace RobinTTY.PersonalFinanceDashboard.API.Utility;

/// <summary>
/// Manages the configuration of the application.
/// </summary>
public static class AppConfigurationManager
{
    /// <summary>
    /// Creates a new instance of <see cref="AppConfiguration"/>.
    /// </summary>
    public static AppConfiguration AppConfiguration
    {
        get
        {
            var serviceProvider = GetServiceProvider();
            return serviceProvider.GetRequiredService<IOptions<AppConfiguration>>().Value;
        }
    }

    /// <summary>
    /// Creates a <see cref="ServiceProvider"/> from the provided configuration files.
    /// </summary>
    /// <returns>The created <see cref="IServiceProvider"/>.</returns>
    private static IServiceProvider GetServiceProvider()
    {
        // TODO: Add environment variables for configuration through Docker?
        var configuration =  new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", false, true)
            .AddUserSecrets<AppConfiguration>(false, true)
            .Build();

        return new ServiceCollection()
            .Configure<AppConfiguration>(configuration.GetSection(nameof(AppConfiguration)))
            .AddOptions()
            .BuildServiceProvider();
    }
}
