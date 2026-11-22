using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using RobinTTY.PersonalFinanceDashboard.Api.Utility;
using Serilog.Debugging;
using Path = System.IO.Path;

namespace RobinTTY.PersonalFinanceDashboard.Api.Extensions;

/// <summary>
/// Provides extensions on the <see cref="IHostBuilder"/> type.
/// </summary>
public static class WebApplicationBuilderExtensions
{
    /// <summary>
    /// Configures the logging of the application via Serilog.
    /// </summary>
    /// <param name="webApplicationBuilder">The host to configure the logging for.</param>
    /// <returns>A reference to the <see cref="IHostBuilder"/> after the operation has completed.</returns>
    public static IHostBuilder ConfigureSerilog(this WebApplicationBuilder webApplicationBuilder)
    {
        // Forward issues with Serilog itself to console
        if (webApplicationBuilder.Environment.IsDevelopment())
        {
            SelfLog.Enable(Console.Error);
        }

        return webApplicationBuilder.Host.UseSerilog((context, loggerConfig) =>
        {
            loggerConfig.ReadFrom.Configuration(context.Configuration);

            var logging = context.Configuration.GetSection(LoggingConfiguration.SectionName)
                .Get<LoggingConfiguration>();
            if (logging?.SeqServerUrl is { } seqServerUrl)
            {
                var fallbackPath = Path.Combine(context.HostingEnvironment.ContentRootPath, logging.SeqFallbackPath);
                loggerConfig.WriteTo.SeqWithFileFallback(seqServerUrl, fallbackPath);
            }
        });
    }
}