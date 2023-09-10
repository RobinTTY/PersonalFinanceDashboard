using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RobinTTY.PersonalFinanceDashboard.API.Models;

namespace RobinTTY.PersonalFinanceDashboard.API.Utility;

public static class AppConfigurationManager
{
    public static AppConfiguration AppConfiguration
    {
        get
        {
            var serviceProvider = GetServiceProvider();
            return serviceProvider.GetRequiredService<IOptions<AppConfiguration>>().Value;
        }
    }

    private static IServiceProvider GetServiceProvider()
    {
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
