namespace RobinTTY.PersonalFinanceDashboard.Core.Models;

/// <summary>
/// An account at a banking institution.
/// </summary>
public class BankAccount : Account
{
    /// <summary>
    /// The IBAN of the bank account.
    /// </summary>
    public string? Iban { get; set; }
    /// <summary>
    /// The BIC (Business Identifier Code) associated with the account.
    /// </summary>
    public string? Bic { get; set; }
    /// <summary>
    /// Basic Bank Account Number represents a country-specific bank account number.
    /// This data element is used for payment accounts which have no IBAN.
    /// </summary>
    public string? Bban { get; set; }
    /// <summary>
    /// Name of the legal account owner. If there is more than one owner, then two names might be noted here.
    /// </summary>
    public string? OwnerName { get; set; }
    /// <summary>
    /// Specifies the nature, or use, of the account.
    /// </summary>
    public string? AccountType { get; set; }
    /// <summary>
    /// The banking institution this account belongs to.
    /// </summary>
    public BankingInstitution? AssociatedInstitution { get; set; }

    /// <summary>
    /// Creates a new instance of <see cref="BankAccount"/>.
    /// </summary>
    /// <param name="id">The id of the account.</param>
    /// <param name="name">The name of the account.</param>
    /// <param name="description">A description of this account.</param>
    /// <param name="balance">The current balance of the account.</param>
    /// <param name="currency">The currency this account is denominated in.</param>
    /// <param name="transactions">Transactions that are associated with this account.</param>
    /// <param name="iban">The IBAN of the bank account.</param>
    /// <param name="bic">The BIC (Business Identifier Code) associated with the account.</param>
    /// <param name="bban">Basic Bank Account Number represents a country-specific bank account number.
    /// This data element is used for payment accounts which have no IBAN.</param>
    /// <param name="ownerName">Name of the legal account owner. If there is more than one owner,
    /// then two names might be noted here.</param>
    /// <param name="accountType">Specifies the nature, or use, of the account.</param>
    /// <param name="associatedInstitution">The banking institution this account belongs to.</param>
    public BankAccount(Guid id, string? name, string? description, decimal? balance, string? currency, List<Transaction> transactions,
        string? iban, string? bic, string? bban, string? ownerName, string? accountType, BankingInstitution? associatedInstitution)
        : base(id, name, description, balance, currency, transactions)
    {
        Iban = iban;
        Bic = bic;
        Bban = bban;
        OwnerName = ownerName;
        AccountType = accountType;
        AssociatedInstitution = associatedInstitution;
    }
}
