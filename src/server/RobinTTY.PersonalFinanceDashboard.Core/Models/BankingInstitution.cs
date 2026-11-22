using RobinTTY.PersonalFinanceDashboard.Core.Models.Base;

namespace RobinTTY.PersonalFinanceDashboard.Core.Models;

/// <summary>
/// A banking institution to which <see cref="BankAccount"/>s belong.
/// </summary>
public class BankingInstitution : SourcedEntity
{
    /// <summary>
    /// The Business Identifier Code (BIC) of the institution.
    /// </summary>
    public string Bic { get; set; }

    /// <summary>
    /// The name of the institution.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// A <see cref="Uri"/> for the logo of the institution
    /// </summary>
    public Uri LogoUri { get; set; }

    /// <summary>
    /// The countries the institution operates in.
    /// </summary>
    public List<string> Countries { get; set; } = [];

    /// <summary>
    /// Creates a new instance of <see cref="BankingInstitution"/>.
    /// </summary>
    public BankingInstitution()
    {
        Bic = string.Empty;
        Name = string.Empty;
        LogoUri = new Uri("https://example.com");
    }

    /// <summary>
    /// Creates a new instance of <see cref="BankingInstitution"/>.
    /// </summary>
    /// <param name="source">Where the institution originates from.</param>
    /// <param name="externalId">The id the data source assigned to the institution, <see langword="null"/> for manual institutions.</param>
    /// <param name="bic">The Business Identifier Code (BIC) of the institution.</param>
    /// <param name="name">The name of the institution.</param>
    /// <param name="logoUri">A <see cref="Uri"/> for the logo of the institution</param>
    /// <param name="countries">The countries the institution operates in.</param>
    public BankingInstitution(DataSource source, string? externalId, string bic, string name, Uri logoUri,
        List<string> countries) : base(source, externalId)
    {
        Bic = bic;
        Name = name;
        LogoUri = logoUri;
        Countries = countries;
    }
}
