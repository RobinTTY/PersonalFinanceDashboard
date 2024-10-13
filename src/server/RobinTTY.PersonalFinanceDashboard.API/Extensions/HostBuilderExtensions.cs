using Microsoft.Extensions.Hosting;

namespace RobinTTY.PersonalFinanceDashboard.Api.Extensions;

/// <summary>
/// Provides extensions on the <see cref="IHostBuilder"/> type.
/// </summary>
public static class HostBuilderExtensions
{
    /// <summary>
    /// Configures the logging of the application via Serilog.
    /// </summary>
    /// <param name="hostBuilder">The host to configure the logging for.</param>
    /// <returns>A reference to the <see cref="IHostBuilder"/> after the operation has completed.</returns>
    public static IHostBuilder ConfigureSerilog(this IHostBuilder hostBuilder)
    {
        // TODO: Only do in development
        // Forward issues with Serilog itself to console
        Serilog.Debugging.SelfLog.Enable(Console.Error);
        
        return hostBuilder.UseSerilog((context, loggerConfig) =>
        {
            loggerConfig.ReadFrom.Configuration(context.Configuration);
        });
    }
}
