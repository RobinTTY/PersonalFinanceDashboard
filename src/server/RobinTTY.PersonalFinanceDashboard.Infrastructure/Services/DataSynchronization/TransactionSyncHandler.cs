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
    ILogger<TransactionSyncHandler> logger) : ITransactionSyncHandler
{
    public async Task<bool> SynchronizeData(Guid? internalAccountId = null, bool forceThirdPartySync = false)
    {
        var transactions = new List<Transaction>();
        var syncedAccountIds = new List<Guid>();

        if (internalAccountId.HasValue)
        {
            var dataIsStale =
                await dataRetrievalMetadataService.DataIsStale(ThirdPartyDataType.Transactions, internalAccountId);
            if (!dataIsStale && !forceThirdPartySync)
            {
                logger.LogDebug(
                    "Transaction data for account with id {accountId} is not stale. Skipping synchronization with third party.",
                    internalAccountId);
            }
            else
            {
                var transactionsForAccount = await FetchTransactionsForAccount(internalAccountId.Value);
                if (transactionsForAccount == null)
                {
                    logger.LogWarning("Failed to fetch transactions from third party API.");
                    return false;
                }

                transactions.AddRange(transactionsForAccount);
                syncedAccountIds.Add(internalAccountId.Value);
            }
        }
        else
        {
            var accountIds = dbContext.BankAccounts
                .Where(account => account.AssociatedAuthenticationRequests.Any(request =>
                    request.Status == AuthenticationStatus.Active &&
                    request.CreatedAt > DateTime.UtcNow.AddDays(-90)))
                .Select(account => account.Id)
                .Where(id => id.HasValue)
                .Cast<Guid>()
                .ToList();

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
                if (transactionsForAccount != null)
                {
                    transactions.AddRange(transactionsForAccount);
                    syncedAccountIds.Add(accountId);
                }
            }
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
        foreach (var updatedTransaction in updatedTransactions)
        {
            var existingTransaction = dbContext.Transactions
                .Include(t => t.BankAccount)
                .SingleOrDefault(t => t.ThirdPartyId == updatedTransaction.ThirdPartyId);

            if (existingTransaction == null)
            {
                var insertEntity = Transaction.CreateWithoutNavigationProperties(updatedTransaction);
                var entry = await dbContext.Transactions.AddAsync(insertEntity);
                existingTransaction = entry.Entity;
            }
            else
            {
                // TODO: Do we ever expect transactions to change after they have been retrieved?
                existingTransaction.UpdateNonNavigationProperties(updatedTransaction);
            }

            await UpdateAssociatedBankAccount(updatedTransaction, existingTransaction);
            await dbContext.SaveChangesAsync();
        }
    }

    private async Task UpdateAssociatedBankAccount(Transaction updatedTransaction, Transaction existingTransaction)
    {
        var associatedBankAccountId = updatedTransaction.BankAccount?.ThirdPartyId;

        if (associatedBankAccountId != null)
        {
            var trackedBankAccount = dbContext.BankAccounts
                .SingleOrDefault(bankAccount => bankAccount.ThirdPartyId == associatedBankAccountId);

            if (trackedBankAccount == null && updatedTransaction.BankAccount != null)
            {
                var entry = await dbContext.BankAccounts.AddAsync(updatedTransaction.BankAccount);
                trackedBankAccount = entry.Entity;
            }

            var associationDoesNotExistYet =
                existingTransaction.BankAccount?.ThirdPartyId != trackedBankAccount?.ThirdPartyId;
            if (associationDoesNotExistYet)
            {
                existingTransaction.BankAccount = trackedBankAccount;
            }
        }
    }
}