using Microsoft.EntityFrameworkCore;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Services;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Repositories;

public class ThirdPartyDataRetrievalMetadataRepository(ApplicationDbContext dbContext)
{
    /// <summary>
    /// Retrieves a single third-party data retrieval metadata record based on the specified data type.
    /// </summary>
    /// <param name="dataType">The type of third-party data for which the retrieval metadata is being requested.</param>
    /// <param name="synchronizationEntityId">Optional id of the synchronization entity for which to retrieve the metadata. For example, for account data, this would be the account id.</param>
    /// <returns>The metadata record for the specified data type, or null if no match is found.</returns>
    public async Task<ThirdPartyDataRetrievalMetadata?> GetThirdPartyDataRetrievalMetadata(
        ThirdPartyDataType dataType, Guid? synchronizationEntityId)
    {
        return await dbContext.ThirdPartyDataRetrievalMetadata
            .SingleOrDefaultAsync(metadata =>
                metadata.DataType == dataType && metadata.SynchronizationEntityId == synchronizationEntityId);
    }

    /// <summary>
    /// Adds a new third-party data retrieval metadata record to the database.
    /// </summary>
    /// <param name="metadata">The metadata entity containing information about the third-party data retrieval, such as data type, data source, last retrieval time, and retrieval interval.</param>
    /// <returns>The metadata entity that was added to the database, including any database-generated values.</returns>
    private async Task AddThirdPartyDataRetrievalMetadata(ThirdPartyDataRetrievalMetadata metadata)
    {
        await dbContext.ThirdPartyDataRetrievalMetadata.AddAsync(metadata);
        await dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Resets the last retrieval time for the synchronization entity with the given id and data type to the current time.
    /// If no record exists for the given entry, one will be created.
    /// </summary>
    /// <param name="dataType">The data type for which to reset the last retrieval time.</param>
    /// <param name="synchronizationEntityId">The id of the synchronization entity for which the last retrieval time should be reset. For example, for account data, this would be the account id.</param>
    /// <returns>True if the last retrieval time was reset successfully, false otherwise.</returns>
    public async Task ResetLastRetrievalTime(ThirdPartyDataType dataType, Guid? synchronizationEntityId)
    {
        var existingMetadata = await GetThirdPartyDataRetrievalMetadata(dataType, synchronizationEntityId);

        if (existingMetadata is null)
        {
            var newMetadata = new ThirdPartyDataRetrievalMetadata
            {
                DataType = dataType,
                DataSource = ThirdPartyDataSource.GoCardless,
                SynchronizationEntityId = synchronizationEntityId,
                LastRetrievalTime = DateTime.Now,
                RetrievalInterval = GetRetrievalInterval(dataType)
            };

            await AddThirdPartyDataRetrievalMetadata(newMetadata);
        }
        else
        {
            existingMetadata.LastRetrievalTime = DateTime.Now;
            await dbContext.SaveChangesAsync();
        }
    }

    private static TimeSpan GetRetrievalInterval(ThirdPartyDataType dataType)
    {
        return dataType switch
        {
            ThirdPartyDataType.BankingInstitutions or ThirdPartyDataType.AuthenticationRequests => TimeSpan.FromDays(7),
            ThirdPartyDataType.Transactions or ThirdPartyDataType.BankAccounts => TimeSpan.FromDays(1),
            _ => throw new ArgumentException("Data type cannot be undefined.", nameof(dataType))
        };
    }
}