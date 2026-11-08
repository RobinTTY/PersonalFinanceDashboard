namespace RobinTTY.PersonalFinanceDashboard.Api.Resolvers.Inputs;

/// <summary>
/// Payload required to create a bank account through the API.
/// </summary>
public record CreateBankAccountInput
{
    /// <summary>
    /// The name of the account.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// A description of this account.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// The current balance of the account.
    /// </summary>
    public decimal? Balance { get; init; }

    /// <summary>
    /// The currency this account is denominated in.
    /// </summary>
    public string? Currency { get; init; }

    /// <summary>
    /// The IBAN of the bank account.
    /// </summary>
    public string? Iban { get; init; }

    /// <summary>
    /// The BIC (Business Identifier Code) associated with the account.
    /// </summary>
    public string? Bic { get; init; }

    /// <summary>
    /// Basic Bank Account Number represents a country-specific bank account number.
    /// This data element is used for payment accounts which have no IBAN.
    /// </summary>
    public string? Bban { get; init; }

    /// <summary>
    /// Name of the legal account owner. If there is more than one owner, then two names might be noted here.
    /// </summary>
    public string? OwnerName { get; init; }

    /// <summary>
    /// Specifies the nature, or use, of the account.
    /// </summary>
    public string? AccountType { get; init; }
}