namespace RobinTTY.PersonalFinanceDashboard.Api.Resolvers.Inputs;

/// <summary>
/// TODO
/// </summary>
public record CreateTransactionInput(
    string AccountId,
    DateTime? ValueDate,
    string? Payer,
    string? Payee,
    decimal Amount,
    string Currency,
    string Category,
    string Notes)
{
    /// <summary>
    /// The id of the account to which the transaction belongs.
    /// </summary>
    public string AccountId { get; set; } = AccountId;
    /// <summary>
    /// Date at which the transaction amount becomes available to the payee.
    /// </summary>
    public DateTime? ValueDate { get; set; } = ValueDate;
    /// <summary>
    /// The name of the party which owes the money.
    /// </summary>
    public string? Payer { get; set; } = Payer;
    /// <summary>
    /// The name of the party which is owed the money.
    /// </summary>
    public string? Payee { get; set; } = Payee;
    /// <summary>
    /// The amount being transacted.
    /// </summary>
    public decimal Amount { get; set; } = Amount;
    /// <summary>
    /// The currency the amount is denominated in.
    /// </summary>
    public string Currency { get; set; } = Currency;
    /// <summary>
    /// The category this transaction belongs to.
    /// </summary>
    // TODO: Shouldn't this be nullable?
    public string Category { get; set; } = Category;
    /// <summary>
    /// User created notes for this transaction.
    /// </summary>
    // TODO: Shouldn't this be nullable?
    public string Notes { get; set; } = Notes;
}
