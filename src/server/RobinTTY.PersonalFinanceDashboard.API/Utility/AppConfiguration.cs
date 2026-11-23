using System.ComponentModel.DataAnnotations;

namespace RobinTTY.PersonalFinanceDashboard.Api.Utility;

/// <summary>
/// General configuration of the application server.
/// </summary>
public class AppConfiguration
{
    /// <summary>
    /// The configuration section the settings are read from.
    /// </summary>
    public const string SectionName = "AppConfiguration";

    /// <summary>
    /// The configuration section the GoCardless credentials are read from.
    /// </summary>
    public const string GoCardlessSectionName = $"{SectionName}:GoCardlessConfiguration";

    /// <summary>
    /// The configuration section the data retrieval intervals are read from.
    /// </summary>
    public const string DataRetrievalSectionName = $"{SectionName}:DataRetrievalConfiguration";

    /// <summary>
    /// The origins allowed to call the API from a browser, e.g. "http://localhost:5173".
    /// If not set, only the Vite dev server is allowed.
    /// </summary>
    public string[]? AllowedCorsOrigins { get; set; }
}

/// <summary>
/// Configuration parameters for the database.
/// </summary>
public class DatabaseConfiguration
{
    /// <summary>
    /// The configuration section the settings are read from.
    /// </summary>
    public const string SectionName = $"{AppConfiguration.SectionName}:DatabaseConfiguration";

    /// <summary>
    /// The connection string of the database.
    /// </summary>
    [Required]
    public string ConnectionString { get; set; } = string.Empty;
}

/// <summary>
/// Configuration parameters for logging.
/// </summary>
public class LoggingConfiguration
{
    /// <summary>
    /// The configuration section the settings are read from.
    /// </summary>
    public const string SectionName = $"{AppConfiguration.SectionName}:LoggingConfiguration";

    /// <summary>
    /// The URL of the Seq server logs are sent to. If not set, logs aren't sent to Seq.
    /// </summary>
    public string? SeqServerUrl { get; set; }

    /// <summary>
    /// The file logs are written to while Seq isn't reachable, relative to the content root.
    /// The date is appended to the file name.
    /// </summary>
    public string SeqFallbackPath { get; set; } = "logs/log-.clef";
}
