using RobinTTY.PersonalFinanceDashboard.Core.Models.Base;

namespace RobinTTY.PersonalFinanceDashboard.Core.Models;

public class AuthenticationRequest : ThirdPartyEntity
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
    /// The ids of the accounts associated with this authentication request.
    /// </summary>
    public List<BankAccount> AssociatedAccounts { get; set; } = [];

    /// <summary>
    /// Creates a new instance of <see cref="AuthenticationRequest"/>.
    /// </summary>
    public AuthenticationRequest()
    {
        ThirdPartyId = Guid.Empty;
        Status = AuthenticationStatus.Unknown;
        AuthenticationLink = new Uri("https://www.placeholder.example/");
    }
    
    /// <summary>
    /// Creates a new instance of <see cref="AuthenticationRequest"/>.
    /// Required constructor for Ef Core.
    /// </summary>
    /// <param name="thirdPartyId">The unique id assigned by the third party data retrieval service.</param>
    /// <param name="status">The status of this authentication request.</param>
    /// <param name="authenticationLink">A <see cref="Uri"/> which can be used to start the authentication via the third party provider.</param>
    public AuthenticationRequest(Guid thirdPartyId, AuthenticationStatus status, Uri authenticationLink)
    {
        ThirdPartyId = thirdPartyId;
        Status = status;
        AuthenticationLink = authenticationLink;
    }

    /// <summary>
    /// Creates a new instance of <see cref="AuthenticationRequest"/>.
    /// </summary>
    /// <param name="thirdPartyId">The unique id assigned by the third party data retrieval service.</param>
    /// <param name="status">The status of this authentication request.</param>
    /// <param name="authenticationLink">A <see cref="Uri"/> which can be used to start the authentication via the third party provider.</param>
    /// <param name="associatedAccounts">The ids of the accounts associated with this authentication request.</param>
    public AuthenticationRequest(Guid thirdPartyId, AuthenticationStatus status, Uri authenticationLink,
        List<BankAccount> associatedAccounts)
    {
        ThirdPartyId = thirdPartyId;
        Status = status;
        AuthenticationLink = authenticationLink;
        AssociatedAccounts = associatedAccounts;
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
    Successful,

    /// <summary>
    /// The authentication failed.
    /// </summary>
    Failed,

    /// <summary>
    /// The authentication has expired and is no longer valid.
    /// </summary>
    Expired
}
