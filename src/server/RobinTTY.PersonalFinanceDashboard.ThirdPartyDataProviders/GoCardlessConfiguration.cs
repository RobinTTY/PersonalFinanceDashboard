using System.ComponentModel.DataAnnotations;

namespace RobinTTY.PersonalFinanceDashboard.ThirdPartyDataProviders;

/// <summary>
/// The credentials to access the GoCardless Bank Account Data API.
/// </summary>
public class GoCardlessConfiguration
{
    /// <summary>
    /// The secret id to access the API.
    /// </summary>
    [Required]
    public string SecretId { get; set; } = string.Empty;

    /// <summary>
    /// The secret key to access the API.
    /// </summary>
    [Required]
    public string SecretKey { get; set; } = string.Empty;
}
