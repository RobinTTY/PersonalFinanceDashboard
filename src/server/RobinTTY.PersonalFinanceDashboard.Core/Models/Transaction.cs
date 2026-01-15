using RobinTTY.PersonalFinanceDashboard.Core.Models.Base;

namespace RobinTTY.PersonalFinanceDashboard.Core.Models;

/// <summary>
/// A transaction represents a monetary exchange between 2 parties.
/// </summary>
public class Transaction : ThirdPartyEntity
{
    /// <summary>
    /// The id of the transaction provided by the banking institution.
    /// </summary>
    public string? BankTransactionId { get; set; }

    /// <summary>
    /// Date at which the transaction amount becomes available to the payee.
    /// </summary>
    public DateTime? ValueDate { get; set; }

    /// <summary>
    /// The name of the party which owes the money.
    /// </summary>
    public string? Payer { get; set; }

    /// <summary>
    /// The name of the party which is owed the money.
    /// </summary>
    public string? Payee { get; set; }

    /// <summary>
    /// The amount being transacted.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// The currency the amount is denominated in.
    /// </summary>
    public string Currency { get; set; }

    /// <summary>
    /// The category this transaction belongs to.
    /// </summary>
    // TODO: Shouldn't this be nullable?
    public string Category { get; set; }

    /// <summary>
    /// User created notes for this transaction.
    /// </summary>
    // TODO: Shouldn't this be nullable?
    public string Notes { get; set; }

    /// <summary>
    /// Tags associated with the transaction (to associate expenses with certain subcategories).
    /// </summary>
    public List<Tag> Tags { get; set; } = [];

    /// <summary>
    /// The bank account associated with the transaction, representing the source or destination of funds.
    /// </summary>
    public BankAccount? BankAccount { get; set; }

    /// <summary>
    /// Creates a new instance of <see cref="Transaction"/>.
    /// </summary>
    public Transaction()
    {
        BankTransactionId = null;
        ValueDate = null;
        Payer = null;
        Payee = null;
        Amount = decimal.MinValue;
        Currency = string.Empty;
        Category = string.Empty;
        Notes = string.Empty;
    }

    /// <summary>
    /// Creates a new instance of <see cref="Transaction"/>.
    /// </summary>
    /// <param name="thirdPartyId">The id of the transaction provided by the third party data provider.</param>
    /// <param name="bankTransactionId">The id of the transaction provided by the third party data retrieval service.</param>
    /// <param name="valueDate">Date at which the transaction amount becomes available to the payee.</param>
    /// <param name="payer">The name of the party which owes the money.</param>
    /// <param name="payee">The name of the party which is owed the money.</param>
    /// <param name="amount">The amount being transacted.</param>
    /// <param name="currency">The currency the amount is denominated in.</param>
    /// <param name="category">The category this transaction belongs to.</param>
    /// <param name="notes">User created notes for this transaction.</param>
    /// <param name="tags">Tags associated with the transaction (to associate expenses with certain subcategories).</param>
    /// <param name="bankAccount">The bank account associated with the transaction, representing the source or destination of funds.</param>
    public Transaction(Guid thirdPartyId, string? bankTransactionId, DateTime? valueDate, string? payer, string? payee,
        decimal amount,
        string currency, string category, string notes, List<Tag> tags, BankAccount? bankAccount)
    {
        ThirdPartyId = thirdPartyId;
        BankTransactionId = bankTransactionId;
        ValueDate = valueDate;
        Payer = payer;
        Payee = payee;
        Amount = amount;
        Currency = currency;
        Category = category;
        Tags = tags;
        Notes = notes;
        BankAccount = bankAccount;
    }

    /// <summary>
    /// Creates a new instance from the supplied transaction without the navigation properties set.
    /// </summary>
    /// <param name="transaction">The transaction to mutate.</param>
    /// <returns>New instance of <see cref="Transaction"/> without navigation properties set.</returns>
    public static Transaction CreateWithoutNavigationProperties(Transaction transaction)
    {
        return new Transaction(transaction.ThirdPartyId, transaction.BankTransactionId, transaction.ValueDate,
            transaction.Payer, transaction.Payee, transaction.Amount, transaction.Currency, transaction.Category,
            transaction.Notes, [], null);
    }

    /// <summary>
    /// Updates the (non-navigation) properties of the current <see cref="Transaction"/> instance with the values
    /// provided in the specified <paramref name="transaction"/>.
    /// </summary>
    /// <param name="transaction">The <see cref="Transaction"/> instance whose properties are used to update the current object.</param>
    /// <returns>The updated <see cref="Transaction"/> instance.</returns>
    public void UpdateNonNavigationProperties(Transaction transaction)
    {
        BankTransactionId = transaction.BankTransactionId;
        ValueDate = transaction.ValueDate;
        Payer = transaction.Payer;
        Payee = transaction.Payee;
        Amount = transaction.Amount;
        Currency = transaction.Currency;
        Category = transaction.Category;
        Notes = transaction.Notes;
    }
}