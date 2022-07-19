namespace RobinTTY.PersonalFinanceDashboard.API.Models;

/// <summary>
/// The configuration of the application server.
/// </summary>
public class AppConfiguration
{
    public NordigenApiClientConfiguration NordigenApi { get; set; }
}

/// <summary>
/// Configuration parameters for the Nordigen API client.
/// </summary>
public class NordigenApiClientConfiguration
{
    /// <summary>
    /// The nordigen API endpoint to use.
    /// </summary>
    public string Url { get; set; }
    /// <summary>
    /// The secret id to access the Nordigen API.
    /// </summary>
    public string SecretId { get; set; }
    /// <summary>
    /// The secret key to access the Nordigen API.
    /// </summary>
    public string SecretKey { get; set; }
    /// <summary>
    /// Unclear documentation on this parameter. Default value = 5.
    /// </summary>
    public int AccessTokenValidBeforeSeconds { get; set; }
    /// <summary>
    /// Unclear documentation on this parameter. Default value = 5.
    /// </summary>
    public int RefreshTokenValidBeforeSeconds { get; set; }
}
