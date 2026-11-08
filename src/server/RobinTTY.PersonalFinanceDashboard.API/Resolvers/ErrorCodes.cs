namespace RobinTTY.PersonalFinanceDashboard.Api.Resolvers;

/// <summary>
/// The error codes returned in the extensions of GraphQL errors.
/// </summary>
public static class ErrorCodes
{
    /// <summary>
    /// The requested bank account doesn't exist.
    /// </summary>
    public const string BankAccountNotFound = "BANK_ACCOUNT_NOT_FOUND";

    /// <summary>
    /// The requested banking institution doesn't exist.
    /// </summary>
    public const string BankingInstitutionNotFound = "BANKING_INSTITUTION_NOT_FOUND";

    /// <summary>
    /// The requested transaction doesn't exist.
    /// </summary>
    public const string TransactionNotFound = "TRANSACTION_NOT_FOUND";

    /// <summary>
    /// The authentication request couldn't be deleted at the third party.
    /// </summary>
    public const string DeleteAuthenticationRequestFailed = "DELETE_AUTHENTICATION_REQUEST_FAILED";
}