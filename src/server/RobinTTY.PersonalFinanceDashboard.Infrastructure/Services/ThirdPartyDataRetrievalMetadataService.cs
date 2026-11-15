using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Repositories;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Services;

/// <summary>
/// Service used to manage the data retrieval from third party services. It is used to
/// make sure data is only retrieved once it has gone stale as specified by the given data
/// retrieval interval.
/// </summary>
/// <param name="thirdPartyDataRetrievalMetadataRepository">The repository which holds the
/// <see cref="ThirdPartyDataRetrievalMetadata"/> used to determine when data is stale.</param>
/// <param name="timeProvider">Provides the current time.</param>
public class ThirdPartyDataRetrievalMetadataService(
    ThirdPartyDataRetrievalMetadataRepository thirdPartyDataRetrievalMetadataRepository,
    TimeProvider timeProvider)
{
    /// <summary>
    /// Checks whether the data of the given <see cref="ThirdPartyDataType"/> and data source, stored in the
    /// database, has gone stale.
    /// </summary>
    /// <param name="dataSource">The data source the data is retrieved from.</param>
    /// <param name="thirdPartyDataType">The data type for which to check.</param>
    /// <param name="synchronizationEntityId">Optional id of the synchronization entity for which to check.</param>
    /// <returns><see langword="true"/> if the data stored in the db is stale,
    /// <see langword="false"/> otherwise.</returns>
    public async Task<bool> DataIsStale(DataSource dataSource, ThirdPartyDataType thirdPartyDataType,
        Guid? synchronizationEntityId = null)
    {
        var retrievalMetadata = await thirdPartyDataRetrievalMetadataRepository
            .GetThirdPartyDataRetrievalMetadata(dataSource, thirdPartyDataType, synchronizationEntityId);

        if (retrievalMetadata is null)
        {
            return true;
        }

        var nextRetrievalTime = retrievalMetadata.LastRetrievalTime + retrievalMetadata.RetrievalInterval;
        return nextRetrievalTime <= timeProvider.GetUtcNow().UtcDateTime;
    }

    /// <summary>
    /// Sets the last retrieval time for the given <see cref="ThirdPartyDataType"/> and data source to the
    /// current moment.
    /// </summary>
    /// <param name="dataSource">The data source the data was retrieved from.</param>
    /// <param name="thirdPartyDataType">The data type for which to reset the data expiry.</param>
    /// <param name="synchronizationEntityId">Optional id of the synchronization entity for which to reset the data expiry.</param>
    public async Task ResetDataExpiry(DataSource dataSource, ThirdPartyDataType thirdPartyDataType,
        Guid? synchronizationEntityId = null)
    {
        await thirdPartyDataRetrievalMetadataRepository.ResetLastRetrievalTime(dataSource, thirdPartyDataType,
            synchronizationEntityId);
    }
}