namespace RobinTTY.PersonalFinanceDashboard.Core.Models;

/// <summary>
/// Where data originates from.
/// </summary>
public enum DataSource
{
    /// <summary>
    /// Created by the user. Manual data isn't synchronized with any external service.
    /// </summary>
    Manual,

    /// <summary>
    /// Retrieved from <see href="https://gocardless.com/bank-account-data/">GoCardless Bank Account Data</see>.
    /// </summary>
    GoCardless
}