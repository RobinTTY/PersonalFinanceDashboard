namespace RobinTTY.PersonalFinanceDashboard.Core.Models;

/// <summary>
/// The types of data retrieved from the data providers.
/// </summary>
public enum ThirdPartyDataType
{
    /// <summary>
    /// No data type, the default value.
    /// </summary>
    Undefined,

    /// <summary>
    /// The banking institutions whose accounts can be connected.
    /// </summary>
    BankingInstitutions,

    /// <summary>
    /// The requests to access the accounts of a banking institution.
    /// </summary>
    AuthenticationRequests,

    /// <summary>
    /// The details and balances of the connected bank accounts.
    /// </summary>
    BankAccounts,

    /// <summary>
    /// The transactions of the connected bank accounts.
    /// </summary>
    Transactions
}
