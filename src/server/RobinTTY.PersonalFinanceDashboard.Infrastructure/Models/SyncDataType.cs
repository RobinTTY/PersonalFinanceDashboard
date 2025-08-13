namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Models;

/// <summary>
/// Represents the different types of data that can be synchronized from the third party data source.
/// </summary>
public enum SyncDataType
{
    BankingInstitutions,
    BankAccounts,
    Transactions,
    AuthenticationRequests
}