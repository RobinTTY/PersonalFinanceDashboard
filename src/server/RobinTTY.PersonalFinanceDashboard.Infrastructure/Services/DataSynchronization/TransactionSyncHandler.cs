using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RobinTTY.PersonalFinanceDashboard.Core.DataProviders;
using RobinTTY.PersonalFinanceDashboard.Core.Extensions;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Services.DataSynchronization.Interfaces;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Services.DataSynchronization;

/// <summary>
/// Synchronizes the transactions of bank accounts with the data providers the accounts originate from.
/// </summary>
public class TransactionSyncHandler(
    ApplicationDbContext dbContext,
    BankDataProviderRegistry providers,
    ThirdPartyDataRetrievalMetadataService dataRetrievalMetadataService,
    TimeProvider timeProvider,
    DataSynchronizationLock synchronizationLock,
    ILogger<TransactionSyncHandler> logger) : ITransactionSyncHandler
{
    private sealed record SyncedAccount(Guid Id, DataSource Source, string ExternalId);

    /// <summary>
    /// Identifies a transaction across connections, whose external ids differ.
    /// </summary>
    private readonly record struct ContentKey(string? BankTransactionId, decimal Amount, string? Payer, string? Payee)
    {
        public static ContentKey Of(Transaction transaction) => string.IsNullOrEmpty(transaction.BankTransactionId)
            ? new ContentKey(null, transaction.Amount, transaction.Payer, transaction.Payee)
            : new ContentKey(transaction.BankTransactionId, 0, null, null);
    }

    public async Task<bool> SynchronizeData(Guid? internalAccountId = null, bool forceThirdPartySync = false)
    {
        using var acquiredLock = await synchronizationLock.Acquire();
        if (internalAccountId.HasValue)
        {
            var account = await dbContext.BankAccounts.SingleOrDefaultAsync(a => a.Id == internalAccountId);
            if (account == null)
            {
                logger.LogWarning("Bank account with id {accountId} does not exist.", internalAccountId);
                return false;
            }

            // Accounts without a provider (e.g. manually created ones) aren't synchronized
            if (!providers.TryGet(account.Source, out var provider))
                return true;

            // Only the explicitly requested single account fails the whole sync on failure
            return await SynchronizeAccounts(provider,
                [new SyncedAccount(internalAccountId.Value, account.Source, account.ExternalId!)],
                forceThirdPartySync, abortOnFailure: true);
        }

        foreach (var accounts in (await GetActiveAccounts()).GroupBy(account => account.Source))
        {
            if (providers.TryGet(accounts.Key, out var provider))
                await SynchronizeAccounts(provider, accounts.ToList(), forceThirdPartySync, abortOnFailure: false);
        }

        return true;
    }

    private async Task<bool> SynchronizeAccounts(IBankDataProvider provider, List<SyncedAccount> accounts,
        bool forceThirdPartySync, bool abortOnFailure)
    {
        var transactions = new List<Transaction>();
        var syncedAccountIds = new List<Guid>();

        foreach (var account in accounts)
        {
            var retrievalIsDue = await dataRetrievalMetadataService.IsRetrievalDue(provider.Source,
                ThirdPartyDataType.Transactions, account.Id);
            if (!retrievalIsDue && !forceThirdPartySync)
            {
                logger.LogDebug(
                    "Transaction data for account with id {accountId} is not stale or waiting to be retried. Skipping synchronization with third party.",
                    account.Id);
                continue;
            }

            var retrieveFrom = await dataRetrievalMetadataService.GetRetrieveFrom(provider.Source,
                ThirdPartyDataType.Transactions, account.Id);
            var response = await provider.GetTransactions(account.ExternalId, retrieveFrom);
            if (!response.IsSuccessful)
            {
                var nextRetryTime = await dataRetrievalMetadataService.RecordFailedRetrieval(provider.Source,
                    ThirdPartyDataType.Transactions, account.Id, response.Error);
                logger.LogWarning(
                    "Encountered error while getting transactions for account with id {accountId} from {dataSource}, retrying after {nextRetryTime}. Summary: {errorSummary} Detail: {errorDetail}",
                    account.Id, provider.Source, nextRetryTime, response.Error.Summary, response.Error.Detail);
                if (abortOnFailure)
                    return false;

                continue;
            }

            transactions.AddRange(retrieveFrom.HasValue
                ? await RemoveAlreadyRetrieved(account.Id, retrieveFrom.Value, response.Result)
                : response.Result);
            syncedAccountIds.Add(account.Id);
        }

        if (syncedAccountIds.Count == 0)
            return true;

        await AddOrUpdateTransactions(provider.Source, transactions);
        foreach (var accountId in syncedAccountIds)
        {
            logger.LogDebug("Resetting data expiry for transactions for account with id {accountId}", accountId);
            await dataRetrievalMetadataService.ResetDataExpiry(provider.Source, ThirdPartyDataType.Transactions,
                accountId);
        }

        logger.LogInformation("Synced {Count} transactions from {dataSource}", transactions.Count, provider.Source);

        return true;
    }

    private async Task<List<SyncedAccount>> GetActiveAccounts()
    {
        var validSince = timeProvider.GetUtcNow().UtcDateTime - AuthenticationRequestExtensions.AccessValidity;

        var accounts = await dbContext.BankAccounts
            .Where(account => account.ExternalId != null &&
                              account.AssociatedAuthenticationRequests.Any(request =>
                                  request.Status == AuthenticationStatus.Active &&
                                  request.CreatedAt > validSince))
            .Select(account => new { account.Id, account.Source, account.ExternalId })
            .ToListAsync();

        return accounts.Select(account => new SyncedAccount(account.Id!.Value, account.Source, account.ExternalId!))
            .ToList();
    }

    /// <summary>
    /// Removes the transactions of a reconnected account that were already retrieved through the earlier
    /// connection, under different ids. Only transactions from the day the account was taken over are retrieved
    /// again, so the ones on that day are matched by their content.
    /// </summary>
    /// <param name="accountId">The id of the account.</param>
    /// <param name="retrieveFrom">The day from which the account's transactions are retrieved.</param>
    /// <param name="fetchedTransactions">The transactions retrieved from the data source.</param>
    /// <returns>The transactions that weren't retrieved before.</returns>
    private async Task<List<Transaction>> RemoveAlreadyRetrieved(Guid accountId, DateTime retrieveFrom,
        IReadOnlyList<Transaction> fetchedTransactions)
    {
        var fetchedIds = fetchedTransactions.Select(transaction => transaction.ExternalId!).ToList();
        var earlierTransactions = await dbContext.Transactions
            .Where(transaction => transaction.BankAccount!.Id == accountId &&
                                  transaction.ValueDate >= retrieveFrom &&
                                  transaction.ValueDate < retrieveFrom.AddDays(1) &&
                                  !fetchedIds.Contains(transaction.ExternalId!))
            .ToListAsync();
        // Counted, so a transaction that occurs twice on that day is only removed as often as it was retrieved
        var remaining = earlierTransactions.GroupBy(ContentKey.Of)
            .ToDictionary(group => group.Key, group => group.Count());

        var newTransactions = new List<Transaction>();
        foreach (var transaction in fetchedTransactions)
        {
            var valueDate = transaction.ValueDate?.Date;
            if (valueDate < retrieveFrom)
                continue;

            var key = ContentKey.Of(transaction);
            if (valueDate == retrieveFrom && remaining.TryGetValue(key, out var count) && count > 0)
            {
                remaining[key] = count - 1;
                continue;
            }

            newTransactions.Add(transaction);
        }

        return newTransactions;
    }

    private async Task AddOrUpdateTransactions(DataSource source, List<Transaction> updatedTransactions)
    {
        var accountIds = updatedTransactions
            .Select(transaction => transaction.BankAccount?.ExternalId)
            .OfType<string>()
            .Distinct()
            .ToList();
        var accounts = await dbContext.BankAccounts
            .Where(account => account.Source == source && accountIds.Contains(account.ExternalId!))
            .ToDictionaryAsync(account => account.ExternalId!);

        // Keyed like the unique index, since external transaction ids are only unique per account
        var transactionIds = updatedTransactions.Select(transaction => transaction.ExternalId!).Distinct().ToList();
        var existingTransactions = await dbContext.Transactions
            .Include(transaction => transaction.BankAccount)
            .Where(transaction => transaction.Source == source &&
                                  transactionIds.Contains(transaction.ExternalId!) &&
                                  transaction.BankAccount != null &&
                                  transaction.BankAccount.Source == source &&
                                  accountIds.Contains(transaction.BankAccount.ExternalId!))
            .ToDictionaryAsync(transaction => (transaction.BankAccount!.ExternalId!, transaction.ExternalId!));

        foreach (var updatedTransaction in updatedTransactions)
        {
            if (updatedTransaction.BankAccount == null ||
                !accounts.TryGetValue(updatedTransaction.BankAccount.ExternalId!, out var account))
            {
                logger.LogWarning("Skipping transaction {transactionId} without a known bank account.",
                    updatedTransaction.ExternalId);
                continue;
            }

            var key = (account.ExternalId!, updatedTransaction.ExternalId!);
            if (existingTransactions.TryGetValue(key, out var existingTransaction))
            {
                // TODO: Do we ever expect transactions to change after they have been retrieved?
                existingTransaction.UpdateNonNavigationProperties(updatedTransaction);
            }
            else
            {
                var insertEntity = Transaction.CreateWithoutNavigationProperties(updatedTransaction);
                insertEntity.BankAccount = account;
                dbContext.Transactions.Add(insertEntity);
                existingTransactions[key] = insertEntity;
            }
        }

        await dbContext.SaveChangesAsync();
    }
}