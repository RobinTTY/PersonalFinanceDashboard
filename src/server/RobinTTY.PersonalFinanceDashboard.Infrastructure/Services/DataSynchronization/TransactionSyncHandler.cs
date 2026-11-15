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
        List<(Guid Id, string ExternalId)> accounts;
        if (internalAccountId.HasValue)
        {
            var account = await dbContext.BankAccounts.SingleOrDefaultAsync(a => a.Id == internalAccountId);
            if (account == null)
            {
                logger.LogWarning("Bank account with id {accountId} does not exist.", internalAccountId);
                return false;
            }

            if (account.Source != DataSource.GoCardless)
                return true;

            accounts = [(internalAccountId.Value, account.ExternalId!)];
        }
        else
        {
            accounts = await GetActiveAccounts();
        }

        var transactions = new List<Transaction>();
        var syncedAccountIds = new List<Guid>();

        foreach (var (accountId, externalAccountId) in accounts)
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

            var transactionsForAccount = await FetchTransactionsForAccount(accountId, externalAccountId);
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

    private async Task<List<(Guid Id, string ExternalId)>> GetActiveAccounts()
    {
        var validSince = timeProvider.GetUtcNow().UtcDateTime.AddDays(-90);

        var accounts = await dbContext.BankAccounts
            .Where(account => account.Source == DataSource.GoCardless &&
                              account.AssociatedAuthenticationRequests.Any(request =>
                                  request.Status == AuthenticationStatus.Active &&
                                  request.CreatedAt > validSince))
            .Select(account => new { account.Id, account.ExternalId })
            .ToListAsync();

        return accounts.Select(account => (account.Id!.Value, account.ExternalId!)).ToList();
    }

    private async Task<List<Transaction>?> FetchTransactionsForAccount(Guid internalAccountId,
        string externalAccountId)
    {
        var response = await dataProvider.GetTransactions(externalAccountId);
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
            .Select(transaction => transaction.BankAccount?.ExternalId)
            .OfType<string>()
            .Distinct()
            .ToList();
        var accounts = await dbContext.BankAccounts
            .Where(account => account.Source == DataSource.GoCardless && accountIds.Contains(account.ExternalId!))
            .ToDictionaryAsync(account => account.ExternalId!);

        // Keyed like the unique index, since third party ids are only unique per account
        var transactionIds = updatedTransactions.Select(transaction => transaction.ExternalId!).Distinct().ToList();
        var existingTransactions = await dbContext.Transactions
            .Include(transaction => transaction.BankAccount)
            .Where(transaction => transaction.Source == DataSource.GoCardless &&
                                  transactionIds.Contains(transaction.ExternalId!) &&
                                  transaction.BankAccount != null &&
                                  transaction.BankAccount.Source == DataSource.GoCardless &&
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