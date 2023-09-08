namespace RobinTTY.PersonalFinanceDashboard.Core.Models;

public class BankingInstitution
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
    /// Creates a new instance of <see cref="BankingInstitution"/>.
    /// </summary>
    /// <param name="bic">The Business Identifier Code (BIC) of the institution.</param>
    /// <param name="name">The name of the institution.</param>
    /// <param name="logoUri">A <see cref="Uri"/> for the logo of the institution</param>
    public BankingInstitution(string bic, string name, Uri logoUri)
    {
        Bic = bic;
        Name = name;
        LogoUri = logoUri;
    }
}
