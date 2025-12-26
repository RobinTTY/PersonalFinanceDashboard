using Microsoft.EntityFrameworkCore;
using RobinTTY.PersonalFinanceDashboard.Core.Models;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Repositories;

public class ThirdPartyDataRetrievalMetadataRepository(ApplicationDbContext dbContext)
{
    public async Task<ThirdPartyDataRetrievalMetadata> GetThirdPartyDataRetrievalMetadata(
        ThirdPartyDataType dataType)
    {
        return await dbContext.ThirdPartyDataRetrievalMetadata
            .SingleAsync(metadata => metadata.DataType == dataType);
    }

    public async Task<bool> ResetLastRetrievalTime(ThirdPartyDataType currentDataType)
    {
        var changedRows = await dbContext.ThirdPartyDataRetrievalMetadata
            .Where(metadata => metadata.DataType == currentDataType)
            .ExecuteUpdateAsync(update => update
                .SetProperty(metadata => metadata.LastRetrievalTime, DateTime.Now));

        return changedRows == 1;
    }
}
