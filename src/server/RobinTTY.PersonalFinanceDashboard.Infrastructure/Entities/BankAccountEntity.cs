namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Entities;

public class BankAccountEntity : BaseEntity
{
    /// <summary>
    /// The name of the account.
    /// </summary>
    public string? Name { get; set; }
    /// <summary>
    /// A description of this account.
    /// </summary>
    public string? Description { get; set; }
    /// <summary>
    /// The current balance of the account.
    /// </summary>
    public decimal? Balance { get; set; }
    /// <summary>
    /// The currency this account is denominated in.
    /// </summary>
    public string? Currency { get; set; }
    /// <summary>
    /// Transactions that are associated with this account.
    /// </summary>
    public ICollection<TransactionEntity> Transactions { get; set; }
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
    public BankingInstitutionEntity? AssociatedInstitution { get; set; }

    public BankAccountEntity(Guid id, string? name, string? description, decimal? balance, string? currency,
        string? iban, string? bic, string? bban, string? ownerName, string? accountType) : base(id)
    {
        Name = name;
        Description = description;
        Balance = balance;
        Currency = currency;
        Transactions = new List<TransactionEntity>();
        Iban = iban;
        Bic = bic;
        Bban = bban;
        OwnerName = ownerName;
        AccountType = accountType;
        AssociatedInstitution = null;
    }
}
