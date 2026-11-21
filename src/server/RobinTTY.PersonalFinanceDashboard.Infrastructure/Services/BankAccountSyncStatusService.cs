using Microsoft.EntityFrameworkCore;
using RobinTTY.PersonalFinanceDashboard.Core.Extensions;
using RobinTTY.PersonalFinanceDashboard.Core.Models;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Services;

/// <summary>
/// Determines how up to date bank accounts are with the data providers they originate from.
/// </summary>
public class BankAccountSyncStatusService(ApplicationDbContext dbContext, TimeProvider timeProvider)
{
    private static readonly IReadOnlyList<ThirdPartyDataType> AccountDataTypes =
        [ThirdPartyDataType.BankAccounts, ThirdPartyDataType.Transactions];

    /// <summary>
    /// Gets the sync status of the given bank accounts.
    /// </summary>
    /// <param name="accountIds">The ids of the bank accounts.</param>
    /// <param name="cancellationToken">A token to signal cancellation.</param>
    /// <returns>The sync status of each existing account, keyed by its id.</returns>
    public async Task<IReadOnlyDictionary<Guid, SyncStatus>> GetSyncStatuses(IReadOnlyCollection<Guid> accountIds,
        CancellationToken cancellationToken = default)
    {
        var accounts = await dbContext.BankAccounts
            .AsNoTracking()
            .Include(account => account.AssociatedAuthenticationRequests)
            .Where(account => accountIds.Contains(account.Id!.Value))
            .ToListAsync(cancellationToken);

        var metadataByAccount = (await dbContext.ThirdPartyDataRetrievalMetadata
                .AsNoTracking()
                .Where(metadata => metadata.SynchronizationEntityId != null &&
                                   accountIds.Contains(metadata.SynchronizationEntityId.Value) &&
                                   AccountDataTypes.Contains(metadata.DataType))
                .ToListAsync(cancellationToken))
            .ToLookup(metadata => metadata.SynchronizationEntityId!.Value);

        var utcNow = timeProvider.GetUtcNow().UtcDateTime;
        return accounts.ToDictionary(account => account.Id!.Value,
            account => GetSyncStatus(account, metadataByAccount[account.Id!.Value].ToList(), utcNow));
    }

    private static SyncStatus GetSyncStatus(BankAccount account,
        IReadOnlyCollection<ThirdPartyDataRetrievalMetadata> metadata, DateTime utcNow)
    {
        if (account.Source == DataSource.Manual)
            return new SyncStatus(SyncState.NotSynced);

        // The account is only up to date once both its details and its transactions were retrieved
        DateTime? lastSuccessfulSync = metadata.Count == AccountDataTypes.Count &&
                                       metadata.All(m => m.LastRetrievalTime > DateTime.MinValue)
            ? metadata.Min(m => m.LastRetrievalTime)
            : null;

        // Accounts without an active authentication aren't synchronized anymore
        if (!account.AssociatedAuthenticationRequests.Any(request => request.IsActive(utcNow)))
            return new SyncStatus(SyncState.NeedsReauthentication, lastSuccessfulSync);

        var failed = metadata
            .Where(m => m.NextRetryTime != null)
            .OrderBy(m => m.NextRetryTime)
            .FirstOrDefault();

        return failed is null
            ? new SyncStatus(SyncState.UpToDate, lastSuccessfulSync)
            : new SyncStatus(SyncState.Retrying, lastSuccessfulSync, failed.LastError, failed.NextRetryTime);
    }
}