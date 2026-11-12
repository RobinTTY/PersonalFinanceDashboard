using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Services.DataSynchronization.Interfaces;
using RobinTTY.PersonalFinanceDashboard.ThirdPartyDataProviders;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Services.DataSynchronization;

public class TransactionSyncHandler(
    ApplicationDbContext dbContext,
    GoCardlessDataProviderService dataProvider,
    ThirdPartyDataRetrievalMetadataService dataRetrievalMetadataService,
    TimeProvider timeProvider,
    ILogger<TransactionSyncHandler> logger) : ITransactionSyncHandler
{
    public async Task<bool> SynchronizeData(Guid? internalAccountId = null, bool forceThirdPartySync = false)
    {
        var transactions = new List<Transaction>();
        var syncedAccountIds = new List<Guid>();

        var accountIds = internalAccountId.HasValue
            ? [internalAccountId.Value]
            : GetActiveAccountIds();

        foreach (var accountId in accountIds)
        {
            var dataIsStale =
                await dataRetrievalMetadataService.DataIsStale(ThirdPartyDataType.Transactions, accountId);
            if (!dataIsStale && !forceThirdPartySync)
            {
                logger.LogDebug(
                    "Transaction data for account with id {accountId} is not stale. Skipping synchronization with third party.",
                    accountId);
                continue;
            }

            var transactionsForAccount = await FetchTransactionsForAccount(accountId);
            if (transactionsForAccount == null)
            {
                // Only the explicitly-requested single account aborts the whole sync on failure.
                if (internalAccountId.HasValue)
                {
                    logger.LogWarning("Failed to fetch transactions from third party API.");
                    return false;
                }

                continue;
            }

            transactions.AddRange(transactionsForAccount);
            syncedAccountIds.Add(accountId);
        }

        if (syncedAccountIds.Count == 0)
            return true;

        await AddOrUpdateTransactions(transactions);
        foreach (var accountId in syncedAccountIds)
        {
            logger.LogDebug("Resetting data expiry for transactions for account with id {accountId}", accountId);
            await dataRetrievalMetadataService.ResetDataExpiry(ThirdPartyDataType.Transactions, accountId);
        }

        logger.LogInformation("Synced {Count} transactions", transactions.Count);

        return true;
    }

    private List<Guid> GetActiveAccountIds()
    {
        var validSince = timeProvider.GetUtcNow().UtcDateTime.AddDays(-90);

        return dbContext.BankAccounts
            .Where(account => account.AssociatedAuthenticationRequests.Any(request =>
                request.Status == AuthenticationStatus.Active &&
                request.CreatedAt > validSince))
            .Select(account => account.Id)
            .Where(id => id.HasValue)
            .Cast<Guid>()
            .ToList();
    }

    private async Task<List<Transaction>?> FetchTransactionsForAccount(Guid internalAccountId)
    {
        var thirdPartyAccountId =
            dbContext.BankAccounts.SingleOrDefault(account => account.Id == internalAccountId)?.ThirdPartyId;
        if (thirdPartyAccountId == null)
            return null;

        var response = await dataProvider.GetTransactions(thirdPartyAccountId.Value);
        if (!response.IsSuccessful)
        {
            logger.LogWarning(
                "Encountered error while getting transactions for account with id {accountId}. Summary: {errorSummary} Detail: {errorDetail}",
                internalAccountId, response.Error.Summary, response.Error.Detail);
            return null;
        }

        return response.Result.ToList();
    }

    private async Task AddOrUpdateTransactions(List<Transaction> updatedTransactions)
    {
        var accountIds = updatedTransactions
            .Select(transaction => transaction.BankAccount?.ThirdPartyId)
            .OfType<Guid>()
            .Distinct()
            .ToList();
        var accounts = await dbContext.BankAccounts
            .Where(account => accountIds.Contains(account.ThirdPartyId))
            .ToDictionaryAsync(account => account.ThirdPartyId);

        // Keyed like the unique index, since third party ids are only unique per account
        var transactionIds = updatedTransactions.Select(transaction => transaction.ThirdPartyId).Distinct().ToList();
        var existingTransactions = await dbContext.Transactions
            .Include(transaction => transaction.BankAccount)
            .Where(transaction => transactionIds.Contains(transaction.ThirdPartyId) &&
                                  transaction.BankAccount != null &&
                                  accountIds.Contains(transaction.BankAccount.ThirdPartyId))
            .ToDictionaryAsync(transaction => (transaction.BankAccount!.ThirdPartyId, transaction.ThirdPartyId));

        foreach (var updatedTransaction in updatedTransactions)
        {
            if (updatedTransaction.BankAccount == null ||
                !accounts.TryGetValue(updatedTransaction.BankAccount.ThirdPartyId, out var account))
            {
                logger.LogWarning("Skipping transaction {transactionId} without a known bank account.",
                    updatedTransaction.ThirdPartyId);
                continue;
            }

            var key = (account.ThirdPartyId, updatedTransaction.ThirdPartyId);
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