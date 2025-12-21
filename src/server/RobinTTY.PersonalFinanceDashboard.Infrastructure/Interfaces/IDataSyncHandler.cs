using RobinTTY.PersonalFinanceDashboard.Core.Models;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Interfaces;

public interface IDataSyncHandler
{
    /// <summary>
    /// Synchronizes data from the third party data source.
    /// </summary>
    /// <param name="forceThirdPartySync">Whether to force synchronization of data via the third party. If set to true
    /// the <see cref="ThirdPartyDataRetrievalMetadata"/> will be ignored and the sync will be forced.</param>
    /// <returns>Returns true if the sync was successful, otherwise returns false. </returns>
    Task<bool> SynchronizeData(bool forceThirdPartySync = false);
}