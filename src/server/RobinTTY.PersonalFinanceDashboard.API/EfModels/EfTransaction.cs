namespace RobinTTY.PersonalFinanceDashboard.API.EfModels;

/// <summary>
/// A transaction represents a monetary exchange between 2 parties.
/// </summary>
public class EfTransaction
{
    /// <summary>
    /// The id of the transaction.
    /// </summary>
    public string Id { get; set; }
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
    public string Category { get; set; }
    /// <summary>
    /// Tags associated with the transaction (to associate expenses with certain sub-categories).
    /// </summary>
    public ICollection<EfTag> Tags { get; }
    /// <summary>
    /// User created notes for this transaction.
    /// </summary>
    public string Notes { get; set; }

    // TODO: Information provided by your bank (e.g. SEPA mandate ids)
    // how should I best handle this?

    /// <summary>
    /// Creates a new instance of <see cref="EfTransaction"/>.
    /// </summary>
    /// <param name="id">The id of the transaction.</param>
    /// <param name="valueDate">Date at which the transaction amount becomes available to the payee.</param>
    /// <param name="payer">The name of the party which owes the money.</param>
    /// <param name="payee">The name of the party which is owed the money.</param>
    /// <param name="amount">The amount being transacted.</param>
    /// <param name="currency">The currency the amount is denominated in.</param>
    /// <param name="category">The category this transaction belongs to.</param>
    /// <param name="notes">User created notes for this transaction.</param>
    public EfTransaction(string id, DateTime? valueDate, string payer, string payee, decimal amount, string currency, string category, string notes)
    {
        Id = id;
        ValueDate = valueDate;
        Payer = payer;
        Payee = payee;
        Amount = amount;
        Currency = currency;
        Category = category;
        Tags = new List<EfTag>();
        Notes = notes;
    }
}
