using Microsoft.EntityFrameworkCore;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.ThirdPartyDataProviders.Models;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Repositories;

public class ThirdPartyDataRetrievalMetadataRepository
{
    private readonly ApplicationDbContext _dbContext;

    public ThirdPartyDataRetrievalMetadataRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ThirdPartyDataRetrievalMetadata> GetThirdPartyDataRetrievalMetadata(
        ThirdPartyDataType dataType)
    {
        return await _dbContext.ThirdPartyDataRetrievalMetadata
            .SingleAsync(metadata => metadata.DataType == dataType);
    }

    public async Task<bool> ResetLastRetrievalTime(ThirdPartyDataType currentDataType)
    {
        var changedRows = await _dbContext.ThirdPartyDataRetrievalMetadata
            .Where(metadata => metadata.DataType == currentDataType)
            .ExecuteUpdateAsync(update => update
                .SetProperty(metadata => metadata.LastRetrievalTime, DateTime.Now));

        return changedRows == 1;
    }
}
