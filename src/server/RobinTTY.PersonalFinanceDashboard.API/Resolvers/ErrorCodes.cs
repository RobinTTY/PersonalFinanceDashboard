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
    /// The bank account can't be deleted while it's synchronized, as the synchronization would recreate it.
    /// </summary>
    public const string BankAccountSynchronized = "BANK_ACCOUNT_SYNCHRONIZED";

    /// <summary>
    /// The requested banking institution doesn't exist.
    /// </summary>
    public const string BankingInstitutionNotFound = "BANKING_INSTITUTION_NOT_FOUND";

    /// <summary>
    /// The banking institution can't be deleted, as it's synchronized from a data source.
    /// </summary>
    public const string BankingInstitutionSynchronized = "BANKING_INSTITUTION_SYNCHRONIZED";

    /// <summary>
    /// The requested transaction doesn't exist.
    /// </summary>
    public const string TransactionNotFound = "TRANSACTION_NOT_FOUND";

    /// <summary>
    /// The transaction can't be deleted while its account is synchronized, as the synchronization would recreate it.
    /// </summary>
    public const string TransactionSynchronized = "TRANSACTION_SYNCHRONIZED";

    /// <summary>
    /// The provided redirect URI isn't a valid absolute URI.
    /// </summary>
    public const string InvalidRedirectUri = "INVALID_REDIRECT_URI";

    /// <summary>
    /// The institution can't be connected to a bank data provider, e.g. because it was created manually.
    /// </summary>
    public const string InstitutionNotConnectable = "INSTITUTION_NOT_CONNECTABLE";

    /// <summary>
    /// The authentication request couldn't be created at the third party.
    /// </summary>
    public const string CreateAuthenticationRequestFailed = "CREATE_AUTHENTICATION_REQUEST_FAILED";

    /// <summary>
    /// The authentication request couldn't be deleted at the third party.
    /// </summary>
    public const string DeleteAuthenticationRequestFailed = "DELETE_AUTHENTICATION_REQUEST_FAILED";
}
