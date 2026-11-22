using RobinTTY.PersonalFinanceDashboard.Core.Models;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Services.DataSynchronization.Interfaces;

/// <summary>
/// Synchronizes the data of the third-party data providers with the database. Only one synchronization runs at a
/// time.
/// </summary>
public interface IDataSynchronizer
{
    /// <summary>
    /// Synchronizes all data that is due for retrieval.
    /// </summary>
    /// <param name="forceThirdPartySync">Whether to retrieve the data even if it isn't due.</param>
    /// <param name="cancellationToken">A token to signal cancellation.</param>
    /// <returns>Whether all data was synchronized successfully.</returns>
    Task<bool> SynchronizeAll(bool forceThirdPartySync = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Synchronizes the data of one type that is due for retrieval.
    /// </summary>
    /// <param name="dataType">The type of the data to synchronize.</param>
    /// <param name="forceThirdPartySync">Whether to retrieve the data even if it isn't due.</param>
    /// <param name="cancellationToken">A token to signal cancellation.</param>
    /// <returns>Whether the data was synchronized successfully.</returns>
    Task<bool> Synchronize(ThirdPartyDataType dataType, bool forceThirdPartySync = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves an authentication request from its data provider, e.g. while waiting for the user to grant access.
    /// Once access was granted, its accounts are synchronized too, and a synchronization of all data is requested,
    /// which retrieves their transactions.
    /// </summary>
    /// <param name="authenticationRequestId">The id of the authentication request.</param>
    /// <param name="cancellationToken">A token to signal cancellation.</param>
    /// <returns>Whether the authentication request was synchronized successfully.</returns>
    Task<bool> RefreshAuthenticationRequest(Guid authenticationRequestId,
        CancellationToken cancellationToken = default);
}
