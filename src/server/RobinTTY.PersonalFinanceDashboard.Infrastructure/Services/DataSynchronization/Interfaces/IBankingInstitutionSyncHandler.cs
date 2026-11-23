namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Services.DataSynchronization.Interfaces;

/// <summary>
/// Synchronizes the banking institutions of the data providers with the database.
/// </summary>
public interface IBankingInstitutionSyncHandler
{
    /// <summary>
    /// Retrieves the banking institutions if they are due for retrieval and updates the database. Institutions the
    /// data provider no longer returns are removed.
    /// </summary>
    /// <param name="forceThirdPartySync">Whether to retrieve the data even if it isn't due.</param>
    /// <param name="cancellationToken">A token to signal cancellation.</param>
    /// <returns>Whether the banking institutions were retrieved, or didn't need to be.</returns>
    Task<bool> SynchronizeData(bool forceThirdPartySync = false, CancellationToken cancellationToken = default);
}
