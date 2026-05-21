using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RobinTTY.PersonalFinanceDashboard.Core.Extensions;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Services.DataSynchronization.Interfaces;
using RobinTTY.PersonalFinanceDashboard.ThirdPartyDataProviders;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Services.DataSynchronization;

public class BankAccountSyncHandler(
    ApplicationDbContext dbContext,
    GoCardlessDataProviderService dataProvider,
    ThirdPartyDataRetrievalMetadataService dataRetrievalMetadataService,
    ILogger<BankAccountSyncHandler> logger) : IBankAccountSyncHandler
{
    /// <summary>
    /// Synchronizes stale bank account data by fetching it from a third-party API and updating the local database. If a specific bank account ID is provided, synchronization is limited to that account; otherwise, all accounts are synchronized.
    /// </summary>
    /// <param name="internalBankAccountId">The internal ID of a specific bank account to synchronize. If <see langword="null"/>, all bank accounts will be synchronized.</param>
    /// <param name="forceThirdPartySync">Indicates whether to force synchronization with the third-party API.</param>
    /// <returns>A boolean value indicating whether the synchronization process was successful.</returns>
    public async Task<bool> SynchronizeData(Guid? internalBankAccountId = null, bool forceThirdPartySync = false)
    {
        Guid? thirdPartyBankAccountId = null;

        // If a specific bank account id is provided, we only want to fetch and update that account. In this case, we need to get the corresponding third party id to fetch the data from the API.
        if (internalBankAccountId.HasValue)
            thirdPartyBankAccountId = dbContext.BankAccounts
                .SingleOrDefault(bankAccount => bankAccount.Id == internalBankAccountId)?.ThirdPartyId;

        // Fetch bank accounts from third party
        var bankAccounts =
            await FetchBankAccountsFromApi(internalBankAccountId, thirdPartyBankAccountId, forceThirdPartySync);
        if (bankAccounts == null)
        {
            logger.LogWarning("Failed to fetch bank accounts from third party API.");
            return false;
        }

        var updatedBankAccounts = await AddOrUpdateBankAccounts(bankAccounts);
        foreach (var bankAccount in updatedBankAccounts)
        {
            logger.LogDebug("Resetting data expiry for bank account with id {bankAccountId}", bankAccount.Id);
            await dataRetrievalMetadataService.ResetDataExpiry(ThirdPartyDataType.BankAccounts, bankAccount.Id);
        }

        logger.LogInformation("Synced {Count} bank accounts", bankAccounts.Count);

        return true;
    }

    /// <summary>
    /// Fetches bank accounts from the third party API. If a specific bank account id is provided, only that account will be fetched. Otherwise, all bank accounts associated with active authentication requests that are not older than 90 days will be fetched.
    /// </summary>
    /// <param name="bankAccountId">The internal id of the bank account to fetch.</param>
    /// <param name="thirdPartyBankAccountId">The id of the bank account to fetch. If <see langword="null"/>, all bank accounts associated with active authentication requests that are not older than 90 days will be fetched.</param>
    /// <param name="forceSync">Indicates whether to force synchronization with the third party API.</param>
    /// <returns></returns>
    private async Task<List<BankAccount>?> FetchBankAccountsFromApi(Guid? bankAccountId, Guid? thirdPartyBankAccountId,
        bool forceSync)
    {
        var accountsToFetch = await DetermineStaleBankAccounts(bankAccountId, thirdPartyBankAccountId, forceSync);
        var responses = await dataProvider.GetBankAccounts(accountsToFetch);

        var bankAccounts = new List<BankAccount>();
        foreach (var response in responses)
        {
            if (response.IsSuccessful)
            {
                bankAccounts.Add(response.Result);
            }
            else
            {
                logger.LogWarning(
                    "Encountered error while getting bank account. Summary: {errorSummary} Detail: {errorDetail}",
                    response.Error.Summary, response.Error.Detail);
            }
        }

        return bankAccounts;
    }

    /// <summary>
    /// Identifies stale bank accounts that require synchronization. If a third-party bank account ID is specified, checks whether its data is stale or a force synchronization is required.
    /// If no specific account is specified, evaluates all associated bank accounts to determine their synchronization status.
    /// Currency information is used as an indicator of whether metadata needs to be fetched for the account, as this information requires additional API calls to be retrieved but doesn't usually change.
    /// </summary>
    /// <param name="bankAccountId">The internal ID of a specific bank account to evaluate for staleness. If <see langword="null"/>, all associated bank accounts are considered.</param>
    /// <param name="thirdPartyBankAccountId">The external ID of a specific third-party bank account to evaluate for staleness. If <see langword="null"/>, this parameter is ignored.</param>
    /// <param name="forceSync">Indicates whether synchronization should be forced regardless of staleness status.</param>
    /// <returns>A dictionary where the key is the GUID of a bank account and the value indicates whether it requires synchronization.</returns>
    private async Task<Dictionary<Guid, bool>> DetermineStaleBankAccounts(Guid? bankAccountId,
        Guid? thirdPartyBankAccountId, bool forceSync)
    {
        var accountsToFetch = new Dictionary<Guid, bool>();

        if (thirdPartyBankAccountId.HasValue)
        {
            var dataIsStale =
                await dataRetrievalMetadataService.DataIsStale(ThirdPartyDataType.BankAccounts, bankAccountId);
            if (!dataIsStale && !forceSync)
            {
                logger.LogDebug(
                    "Bank account data for account with id {bankAccountId} is not stale. Skipping synchronization with third party.",
                    bankAccountId);
            }
            else
            {
                var account =
                    dbContext.BankAccounts.FirstOrDefault(account =>
                        account.ThirdPartyId == thirdPartyBankAccountId.Value);
                accountsToFetch = new Dictionary<Guid, bool>
                {
                    { thirdPartyBankAccountId.Value, account?.Currency == null }
                };
            }
        }
        else
        {
            var authRequests = dbContext.AuthenticationRequests
                .Include(authenticationRequest => authenticationRequest.AssociatedAccounts).ToList();

            foreach (var authenticationRequest in authRequests)
            {
                if (!authenticationRequest.IsActive()) continue;

                var accountIdsIncludeMetadataPairs = authenticationRequest.AssociatedAccounts
                    .Select(acc => (internalId: acc.Id, id: acc.ThirdPartyId, includeMetadata: acc.Currency == null))
                    .DistinctBy(pair => pair.id);

                foreach (var pair in accountIdsIncludeMetadataPairs)
                {
                    var dataIsStale =
                        await dataRetrievalMetadataService.DataIsStale(ThirdPartyDataType.BankAccounts,
                            pair.internalId);
                    if (dataIsStale || forceSync)
                    {
                        accountsToFetch.Add(pair.id, pair.includeMetadata);
                    }
                    else
                    {
                        logger.LogDebug(
                            "Bank account data for account with id {bankAccountId} is not stale. Skipping synchronization with third party.",
                            pair.internalId);
                    }
                }
            }
        }

        return accountsToFetch;
    }

    /// <summary>
    /// Adds new bank accounts or updates existing ones in the database based on the provided list of bank accounts.
    /// If a bank account with a matching third party identifier exists, its information will be updated; otherwise,
    /// a new bank account record will be created.
    /// </summary>
    /// <param name="bankAccounts">The list of bank accounts to add or update in the database.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of bank accounts
    /// that were added or updated in the database.</returns>
    private async Task<List<BankAccount>> AddOrUpdateBankAccounts(List<BankAccount> bankAccounts)
    {
        var updatedBankAccounts = new List<BankAccount>();

        foreach (var updatedBankAccount in bankAccounts)
        {
            var existingBankAccount = dbContext.BankAccounts
                .Include(account => account.AssociatedAuthenticationRequests)
                .Include(account => account.AssociatedInstitution)
                .SingleOrDefault(account => account.ThirdPartyId == updatedBankAccount.ThirdPartyId);

            if (existingBankAccount == null)
            {
                var insertEntity = BankAccount.CreateWithoutNavigationProperties(updatedBankAccount);
                var entry = await dbContext.BankAccounts.AddAsync(insertEntity);
                existingBankAccount = entry.Entity;
            }
            else
            {
                existingBankAccount.UpdateNonNavigationProperties(updatedBankAccount);
                logger.LogInformation(
                    "Updated existing bank account {name} with id {bankAccountId}. New balance: {balance}.",
                    existingBankAccount.Name, existingBankAccount.Id, updatedBankAccount.Balance);
            }

            await UpdateAssociatedAuthenticationRequests(updatedBankAccount, existingBankAccount);
            await UpdateAssociatedInstitution(updatedBankAccount, existingBankAccount);
            updatedBankAccounts.Add(existingBankAccount);

            await dbContext.SaveChangesAsync();
        }

        return updatedBankAccounts;
    }

    /// <summary>
    /// Updates the authentication requests associated with an existing bank account based on the authentication requests of the updated bank account.
    /// </summary>
    /// <param name="updatedBankAccount">The bank account containing the updated authentication requests.</param>
    /// <param name="existingBankAccount">The existing bank account to which the updated authentication requests will be linked.</param>
    private async Task UpdateAssociatedAuthenticationRequests(BankAccount updatedBankAccount,
        BankAccount existingBankAccount)
    {
        var associatedAuthenticationRequestIds = updatedBankAccount.AssociatedAuthenticationRequests
            .Select(req => req.ThirdPartyId);
        var authRequests = dbContext.AuthenticationRequests
            .Where(request => associatedAuthenticationRequestIds.Contains(request.ThirdPartyId)).ToList();

        foreach (var request in updatedBankAccount.AssociatedAuthenticationRequests)
        {
            var linkedRequest = authRequests.SingleOrDefault(existingRequest =>
                existingRequest.ThirdPartyId == request.ThirdPartyId);

            if (linkedRequest == null)
            {
                var entry = await dbContext.AuthenticationRequests.AddAsync(request);
                linkedRequest = entry.Entity;
                existingBankAccount.AssociatedAuthenticationRequests.Add(request);
            }

            var associationAlreadyExists = existingBankAccount.AssociatedAuthenticationRequests
                .All(req => req.ThirdPartyId != linkedRequest.ThirdPartyId);
            if (associationAlreadyExists)
            {
                existingBankAccount.AssociatedAuthenticationRequests.Add(linkedRequest);
            }
        }
    }

    /// <summary>
    /// Updates the associated institution of the bank account if it has changed. If the institution does not exist in the database, it will be added.
    /// </summary>
    /// <param name="updatedBankAccount">The bank account with the updated data.</param>
    /// <param name="existingBankAccount">The existing bank account in the database.</param>
    private async Task UpdateAssociatedInstitution(BankAccount updatedBankAccount,
        BankAccount existingBankAccount)
    {
        var associatedInstitutionId = updatedBankAccount.AssociatedInstitution?.ThirdPartyId;

        if (associatedInstitutionId != null)
        {
            var trackedInstitution = dbContext.BankingInstitutions
                .SingleOrDefault(institution => institution.ThirdPartyId == associatedInstitutionId);

            if (trackedInstitution == null && updatedBankAccount.AssociatedInstitution != null)
            {
                var entry = await dbContext.BankingInstitutions.AddAsync(updatedBankAccount.AssociatedInstitution);
                trackedInstitution = entry.Entity;
            }

            var associationDoesNotExistYet =
                existingBankAccount.AssociatedInstitution?.ThirdPartyId != trackedInstitution?.ThirdPartyId;
            if (associationDoesNotExistYet)
            {
                existingBankAccount.AssociatedInstitution = trackedInstitution;
            }
        }
    }
}