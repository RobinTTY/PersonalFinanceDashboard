namespace RobinTTY.PersonalFinanceDashboard.Core.Models;

/// <summary>
/// The state of the synchronization of an entity with its data source.
/// </summary>
public enum SyncState
{
    /// <summary>
    /// The entity is created by the user and isn't synchronized.
    /// </summary>
    NotSynced,

    /// <summary>
    /// The last synchronization succeeded.
    /// </summary>
    UpToDate,

    /// <summary>
    /// The last synchronization failed and is retried later.
    /// </summary>
    Retrying,

    /// <summary>
    /// The access to the data source expired or was revoked, so the user has to authenticate again.
    /// </summary>
    NeedsReauthentication
}

/// <summary>
/// How up to date an entity is with its data source.
/// </summary>
/// <param name="State">The state of the synchronization.</param>
/// <param name="LastSuccessfulSync">When all data of the entity was last synchronized successfully,
/// <see langword="null"/> if it never was.</param>
/// <param name="LastError">The error of the last failed synchronization, <see langword="null"/> if it succeeded.</param>
/// <param name="NextRetry">When a failed synchronization is retried, <see langword="null"/> if none failed.</param>
public sealed record SyncStatus(
    SyncState State,
    DateTime? LastSuccessfulSync = null,
    string? LastError = null,
    DateTime? NextRetry = null);
