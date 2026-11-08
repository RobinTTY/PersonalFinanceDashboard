namespace RobinTTY.PersonalFinanceDashboard.Api.Resolvers.Inputs;

/// <summary>
/// Payload required to update a bank account through the API.
/// </summary>
public record UpdateBankAccountInput
{
    /// <summary>
    /// The id of the bank account to update.
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    /// The name of the account.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// A description of this account.
    /// </summary>
    public string? Description { get; init; }
}