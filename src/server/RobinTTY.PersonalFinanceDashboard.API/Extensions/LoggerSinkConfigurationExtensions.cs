using System.Globalization;
using Serilog.Configuration;
using Serilog.Formatting.Compact;

namespace RobinTTY.PersonalFinanceDashboard.Api.Extensions;

/// <summary>
/// Provides extensions on the <see cref="LoggerSinkConfiguration"/> type.
/// </summary>
public static class LoggerSinkConfigurationExtensions
{
    private static readonly TimeSpan ReachabilityTimeout = TimeSpan.FromSeconds(1);

    private static readonly HttpClient Client = new()
    {
        Timeout = ReachabilityTimeout
    };

    /// <summary>
    /// Writes log events to Seq, or to a daily file in the compact JSON format if Seq isn't reachable on startup.
    /// </summary>
    /// <remarks>
    /// If Seq becomes unreachable later on, events are only written to the file once Seq stops retrying them,
    /// and may be lost if the application stops before that.
    /// </remarks>
    /// <param name="writeTo">The sink configuration to add the sinks to.</param>
    /// <param name="serverUrl">The URL of the Seq server.</param>
    /// <param name="fallbackPath">The path of the file, to which the date is appended.</param>
    /// <returns>Configuration object allowing method chaining.</returns>
    public static LoggerConfiguration SeqWithFileFallback(this LoggerSinkConfiguration writeTo, string serverUrl,
        string fallbackPath)
    {
        void File(LoggerSinkConfiguration sink) =>
            sink.File(new CompactJsonFormatter(), fallbackPath, rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7);

        if (IsReachable(serverUrl))
        {
            return writeTo.FallbackChain(sink => sink.Seq(serverUrl, formatProvider: CultureInfo.InvariantCulture),
                File);
        }

        // The logger doesn't exist yet
        Console.Error.WriteLine($"Seq isn't reachable at {serverUrl}, logging to {fallbackPath} instead.");
        return writeTo.Logger(logger => File(logger.WriteTo));
    }

    private static bool IsReachable(string serverUrl)
    {
        try
        {
            using var response = Client.Send(new HttpRequestMessage(HttpMethod.Get, serverUrl));
            return true;
        }
        catch (Exception exception) when (exception is HttpRequestException or TaskCanceledException)
        {
            return false;
        }
    }
}
