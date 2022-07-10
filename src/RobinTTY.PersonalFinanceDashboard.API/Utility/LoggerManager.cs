namespace RobinTTY.PersonalFinanceDashboard.API.Utility;

/// <summary>
/// Manages the global logger instance.
/// </summary>
public class LoggerManager
{
    /// <summary>
    /// Sets up the logger and returns it.
    /// </summary>
    /// <returns>Instance of <see cref="Serilog.Core.Logger"/>.</returns>
    public static ILogger GetDefaultLogger()
    {
        var logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        Log.Logger = logger;
        return logger;
    }
}