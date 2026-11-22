namespace RobinTTY.PersonalFinanceDashboard.Api.Resolvers.Inputs;

/// <summary>
/// Payload required to update a transaction through the API.
/// </summary>
public record UpdateTransactionInput
{
    /// <summary>
    /// The id of the transaction to update.
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    /// The category this transaction belongs to, empty or omitted to remove it.
    /// </summary>
    public string? Category { get; init; }

    /// <summary>
    /// User created notes for this transaction, empty or omitted to remove them.
    /// </summary>
    public string? Notes { get; init; }
}
