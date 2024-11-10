using Microsoft.Extensions.Logging;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Repositories;
using RobinTTY.PersonalFinanceDashboard.ThirdPartyDataProviders.Models;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Services;

/// <summary>
/// Service used to manage the data retrieval from third party services. It is used to
/// make sure data is only retrieved once it has gone stale as specified by the given data
/// retrieval interval.
/// </summary>
/// <param name="thirdPartyDataRetrievalMetadataRepository">The repository which holds the
/// <see cref="ThirdPartyDataRetrievalMetadata"/> used to determine when data is stale.</param>
public class ThirdPartyDataRetrievalMetadataService(
    ILogger logger,
    ThirdPartyDataRetrievalMetadataRepository thirdPartyDataRetrievalMetadataRepository)
{
    /// <summary>
    /// Checks whether the data for the given <see cref="ThirdPartyDataType"/>, stored in the
    /// database, has gone stale.
    /// </summary>
    /// <param name="thirdPartyDataType">The data type for which to check.</param>
    /// <returns><see langword="true"/> if the data stored in the db is stale,
    /// <see langword="false"/> otherwise.</returns>
    public async Task<bool> DataIsStale(ThirdPartyDataType thirdPartyDataType)
    {
        var retrievalMetadata = await thirdPartyDataRetrievalMetadataRepository
            .GetThirdPartyDataRetrievalMetadata(thirdPartyDataType);
        var nextRetrievalTime = retrievalMetadata.LastRetrievalTime + retrievalMetadata.RetrievalInterval;

        return nextRetrievalTime <= DateTime.Now;
    }

    /// <summary>
    /// Sets the last retrieval time for the given <see cref="ThirdPartyDataType"/> to the
    /// current moment.
    /// </summary>
    /// <param name="thirdPartyDataType">The data type for which to reset the data expiry.</param>
    public async Task ResetDataExpiry(ThirdPartyDataType thirdPartyDataType)
    {
        var resetWasSuccessful =
            await thirdPartyDataRetrievalMetadataRepository.ResetLastRetrievalTime(thirdPartyDataType);

        if (!resetWasSuccessful)
            logger.LogError("Could not reset the data expiry time for ThirdPartyDataType {dataType}",
                thirdPartyDataType);
    }
}
