using RobinTTY.PersonalFinanceDashboard.Infrastructure.Repositories;
using RobinTTY.PersonalFinanceDashboard.ThirdPartyDataProviders.Models;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Services;

/// <summary>
/// TODO
/// </summary>
/// <param name="thirdPartyDataRetrievalMetadataRepository"></param>
public class ThirdPartyDataRetrievalMetadataService(
    ThirdPartyDataRetrievalMetadataRepository thirdPartyDataRetrievalMetadataRepository)
{
    /// <summary>
    /// TODO
    /// </summary>
    /// <param name="thirdPartyDataType"></param>
    /// <returns></returns>
    public async Task<bool> DataIsStale(ThirdPartyDataType thirdPartyDataType)
    {
        var retrievalMetadata = await thirdPartyDataRetrievalMetadataRepository
            .GetThirdPartyDataRetrievalMetadata(thirdPartyDataType);
        var nextRetrievalTime = retrievalMetadata.LastRetrievalTime + retrievalMetadata.RetrievalInterval;

        return nextRetrievalTime <= DateTime.Now;
    }

    /// <summary>
    /// TODO
    /// </summary>
    /// <param name="thirdPartyDataType"></param>
    public async Task ResetDataExpiry(ThirdPartyDataType thirdPartyDataType)
    {
        var retrievalMetadata = await thirdPartyDataRetrievalMetadataRepository
            .GetThirdPartyDataRetrievalMetadata(thirdPartyDataType);
        
        retrievalMetadata.LastRetrievalTime = DateTime.Now;
        await thirdPartyDataRetrievalMetadataRepository
            .UpdateThirdPartyDataRetrievalMetadata(retrievalMetadata);
    }
}
