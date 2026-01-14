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
    ILogger<BankingInstitutionSyncHandler> logger) : ITransactionSyncHandler
{
    public async Task<bool> SynchronizeData(Guid? internalAccountId = null, bool forceThirdPartySync = false)
    {
        var dataIsStale = await dataRetrievalMetadataService.DataIsStale(ThirdPartyDataType.Transactions);

        if (dataIsStale || forceThirdPartySync)
        {
            var transactions = new List<Transaction>();
            if (internalAccountId.HasValue)
                transactions = await GetTransactions(internalAccountId.Value);
            else
            {
                var accountIds = dbContext.BankAccounts
                    .Select(account => account.Id)
                    .Where(id => id.HasValue)
                    .Cast<Guid>();
                
                foreach (var accountId in accountIds)
                {
                    var transactionForAccount = await GetTransactions(accountId);
                    if (transactionForAccount != null)
                        transactions.AddRange(transactionForAccount);
                }
            }


            if (transactions == null || transactions.Count == 0)
            {
                logger.LogWarning("Could not retrieve transactions from {dataProvider}",
                    nameof(GoCardlessDataProviderService));
                return false;
            }

            await AddNewTransactions(transactions);

            // TODO: Maybe optimize to be able to save sync metadata for individual accounts
            // If we are updating only one account, do not reset the data expiry
            if (!internalAccountId.HasValue)
                await dataRetrievalMetadataService.ResetDataExpiry(ThirdPartyDataType.Transactions);

            logger.LogInformation("Synced {Count} transactions", transactions.Count);
        }

        return true;
    }

    private async Task<List<Transaction>?> GetTransactions(Guid accountId)
    {
        var thirdPartyAccountId =
            dbContext.BankAccounts.SingleOrDefault(account => account.Id == accountId)?.ThirdPartyId;
        if (thirdPartyAccountId == null)
            return null;

        var response = await dataProvider.GetTransactions(accountId, thirdPartyAccountId.Value);
        return response.IsSuccessful ? response.Result.ToList() : null;
    }

    // TODO: Should transactions ever be updated after they are added? For now only add them
    private async Task AddNewTransactions(List<Transaction> transactions)
    {
        var existingIds = dbContext.Transactions.Select(transaction => transaction.ThirdPartyId);
        var newTransactions = transactions.Where(transaction => !existingIds.Contains(transaction.ThirdPartyId));

        // TODO: set account association
        dbContext.Transactions.AddRange(newTransactions);
        await dbContext.SaveChangesAsync();
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