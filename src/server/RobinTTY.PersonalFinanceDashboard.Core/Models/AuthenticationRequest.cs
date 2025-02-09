namespace RobinTTY.PersonalFinanceDashboard.Core.Models;

public class AuthenticationRequest
{
    /// <summary>
    /// The id of this authentication request.
    /// </summary>
    public Guid Id { get; set; }
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
    public ICollection<BankAccount> AssociatedAccounts { get; set; } = new List<BankAccount>();
    
    /// <summary>
    /// Creates a new instance of <see cref="AuthenticationRequest"/>.
    /// Required constructor for Ef Core.
    /// </summary>
    /// <param name="id">The id of this authentication request.</param>
    /// <param name="status">The status of this authentication request.</param>
    /// <param name="authenticationLink">A <see cref="Uri"/> which can be used to start the authentication via the third party provider.</param>
    public AuthenticationRequest(Guid id, AuthenticationStatus status, Uri authenticationLink)
    {
        Id = id;
        Status = status;
        AuthenticationLink = authenticationLink;
    }

    /// <summary>
    /// Creates a new instance of <see cref="AuthenticationRequest"/>.
    /// </summary>
    /// <param name="id">The id of this authentication request.</param>
    /// <param name="status">The status of this authentication request.</param>
    /// <param name="authenticationLink">A <see cref="Uri"/> which can be used to start the authentication via the third party provider.</param>
    /// <param name="associatedAccounts">The ids of the accounts associated with this authentication request.</param>
    public AuthenticationRequest(Guid id, AuthenticationStatus status, Uri authenticationLink,
        ICollection<BankAccount> associatedAccounts)
    {
        Id = id;
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
