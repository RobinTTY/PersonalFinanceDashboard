namespace RobinTTY.PersonalFinanceDashboard.Core.Models;

/// <summary>
/// TODO
/// </summary>
public class BankingInstitution
{
    /// <summary>
    /// The identifier of the bank.
    /// </summary>
    public string Id { get; set; }
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
    public List<string> Countries { get; set; }

    /// <summary>
    /// Creates a new instance of <see cref="BankingInstitution"/>.
    /// </summary>
    public BankingInstitution()
    {
        Id = string.Empty;
        Bic = string.Empty;
        Name = string.Empty;
        LogoUri = new Uri("https://example.com");
        Countries = [];
    }

    /// <summary>
    /// Creates a new instance of <see cref="BankingInstitution"/>.
    /// </summary>
    /// <param name="id">The identifier of the bank.</param>
    /// <param name="bic">The Business Identifier Code (BIC) of the institution.</param>
    /// <param name="name">The name of the institution.</param>
    /// <param name="logoUri">A <see cref="Uri"/> for the logo of the institution</param>
    /// <param name="countries">The countries the institution operates in.</param>
    public BankingInstitution(string id, string bic, string name, Uri logoUri, List<string> countries)
    {
        Id = id;
        Bic = bic;
        Name = name;
        LogoUri = logoUri;
        Countries = countries;
    }
}
