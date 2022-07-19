using Microsoft.Extensions.Configuration;
using RobinTTY.PersonalFinanceDashboard.API.Models;

namespace RobinTTY.PersonalFinanceDashboard.API.Utility;

public class AppConfigurationManager
{
    public static IConfiguration GetApplicationConfiguration()
    {
        return new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", false, true)
            .AddUserSecrets<AppConfiguration>(false, true)
            .Build();
    }
}
