using RobinTTY.PersonalFinanceDashboard.Core.Models.Base;

namespace RobinTTY.PersonalFinanceDashboard.Core.Models;

/// <summary>
/// A banking institution to which <see cref="BankAccount"/>s belong.
/// </summary>
public class BankingInstitution : DatabaseEntity
{
    /// <summary>
    /// The unique id assigned by the third party data retrieval service.
    /// </summary>
    public string ThirdPartyId { get; set; }
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
        ThirdPartyId = string.Empty;
        Bic = string.Empty;
        Name = string.Empty;
        LogoUri = new Uri("https://example.com");
        Countries = [];
    }

    /// <summary>
    /// Creates a new instance of <see cref="BankingInstitution"/>.
    /// </summary>
    /// <param name="thirdPartyId">The unique id assigned by the third party data retrieval service.</param>
    /// <param name="bic">The Business Identifier Code (BIC) of the institution.</param>
    /// <param name="name">The name of the institution.</param>
    /// <param name="logoUri">A <see cref="Uri"/> for the logo of the institution</param>
    /// <param name="countries">The countries the institution operates in.</param>
    public BankingInstitution(string thirdPartyId, string bic, string name, Uri logoUri, List<string> countries)
    {
        ThirdPartyId = thirdPartyId;
        Bic = bic;
        Name = name;
        LogoUri = logoUri;
        Countries = countries;
    }
}
