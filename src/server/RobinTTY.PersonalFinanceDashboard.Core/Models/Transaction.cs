using RobinTTY.PersonalFinanceDashboard.Core.Models.Base;

namespace RobinTTY.PersonalFinanceDashboard.Core.Models;

/// <summary>
/// A transaction represents a monetary exchange between 2 parties.
/// </summary>
public class Transaction : SourcedEntity
{
    /// <summary>
    /// The id of the transaction provided by the banking institution.
    /// </summary>
    public string? BankTransactionId { get; set; }

    /// <summary>
    /// Whether the transaction was already posted to the account.
    /// </summary>
    public TransactionStatus Status { get; set; }

    /// <summary>
    /// Date at which the transaction was posted to the account.
    /// </summary>
    public DateTime? BookingDate { get; set; }

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
    /// What the transaction is for, e.g. the reference of a transfer or the details of a card payment.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// The amount being transacted.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// The currency the amount is denominated in.
    /// </summary>
    public string Currency { get; set; }

    /// <summary>
    /// The category this transaction belongs to, set by the user.
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// User created notes for this transaction.
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// The bank account associated with the transaction, representing the source or destination of funds.
    /// </summary>
    public BankAccount? BankAccount { get; set; }

    /// <summary>
    /// Creates a new instance of <see cref="Transaction"/>.
    /// </summary>
    public Transaction()
    {
        Currency = string.Empty;
    }

    /// <summary>
    /// Creates a new instance of <see cref="Transaction"/>.
    /// </summary>
    /// <param name="source">Where the transaction originates from.</param>
    /// <param name="externalId">The id the data source assigned to the transaction, <see langword="null"/> for manual transactions.</param>
    /// <param name="bankTransactionId">The id of the transaction provided by the third party data retrieval service.</param>
    /// <param name="valueDate">Date at which the transaction amount becomes available to the payee.</param>
    /// <param name="payer">The name of the party which owes the money.</param>
    /// <param name="payee">The name of the party which is owed the money.</param>
    /// <param name="amount">The amount being transacted.</param>
    /// <param name="currency">The currency the amount is denominated in.</param>
    /// <param name="category">The category this transaction belongs to.</param>
    /// <param name="notes">User created notes for this transaction.</param>
    /// <param name="bankAccount">The bank account associated with the transaction, representing the source or destination of funds.</param>
    /// <param name="status">Whether the transaction was already posted to the account.</param>
    /// <param name="bookingDate">Date at which the transaction was posted to the account.</param>
    /// <param name="description">What the transaction is for.</param>
    public Transaction(DataSource source, string? externalId, string? bankTransactionId, DateTime? valueDate,
        string? payer, string? payee, decimal amount, string currency, string? category, string? notes,
        BankAccount? bankAccount, TransactionStatus status = TransactionStatus.Booked, DateTime? bookingDate = null,
        string? description = null) : base(source, externalId)
    {
        BankTransactionId = bankTransactionId;
        Status = status;
        BookingDate = bookingDate;
        ValueDate = valueDate;
        Payer = payer;
        Payee = payee;
        Description = description;
        Amount = amount;
        Currency = currency;
        Category = category;
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
        return new Transaction(transaction.Source, transaction.ExternalId, transaction.BankTransactionId,
            transaction.ValueDate, transaction.Payer, transaction.Payee, transaction.Amount, transaction.Currency,
            transaction.Category, transaction.Notes, null, transaction.Status, transaction.BookingDate,
            transaction.Description);
    }

    /// <summary>
    /// Updates the (non-navigation) properties of the current <see cref="Transaction"/> instance with the values
    /// provided in the specified <paramref name="transaction"/>. User-entered properties (<see cref="Category"/> and
    /// <see cref="Notes"/>) are not updated, since third party data doesn't contain them.
    /// </summary>
    /// <param name="transaction">The <see cref="Transaction"/> instance whose properties are used to update the current object.</param>
    public void UpdateNonNavigationProperties(Transaction transaction)
    {
        BankTransactionId = transaction.BankTransactionId;
        Status = transaction.Status;
        BookingDate = transaction.BookingDate;
        ValueDate = transaction.ValueDate;
        Payer = transaction.Payer;
        Payee = transaction.Payee;
        Description = transaction.Description;
        Amount = transaction.Amount;
        Currency = transaction.Currency;
    }
}

/// <summary>
/// Whether a <see cref="Transaction"/> was already posted to the account.
/// </summary>
public enum TransactionStatus
{
    /// <summary>
    /// The transaction was posted to the account.
    /// </summary>
    Booked,

    /// <summary>
    /// The transaction wasn't posted to the account yet. It may still change, or be replaced by a booked transaction
    /// with a different id.
    /// </summary>
    Pending
}
