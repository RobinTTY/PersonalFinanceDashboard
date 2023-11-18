namespace RobinTTY.PersonalFinanceDashboard.Api.Types;

/// <summary>
/// The configuration of the application server.
/// </summary>
public class AppConfiguration
{
    /// <summary>
    /// Configuration parameters for the Nordigen API client.
    /// </summary>
    public NordigenApiClientConfiguration? NordigenApi { get; set; }

    /// <summary>
    /// Creates a new instance of <see cref="AppConfiguration"/>.
    /// </summary>
    public AppConfiguration() { }

    /// <summary>
    /// Creates a new instance of <see cref="AppConfiguration"/>.
    /// </summary>
    /// <param name="nordigenApi">Configuration parameters for the Nordigen API client.</param>
    public AppConfiguration(NordigenApiClientConfiguration nordigenApi)
    {
        NordigenApi = nordigenApi;
    }
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
    /// 
    /// </summary>
    /// <param name="url">The nordigen API endpoint to use.</param>
    /// <param name="secretId">The secret id to access the Nordigen API.</param>
    /// <param name="secretKey">The secret key to access the Nordigen API.</param>
    public NordigenApiClientConfiguration(string url, string secretId, string secretKey)
    {
        Url = url;
        SecretId = secretId;
        SecretKey = secretKey;
    }
}
