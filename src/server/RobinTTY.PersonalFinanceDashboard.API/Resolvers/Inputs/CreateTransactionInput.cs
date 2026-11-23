namespace RobinTTY.PersonalFinanceDashboard.Api.Resolvers.Inputs;

/// <summary>
/// Payload required to create a transaction through the API.
/// </summary>
public record CreateTransactionInput
{
    /// <summary>
    /// The id of the account to which the transaction belongs.
    /// </summary>
    public required Guid AccountId { get; init; }

    /// <summary>
    /// Date at which the transaction amount becomes available to the payee.
    /// </summary>
    public DateTime? ValueDate { get; init; }

    /// <summary>
    /// The name of the party which owes the money.
    /// </summary>
    public string? Payer { get; init; }

    /// <summary>
    /// The name of the party which is owed the money.
    /// </summary>
    public string? Payee { get; init; }

    /// <summary>
    /// What the transaction is for.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// The amount being transacted.
    /// </summary>
    public required decimal Amount { get; init; }

    /// <summary>
    /// The currency the amount is denominated in.
    /// </summary>
    public required string Currency { get; init; }

    /// <summary>
    /// The category this transaction belongs to.
    /// </summary>
    public string? Category { get; init; }

    /// <summary>
    /// User created notes for this transaction.
    /// </summary>
    public string? Notes { get; init; }
}
