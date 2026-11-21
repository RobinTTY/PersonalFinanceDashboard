using Microsoft.EntityFrameworkCore;
using RobinTTY.PersonalFinanceDashboard.Core.Models;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Repositories;

public class ThirdPartyDataRetrievalMetadataRepository(ApplicationDbContext dbContext)
{
    /// <summary>
    /// Retrieves a single third-party data retrieval metadata record based on the specified data source and type.
    /// </summary>
    /// <param name="dataSource">The data source the data is retrieved from.</param>
    /// <param name="dataType">The type of third-party data for which the retrieval metadata is being requested.</param>
    /// <param name="synchronizationEntityId">Optional id of the synchronization entity for which to retrieve the metadata. For example, for account data, this would be the account id.</param>
    /// <returns>The metadata record for the specified data type, or null if no match is found.</returns>
    public async Task<ThirdPartyDataRetrievalMetadata?> GetThirdPartyDataRetrievalMetadata(DataSource dataSource,
        ThirdPartyDataType dataType, Guid? synchronizationEntityId)
    {
        return await dbContext.ThirdPartyDataRetrievalMetadata
            .SingleOrDefaultAsync(metadata => metadata.DataSource == dataSource && metadata.DataType == dataType &&
                                              metadata.SynchronizationEntityId == synchronizationEntityId);
    }

    /// <summary>
    /// Gets the metadata record for the given data, or adds a new one for data that was never retrieved.
    /// Changes to the record are persisted by <see cref="SaveChanges"/>.
    /// </summary>
    /// <param name="dataSource">The data source the data is retrieved from.</param>
    /// <param name="dataType">The type of the data.</param>
    /// <param name="synchronizationEntityId">Optional id of the synchronization entity. For example, for account data, this would be the account id.</param>
    /// <returns>The tracked metadata record.</returns>
    public async Task<ThirdPartyDataRetrievalMetadata> GetOrAddThirdPartyDataRetrievalMetadata(
        DataSource dataSource, ThirdPartyDataType dataType, Guid? synchronizationEntityId)
    {
        var metadata = await GetThirdPartyDataRetrievalMetadata(dataSource, dataType, synchronizationEntityId);
        if (metadata is not null)
            return metadata;

        metadata = new ThirdPartyDataRetrievalMetadata
        {
            DataType = dataType,
            DataSource = dataSource,
            SynchronizationEntityId = synchronizationEntityId,
            LastRetrievalTime = DateTime.MinValue,
            RetrievalInterval = GetRetrievalInterval(dataType)
        };
        dbContext.ThirdPartyDataRetrievalMetadata.Add(metadata);

        return metadata;
    }

    /// <summary>
    /// Removes all metadata records of the given synchronization entity. The removal is persisted by
    /// <see cref="SaveChanges"/>.
    /// </summary>
    /// <param name="dataSource">The data source the entity originates from.</param>
    /// <param name="synchronizationEntityId">The id of the synchronization entity, e.g. an account id.</param>
    public async Task RemoveThirdPartyDataRetrievalMetadata(DataSource dataSource, Guid synchronizationEntityId)
    {
        var metadata = await dbContext.ThirdPartyDataRetrievalMetadata
            .Where(metadata => metadata.DataSource == dataSource &&
                               metadata.SynchronizationEntityId == synchronizationEntityId)
            .ToListAsync();
        dbContext.ThirdPartyDataRetrievalMetadata.RemoveRange(metadata);
    }

    /// <summary>
    /// Saves the changes made to metadata records.
    /// </summary>
    public async Task SaveChanges() => await dbContext.SaveChangesAsync();

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