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
    /// The category this transaction belongs to.
    /// </summary>
    public required string Category { get; init; }

    /// <summary>
    /// User created notes for this transaction.
    /// </summary>
    public required string Notes { get; init; }
}