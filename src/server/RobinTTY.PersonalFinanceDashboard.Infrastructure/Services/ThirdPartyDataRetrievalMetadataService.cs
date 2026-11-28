using Microsoft.Extensions.Options;
using RobinTTY.PersonalFinanceDashboard.Core.DataProviders;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Repositories;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Services;

/// <summary>
/// Service used to manage the data retrieval from third party services. It is used to
/// make sure data is only retrieved once it has gone stale as specified by the configured data
/// retrieval interval, and that failed retrievals are retried with an increasing delay.
/// </summary>
/// <param name="thirdPartyDataRetrievalMetadataRepository">The repository which holds the
/// <see cref="ThirdPartyDataRetrievalMetadata"/> used to determine when data is stale.</param>
/// <param name="configuration">The intervals at which data is retrieved again.</param>
/// <param name="timeProvider">Provides the current time.</param>
public class ThirdPartyDataRetrievalMetadataService(
    ThirdPartyDataRetrievalMetadataRepository thirdPartyDataRetrievalMetadataRepository,
    IOptions<DataRetrievalConfiguration> configuration,
    TimeProvider timeProvider)
{
    /// <summary>
    /// The delay before the first retry of a failed retrieval. It doubles with every further failure.
    /// </summary>
    public static readonly TimeSpan InitialRetryDelay = TimeSpan.FromHours(1);

    /// <summary>
    /// The number of retrievals recorded through this instance, successful or not.
    /// </summary>
    public int RecordedRetrievals { get; private set; }

    /// <summary>
    /// Checks whether the data of the given <see cref="ThirdPartyDataType"/> and data source should be retrieved,
    /// either because it has gone stale or because a failed retrieval is due to be retried.
    /// </summary>
    /// <param name="dataSource">The data source the data is retrieved from.</param>
    /// <param name="thirdPartyDataType">The data type for which to check.</param>
    /// <param name="synchronizationEntityId">Optional id of the synchronization entity for which to check.</param>
    /// <param name="cancellationToken">A token to signal cancellation.</param>
    /// <returns><see langword="true"/> if the data should be retrieved, <see langword="false"/> otherwise.</returns>
    public async Task<bool> IsRetrievalDue(DataSource dataSource, ThirdPartyDataType thirdPartyDataType,
        Guid? synchronizationEntityId = null, CancellationToken cancellationToken = default)
    {
        var retrievalMetadata = await thirdPartyDataRetrievalMetadataRepository
            .GetThirdPartyDataRetrievalMetadata(dataSource, thirdPartyDataType, synchronizationEntityId,
                cancellationToken);

        if (retrievalMetadata is null)
        {
            return true;
        }

        var nextRetrievalTime = retrievalMetadata.NextRetryTime ?? retrievalMetadata.LastRetrievalTime +
            configuration.Value.GetRetrievalInterval(thirdPartyDataType);
        return nextRetrievalTime <= timeProvider.GetUtcNow().UtcDateTime;
    }

    /// <summary>
    /// Records a successful retrieval of the given <see cref="ThirdPartyDataType"/> and data source: the data
    /// expires again after its retrieval interval, and earlier failures are cleared.
    /// </summary>
    /// <param name="dataSource">The data source the data was retrieved from.</param>
    /// <param name="thirdPartyDataType">The data type for which to reset the data expiry.</param>
    /// <param name="synchronizationEntityId">Optional id of the synchronization entity for which to reset the data expiry.</param>
    /// <param name="cancellationToken">A token to signal cancellation.</param>
    public async Task ResetDataExpiry(DataSource dataSource, ThirdPartyDataType thirdPartyDataType,
        Guid? synchronizationEntityId = null, CancellationToken cancellationToken = default)
    {
        var metadata = await thirdPartyDataRetrievalMetadataRepository
            .GetOrAddThirdPartyDataRetrievalMetadata(dataSource, thirdPartyDataType, synchronizationEntityId,
                cancellationToken);

        metadata.LastRetrievalTime = timeProvider.GetUtcNow().UtcDateTime;
        metadata.ConsecutiveFailures = 0;
        metadata.LastError = null;
        metadata.NextRetryTime = null;

        await thirdPartyDataRetrievalMetadataRepository.SaveChanges(cancellationToken);
        RecordedRetrievals++;
    }

    /// <summary>
    /// Records a failed retrieval of the given <see cref="ThirdPartyDataType"/> and data source, so it's only
    /// retried after a delay (see <see cref="GetRetryDelay"/>), or later if the provider asked for it.
    /// </summary>
    /// <param name="dataSource">The data source the data was retrieved from.</param>
    /// <param name="thirdPartyDataType">The data type that failed to be retrieved.</param>
    /// <param name="synchronizationEntityId">Optional id of the synchronization entity that failed to be retrieved.</param>
    /// <param name="error">The error returned by the provider.</param>
    /// <param name="cancellationToken">A token to signal cancellation.</param>
    /// <returns>The earliest time the retrieval is retried.</returns>
    public async Task<DateTime> RecordFailedRetrieval(DataSource dataSource, ThirdPartyDataType thirdPartyDataType,
        Guid? synchronizationEntityId, ThirdPartyError error, CancellationToken cancellationToken = default)
    {
        var metadata = await thirdPartyDataRetrievalMetadataRepository
            .GetOrAddThirdPartyDataRetrievalMetadata(dataSource, thirdPartyDataType, synchronizationEntityId,
                cancellationToken);

        metadata.ConsecutiveFailures++;
        metadata.LastError = error.Message;

        var retryDelay = GetRetryDelay(metadata.ConsecutiveFailures,
            configuration.Value.GetRetrievalInterval(thirdPartyDataType));
        if (error.RetryAfter > retryDelay)
            retryDelay = error.RetryAfter.Value;
        metadata.NextRetryTime = timeProvider.GetUtcNow().UtcDateTime + retryDelay;

        await thirdPartyDataRetrievalMetadataRepository.SaveChanges(cancellationToken);
        RecordedRetrievals++;

        return metadata.NextRetryTime.Value;
    }

    /// <summary>
    /// Gets the delay before retrying a failed retrieval: <see cref="InitialRetryDelay"/>, doubled for every
    /// further consecutive failure, but never longer than the regular retrieval interval.
    /// </summary>
    /// <param name="consecutiveFailures">The number of consecutive failures, at least 1.</param>
    /// <param name="retrievalInterval">The regular retrieval interval of the data.</param>
    public static TimeSpan GetRetryDelay(int consecutiveFailures, TimeSpan retrievalInterval)
    {
        var factor = Math.Pow(2, Math.Clamp(consecutiveFailures - 1, 0, 30));
        return TimeSpan.FromTicks((long)Math.Min(InitialRetryDelay.Ticks * factor, retrievalInterval.Ticks));
    }
}
