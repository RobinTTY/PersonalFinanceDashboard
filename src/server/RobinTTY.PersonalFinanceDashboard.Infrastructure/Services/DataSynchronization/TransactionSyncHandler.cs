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
}