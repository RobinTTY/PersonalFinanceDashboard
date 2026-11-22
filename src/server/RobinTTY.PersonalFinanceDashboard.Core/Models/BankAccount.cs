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
    /// The name the data provider gives the account, e.g. its product name. Unlike <see cref="Account.Name"/>,
    /// which the user sets, this is updated by the synchronization.
    /// </summary>
    public string? ProviderName { get; set; }

    /// <summary>
    /// The description the data provider gives the account. Unlike <see cref="Account.Description"/>, which the
    /// user sets, this is updated by the synchronization.
    /// </summary>
    public string? ProviderDescription { get; set; }

    /// <summary>
    /// Whether this account is included in analytics such as graphs and account views.
    /// This is user-controlled and is not affected by third party data synchronization.
    /// </summary>
    public bool IncludeInAnalytics { get; set; } = true;

    /// <summary>
    /// The banking institution this account belongs to.
    /// </summary>
    public BankingInstitution? AssociatedInstitution { get; set; }

    /// <summary>
    /// Authentication requests made for this bank account.
    /// </summary>
    public List<AuthenticationRequest> AssociatedAuthenticationRequests { get; set; } = [];

    /// <summary>
    /// Creates a new instance of <see cref="BankAccount"/>.
    /// </summary>
    public BankAccount()
    {
        Id = Guid.Empty;
        Iban = null;
        Bic = null;
        Bban = null;
        OwnerName = null;
        AccountType = null;
        IncludeInAnalytics = true;
        AssociatedInstitution = null;
    }

    /// <summary>
    /// Creates a new instance of <see cref="BankAccount"/>.
    /// </summary>
    /// <param name="source">Where the account originates from.</param>
    /// <param name="externalId">The id the data source assigned to the account, <see langword="null"/> for manual accounts.</param>
    /// <param name="name">The name of the account.</param>
    /// <param name="description">A description of this account.</param>
    /// <param name="balance">The current balance of the account.</param>
    /// <param name="currency">The currency this account is denominated in.</param>
    /// <param name="iban">The IBAN of the bank account.</param>
    /// <param name="bic">The BIC (Business Identifier Code) associated with the account.</param>
    /// <param name="bban">Basic Bank Account Number represents a country-specific bank account number.
    /// This data element is used for payment accounts which have no IBAN.</param>
    /// <param name="ownerName">Name of the legal account owner. If there is more than one owner,
    /// then two names might be noted here.</param>
    /// <param name="accountType">Specifies the nature, or use, of the account.</param>
    /// <param name="associatedAuthenticationRequests">Authentication requests made for this bank account.</param>
    /// <param name="providerName">The name the data provider gives the account.</param>
    /// <param name="providerDescription">The description the data provider gives the account.</param>
    public BankAccount(DataSource source, string? externalId, string? name = null, string? description = null,
        decimal? balance = null, string? currency = null, string? iban = null, string? bic = null, string? bban = null,
        string? ownerName = null, string? accountType = null,
        List<AuthenticationRequest>? associatedAuthenticationRequests = null, string? providerName = null,
        string? providerDescription = null)
        : base(source, externalId, name, description, balance, currency)
    {
        Iban = iban;
        Bic = bic;
        Bban = bban;
        OwnerName = ownerName;
        AccountType = accountType;
        ProviderName = providerName;
        ProviderDescription = providerDescription;
        AssociatedInstitution = null;
        AssociatedAuthenticationRequests = associatedAuthenticationRequests ?? [];
    }

    /// <summary>
    /// Creates a new instance of <see cref="BankAccount"/>.
    /// </summary>
    /// <param name="source">Where the account originates from.</param>
    /// <param name="externalId">The id the data source assigned to the account, <see langword="null"/> for manual accounts.</param>
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
    /// <param name="associatedAuthenticationRequests">Authentication requests made for this bank account.</param>
    /// <param name="providerName">The name the data provider gives the account.</param>
    /// <param name="providerDescription">The description the data provider gives the account.</param>
    public BankAccount(DataSource source, string? externalId, string? name, string? description, decimal? balance,
        string? currency, List<Transaction> transactions,
        string? iban, string? bic, string? bban, string? ownerName, string? accountType,
        BankingInstitution? associatedInstitution, List<AuthenticationRequest> associatedAuthenticationRequests,
        string? providerName = null, string? providerDescription = null)
        : base(source, externalId, name, description, balance, currency, transactions)
    {
        Iban = iban;
        Bic = bic;
        Bban = bban;
        OwnerName = ownerName;
        AccountType = accountType;
        ProviderName = providerName;
        ProviderDescription = providerDescription;
        AssociatedInstitution = associatedInstitution;
        AssociatedAuthenticationRequests = associatedAuthenticationRequests;
    }

    /// <summary>
    /// Creates a new instance from the supplied bank account without the navigation properties set.
    /// </summary>
    /// <param name="bankAccount">The bank account to mutate.</param>
    /// <returns>New instance of <see cref="BankAccount"/> without navigation properties set.</returns>
    public static BankAccount CreateWithoutNavigationProperties(BankAccount bankAccount)
    {
        return new BankAccount(bankAccount.Source, bankAccount.ExternalId, bankAccount.Name, bankAccount.Description,
            bankAccount.Balance, bankAccount.Currency, bankAccount.Iban, bankAccount.Bic, bankAccount.Bban,
            bankAccount.OwnerName, bankAccount.AccountType, providerName: bankAccount.ProviderName,
            providerDescription: bankAccount.ProviderDescription);
    }

    /// <summary>
    /// Updates the (non-navigation) properties of the current <see cref="BankAccount"/> instance with the values
    /// provided in the specified <paramref name="updatedAccount"/>. Only properties that are not <see langword="null"/> in the <paramref name="updatedAccount"/> will be updated.
    /// The user's <see cref="Account.Name"/> and <see cref="Account.Description"/> are left untouched.
    /// </summary>
    /// <param name="updatedAccount">The <see cref="BankAccount"/> instance whose properties are used to update the current object.</param>
    public void UpdateNonNavigationProperties(BankAccount updatedAccount)
    {
        if (updatedAccount.ProviderName != null)
            ProviderName = updatedAccount.ProviderName;
        if (updatedAccount.ProviderDescription != null)
            ProviderDescription = updatedAccount.ProviderDescription;
        if (updatedAccount.Iban != null)
            Iban = updatedAccount.Iban;
        if (updatedAccount.Bic != null)
            Bic = updatedAccount.Bic;
        if (updatedAccount.Bban != null)
            Bban = updatedAccount.Bban;
        if (updatedAccount.Balance != null)
            Balance = updatedAccount.Balance;
        if (updatedAccount.Currency != null)
            Currency = updatedAccount.Currency;
        if (updatedAccount.OwnerName != null)
            OwnerName = updatedAccount.OwnerName;
        if (updatedAccount.AccountType != null)
            AccountType = updatedAccount.AccountType;
    }
}
