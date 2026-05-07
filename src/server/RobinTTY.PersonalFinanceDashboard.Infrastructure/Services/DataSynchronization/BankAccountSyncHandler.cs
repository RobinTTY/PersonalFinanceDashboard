using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RobinTTY.PersonalFinanceDashboard.Core.Extensions;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.Core.Utility;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Services.DataSynchronization.Interfaces;
using RobinTTY.PersonalFinanceDashboard.ThirdPartyDataProviders;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Services.DataSynchronization;

public class BankAccountSyncHandler(
    ApplicationDbContext dbContext,
    GoCardlessDataProviderService dataProvider,
    ThirdPartyDataRetrievalMetadataService dataRetrievalMetadataService,
    ILogger<BankAccountSyncHandler> logger) : IBankAccountSyncHandler
{
    public async Task<bool> SynchronizeData(Guid? bankAccountId = null, bool forceThirdPartySync = false)
    {
        var dataIsStale = await dataRetrievalMetadataService.DataIsStale(ThirdPartyDataType.BankAccounts);

        if (dataIsStale || forceThirdPartySync)
        {
            if (bankAccountId.HasValue)
                bankAccountId = dbContext.BankAccounts
                    .SingleOrDefault(bankAccount => bankAccount.Id == bankAccountId)?.ThirdPartyId;

            var bankAccounts = await FetchBankAccountsFromApi(bankAccountId);
            if (bankAccounts == null || bankAccounts.Count == 0)
            {
                return false;
            }

            await AddOrUpdateBankAccounts(bankAccounts);

            // If we are updating only one account, do not reset the data expiry
            if (!bankAccountId.HasValue)
                await dataRetrievalMetadataService.ResetDataExpiry(ThirdPartyDataType.BankAccounts);

            logger.LogInformation("Synced {Count} bank accounts", bankAccounts.Count);
        }
        else
        {
            logger.LogDebug("{dataType} data is not stale. Skipping synchronization with third party.", ThirdPartyDataType.BankAccounts);
        }

        return true;
    }

    /// <summary>
    /// Fetches bank accounts from the third party API. If a specific bank account id is provided, only that account will be fetched. Otherwise, all bank accounts associated with active authentication requests that are not older than 90 days will be fetched.
    /// </summary>
    /// <param name="thirdPartyBankAccountId">The id of the bank account to fetch. If <see langword="null"/>, all bank accounts associated with active authentication requests that are not older than 90 days will be fetched.</param>
    /// <returns></returns>
    private async Task<List<BankAccount>?> FetchBankAccountsFromApi(Guid? thirdPartyBankAccountId)
    {
        // Currency is the only mandatory field returned by the API when fetching metadata. If it is currently null, we need to fetch the metadata for this account.
        var accountsToFetch = new Dictionary<Guid, bool>();

        if (thirdPartyBankAccountId.HasValue)
        {
            var account =
                dbContext.BankAccounts.FirstOrDefault(account => account.ThirdPartyId == thirdPartyBankAccountId.Value);
            accountsToFetch = new Dictionary<Guid, bool>
            {
                { thirdPartyBankAccountId.Value, account?.Currency == null }
            };
        }
        else
        {
            var authRequests = dbContext.AuthenticationRequests
                .Include(authenticationRequest => authenticationRequest.AssociatedAccounts).ToList();

            foreach (var authenticationRequest in authRequests)
            {
                if (authenticationRequest.IsActive())
                {
                    var accountIdsIncludeMetadataPairs = authenticationRequest.AssociatedAccounts
                        .Select(acc => (id: acc.ThirdPartyId, includeMetadata: acc.Currency == null))
                        .DistinctBy(pair => pair.id);

                    foreach (var pair in accountIdsIncludeMetadataPairs)
                    {
                        accountsToFetch.Add(pair.id, pair.includeMetadata);
                    }
                }
            }
        }

        var bankAccounts = new List<BankAccount>();
        var responses = await dataProvider.GetBankAccounts(accountsToFetch);
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

    private async Task AddOrUpdateBankAccounts(List<BankAccount> bankAccounts)
    {
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
            await dbContext.SaveChangesAsync();
        }
    }

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