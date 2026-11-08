namespace RobinTTY.PersonalFinanceDashboard.Api.Utility;

/// <summary>
/// The configuration of the application server.
/// </summary>
public class AppConfiguration
{
    /// <summary>
    /// Configuration parameters for the Nordigen API client.
    /// </summary>
    public required NordigenApiClientConfiguration NordigenApiConfiguration { get; set; }

    /// <summary>
    /// Configuration parameters for the database.
    /// </summary>
    public required DatabaseConfiguration DatabaseConfiguration { get; set; }

    /// <summary>
    /// The origins allowed to call the API from a browser, e.g. "http://localhost:5173".
    /// If not set, only the Vite dev server is allowed.
    /// </summary>
    public string[]? AllowedCorsOrigins { get; set; }

    /// <summary>
    /// Creates a new instance of <see cref="AppConfiguration"/>.
    /// Necessary for 
    /// </summary>
    public AppConfiguration()
    {
    }
}

/// <summary>
/// Configuration parameters for the Nordigen API client.
/// </summary>
public class NordigenApiClientConfiguration
{
    /// <summary>
    /// The secret id to access the Nordigen API.
    /// </summary>
    public string SecretId { get; set; }

    /// <summary>
    /// The secret key to access the Nordigen API.
    /// </summary>
    public string SecretKey { get; set; }

    /// <summary>
    /// Creates a new instance of <see cref="NordigenApiClientConfiguration"/>.
    /// </summary>
    /// <param name="secretId">The secret id to access the Nordigen API.</param>
    /// <param name="secretKey">The secret key to access the Nordigen API.</param>
    public NordigenApiClientConfiguration(string secretId, string secretKey)
    {
        SecretId = secretId;
        SecretKey = secretKey;
    }
}

/// <summary>
/// Configuration parameters for the database.
/// </summary>
public class DatabaseConfiguration
{
    /// <summary>
    /// The connection string of the database.
    /// </summary>
    public string ConnectionString { get; set; }

    /// <summary>
    /// Creates a new instance of <see cref="DatabaseConfiguration"/>.
    /// </summary>
    /// <param name="connectionString">The connection string of the database.</param>
    public DatabaseConfiguration(string connectionString)
    {
        ConnectionString = connectionString;
    }
}