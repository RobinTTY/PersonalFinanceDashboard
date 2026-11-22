using RobinTTY.PersonalFinanceDashboard.Core.Models.Base;

namespace RobinTTY.PersonalFinanceDashboard.Core.Models;

/// <summary>
/// A request to access the accounts of a banking institution, which the user authenticates at the data source.
/// </summary>
public class AuthenticationRequest : SourcedEntity
{
    /// <summary>
    /// The status of this authentication request.
    /// </summary>
    public AuthenticationStatus Status { get; set; }

    /// <summary>
    /// A <see cref="Uri"/> which can be used to start the authentication via the third party provider.
    /// </summary>
    public Uri AuthenticationLink { get; set; }

    /// <summary>
    /// The date when the authentication request was created. Usually authentication requests are valid for 90 days.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// The banking institution whose accounts are accessed.
    /// </summary>
    public BankingInstitution? Institution { get; set; }

    /// <summary>
    /// The ids of the accounts associated with this authentication request.
    /// </summary>
    public List<BankAccount> AssociatedAccounts { get; set; } = [];

    /// <summary>
    /// Creates a new instance of <see cref="AuthenticationRequest"/>.
    /// </summary>
    public AuthenticationRequest()
    {
        Status = AuthenticationStatus.Unknown;
        AuthenticationLink = new Uri("https://www.placeholder.example/");
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Creates a new instance of <see cref="AuthenticationRequest"/>.
    /// Required constructor for Ef Core.
    /// </summary>
    /// <param name="source">The data source the authentication request was made at.</param>
    /// <param name="externalId">The id the data source assigned to the authentication request.</param>
    /// <param name="status">The status of this authentication request.</param>
    /// <param name="authenticationLink">A <see cref="Uri"/> which can be used to start the authentication via the third party provider.</param>
    /// <param name="createdAt">The date when the authentication request was created. Usually authentication requests are valid for 90 days.</param>
    public AuthenticationRequest(DataSource source, string externalId, AuthenticationStatus status,
        Uri authenticationLink, DateTime createdAt) : base(source, externalId)
    {
        Status = status;
        AuthenticationLink = authenticationLink;
        CreatedAt = createdAt;
    }

    /// <summary>
    /// Creates a new instance of <see cref="AuthenticationRequest"/>.
    /// </summary>
    /// <param name="source">The data source the authentication request was made at.</param>
    /// <param name="externalId">The id the data source assigned to the authentication request.</param>
    /// <param name="status">The status of this authentication request.</param>
    /// <param name="authenticationLink">A <see cref="Uri"/> which can be used to start the authentication via the third party provider.</param>
    /// <param name="createdAt">The date when the authentication request was created. Usually authentication requests are valid for 90 days.</param>
    /// <param name="associatedAccounts">The ids of the accounts associated with this authentication request.</param>
    public AuthenticationRequest(DataSource source, string externalId, AuthenticationStatus status,
        Uri authenticationLink, DateTime createdAt, List<BankAccount> associatedAccounts) : base(source, externalId)
    {
        Status = status;
        AuthenticationLink = authenticationLink;
        AssociatedAccounts = associatedAccounts;
        CreatedAt = createdAt;
    }

    public static AuthenticationRequest CreateWithoutNavigationProperties(AuthenticationRequest authenticationRequest)
    {
        return new AuthenticationRequest(authenticationRequest.Source, authenticationRequest.ExternalId!,
            authenticationRequest.Status,
            authenticationRequest.AuthenticationLink, authenticationRequest.CreatedAt);
    }
}

/// <summary>
/// The status of an <see cref="AuthenticationRequest"/>.
/// </summary>
public enum AuthenticationStatus
{
    /// <summary>
    /// Assigned if the status of the authentication request couldn't be determined.
    /// </summary>
    Unknown,

    /// <summary>
    /// A user action is required for the authentication to proceed.
    /// </summary>
    RequiresUserAction,

    /// <summary>
    /// The authentication is currently pending.
    /// </summary>
    Pending,

    /// <summary>
    /// The authentication was successful and associated accounts can be used.
    /// </summary>
    Active,

    /// <summary>
    /// The authentication failed.
    /// </summary>
    Failed,

    /// <summary>
    /// The authentication has expired and is no longer valid.
    /// </summary>
    Expired
}