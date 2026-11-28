using Microsoft.EntityFrameworkCore;
using RobinTTY.PersonalFinanceDashboard.Core.Models;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Repositories;

/// <summary>
/// Manages <see cref="ThirdPartyDataRetrievalMetadata"/> data retrieval.
/// </summary>
/// <param name="dbContext">The <see cref="ApplicationDbContext"/> to use for data retrieval.</param>
public class ThirdPartyDataRetrievalMetadataRepository(ApplicationDbContext dbContext)
{
    /// <summary>
    /// Retrieves a single third-party data retrieval metadata record based on the specified data source and type.
    /// </summary>
    /// <param name="dataSource">The data source the data is retrieved from.</param>
    /// <param name="dataType">The type of third-party data for which the retrieval metadata is being requested.</param>
    /// <param name="synchronizationEntityId">Optional id of the synchronization entity for which to retrieve the metadata. For example, for account data, this would be the account id.</param>
    /// <param name="cancellationToken">A token to signal cancellation.</param>
    /// <returns>The metadata record for the specified data type, or null if no match is found.</returns>
    public async Task<ThirdPartyDataRetrievalMetadata?> GetThirdPartyDataRetrievalMetadata(DataSource dataSource,
        ThirdPartyDataType dataType, Guid? synchronizationEntityId, CancellationToken cancellationToken = default)
    {
        return await dbContext.ThirdPartyDataRetrievalMetadata
            .SingleOrDefaultAsync(metadata => metadata.DataSource == dataSource && metadata.DataType == dataType &&
                                              metadata.SynchronizationEntityId == synchronizationEntityId,
                cancellationToken);
    }

    /// <summary>
    /// Gets the metadata record for the given data, or adds a new one for data that was never retrieved.
    /// Changes to the record are persisted by <see cref="SaveChanges"/>.
    /// </summary>
    /// <param name="dataSource">The data source the data is retrieved from.</param>
    /// <param name="dataType">The type of the data.</param>
    /// <param name="synchronizationEntityId">Optional id of the synchronization entity. For example, for account data, this would be the account id.</param>
    /// <param name="cancellationToken">A token to signal cancellation.</param>
    /// <returns>The tracked metadata record.</returns>
    public async Task<ThirdPartyDataRetrievalMetadata> GetOrAddThirdPartyDataRetrievalMetadata(
        DataSource dataSource, ThirdPartyDataType dataType, Guid? synchronizationEntityId,
        CancellationToken cancellationToken = default)
    {
        var metadata = await GetThirdPartyDataRetrievalMetadata(dataSource, dataType, synchronizationEntityId,
            cancellationToken);
        if (metadata is not null)
            return metadata;

        metadata = new ThirdPartyDataRetrievalMetadata
        {
            DataType = dataType,
            DataSource = dataSource,
            SynchronizationEntityId = synchronizationEntityId,
            LastRetrievalTime = DateTime.MinValue
        };
        dbContext.ThirdPartyDataRetrievalMetadata.Add(metadata);

        return metadata;
    }

    /// <summary>
    /// Saves the changes made to metadata records.
    /// </summary>
    /// <param name="cancellationToken">A token to signal cancellation.</param>
    public async Task SaveChanges(CancellationToken cancellationToken = default) =>
        await dbContext.SaveChangesAsync(cancellationToken);
}
