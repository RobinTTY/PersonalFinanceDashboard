namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Services.DataSynchronization.Interfaces;

/// <summary>
/// Synchronizes the authentication requests of the data providers, and the accounts they grant access to, with the
/// database.
/// </summary>
public interface IAuthenticationRequestSyncHandler
{
    /// <summary>
    /// Retrieves the authentication requests that are due for retrieval and updates the database. Requests the
    /// data provider no longer returns are removed.
    /// </summary>
    /// <param name="authenticationRequestId">The id of a single authentication request to retrieve,
    /// <see langword="null"/> to retrieve all of them.</param>
    /// <param name="forceThirdPartySync">Whether to retrieve the data even if it isn't due.</param>
    /// <param name="cancellationToken">A token to signal cancellation.</param>
    /// <returns>Whether the authentication requests were retrieved, or didn't need to be.</returns>
    Task<bool> SynchronizeData(Guid? authenticationRequestId = null, bool forceThirdPartySync = false,
        CancellationToken cancellationToken = default);
}
