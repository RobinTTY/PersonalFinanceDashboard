namespace RobinTTY.PersonalFinanceDashboard.Api.Resolvers.Inputs;

/// <summary>
/// Payload required to update a banking institution through the API.
/// </summary>
public record UpdateBankingInstitutionInput
{
    /// <summary>
    /// The id of the banking institution to update.
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    /// The name of the institution.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// The Business Identifier Code (BIC) of the institution.
    /// </summary>
    public required string Bic { get; init; }

    /// <summary>
    /// A <see cref="Uri"/> for the logo of the institution.
    /// </summary>
    public required Uri LogoUri { get; init; }

    /// <summary>
    /// The countries the institution operates in.
    /// </summary>
    public required List<string> Countries { get; init; }
}