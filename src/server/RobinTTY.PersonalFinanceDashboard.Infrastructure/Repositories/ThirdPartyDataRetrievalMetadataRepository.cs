using Microsoft.EntityFrameworkCore;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Entities;
using RobinTTY.PersonalFinanceDashboard.ThirdPartyDataProviders.Models;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Repositories;

public class ThirdPartyDataRetrievalMetadataRepository
{
    private readonly ApplicationDbContext _dbContext;

    public ThirdPartyDataRetrievalMetadataRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ThirdPartyDataRetrievalMetadataEntity> GetThirdPartyDataRetrievalMetadata(
        ThirdPartyDataType dataType)
    {
        return await _dbContext.ThirdPartyDataRetrievalMetadata
            .SingleAsync(metadata => metadata.DataType == dataType);
    }

    public async Task<ThirdPartyDataRetrievalMetadataEntity> UpdateThirdPartyDataRetrievalMetadata(
        ThirdPartyDataRetrievalMetadataEntity metadata)
    {
        var updateEntry = _dbContext.ThirdPartyDataRetrievalMetadata.Update(metadata);
        await _dbContext.SaveChangesAsync();
        
        return updateEntry.Entity;
    }
}
