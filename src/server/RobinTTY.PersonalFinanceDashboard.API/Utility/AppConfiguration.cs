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