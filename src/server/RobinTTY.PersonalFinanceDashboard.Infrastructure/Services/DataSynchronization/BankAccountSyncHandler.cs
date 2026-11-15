using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RobinTTY.PersonalFinanceDashboard.Core.DataProviders;
using RobinTTY.PersonalFinanceDashboard.Core.Extensions;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Services.DataSynchronization.Interfaces;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Services.DataSynchronization;

/// <summary>
/// Synchronizes bank accounts (balances and details) with the data providers they originate from.
/// </summary>
public class BankAccountSyncHandler(
    ApplicationDbContext dbContext,
    BankDataProviderRegistry providers,
    ThirdPartyDataRetrievalMetadataService dataRetrievalMetadataService,
    TimeProvider timeProvider,
    ILogger<BankAccountSyncHandler> logger) : IBankAccountSyncHandler
{
    /// <summary>
    /// Synchronizes stale bank account data by fetching it from the account's data provider and updating the local database. If a specific bank account ID is provided, synchronization is limited to that account; otherwise, all accounts of active authentication requests are synchronized.
    /// </summary>
    /// <param name="internalBankAccountId">The internal ID of a specific bank account to synchronize. If <see langword="null"/>, all bank accounts will be synchronized.</param>
    /// <param name="forceThirdPartySync">Indicates whether to force synchronization with the third-party API.</param>
    /// <returns>A boolean value indicating whether the synchronization process was successful.</returns>
    public async Task<bool> SynchronizeData(Guid? internalBankAccountId = null, bool forceThirdPartySync = false)
    {
        if (internalBankAccountId.HasValue)
        {
            var bankAccount = await dbContext.BankAccounts
                .SingleOrDefaultAsync(account => account.Id == internalBankAccountId);
            if (bankAccount == null)
            {
                logger.LogWarning("Bank account with id {bankAccountId} does not exist.", internalBankAccountId);
                return false;
            }

            // Accounts without a provider (e.g. manually created ones) aren't synchronized
            if (providers.TryGet(bankAccount.Source, out var provider))
                await SynchronizeBankAccounts(provider, [bankAccount], forceThirdPartySync);

            return true;
        }

        var accountsBySource = (await GetAccountsOfActiveAuthenticationRequests()).GroupBy(account => account.Source);
        foreach (var accounts in accountsBySource)
        {
            if (providers.TryGet(accounts.Key, out var provider))
                await SynchronizeBankAccounts(provider, accounts, forceThirdPartySync);
        }

        return true;
    }

    /// <summary>
    /// Gets the bank accounts associated with active authentication requests that are not older than 90 days.
    /// </summary>
    private async Task<List<BankAccount>> GetAccountsOfActiveAuthenticationRequests()
    {
        var authRequests = await dbContext.AuthenticationRequests
            .Include(authenticationRequest => authenticationRequest.AssociatedAccounts).ToListAsync();

        // An account can be linked to multiple active authentication requests
        return authRequests
            .Where(authenticationRequest => authenticationRequest.IsActive(timeProvider.GetUtcNow().UtcDateTime))
            .SelectMany(authenticationRequest => authenticationRequest.AssociatedAccounts)
            .DistinctBy(account => account.Id)
            .ToList();
    }

    /// <summary>
    /// Fetches the stale bank accounts from the provider and updates them in the database.
    /// </summary>
    /// <param name="provider">The provider the accounts originate from.</param>
    /// <param name="bankAccounts">The bank accounts to synchronize, if their data is stale.</param>
    /// <param name="forceSync">Indicates whether to force synchronization with the third party API.</param>
    private async Task SynchronizeBankAccounts(IBankDataProvider provider, IEnumerable<BankAccount> bankAccounts,
        bool forceSync)
    {
        var accountsToFetch = await DetermineStaleBankAccounts(provider.Source, bankAccounts, forceSync);
        if (accountsToFetch.Count == 0)
            return;

        var fetchedBankAccounts = new List<BankAccount>();
        foreach (var response in await provider.GetBankAccounts(accountsToFetch))
        {
            if (response.IsSuccessful)
            {
                fetchedBankAccounts.Add(response.Result);
            }
            else
            {
                logger.LogWarning(
                    "Encountered error while getting bank account from {dataSource}. Summary: {errorSummary} Detail: {errorDetail}",
                    provider.Source, response.Error.Summary, response.Error.Detail);
            }
        }

        var updatedBankAccounts = await AddOrUpdateBankAccounts(provider.Source, fetchedBankAccounts);
        foreach (var bankAccount in updatedBankAccounts)
        {
            logger.LogDebug("Resetting data expiry for bank account with id {bankAccountId}", bankAccount.Id);
            await dataRetrievalMetadataService.ResetDataExpiry(provider.Source, ThirdPartyDataType.BankAccounts,
                bankAccount.Id);
        }

        logger.LogInformation("Synced {Count} bank accounts from {dataSource}", fetchedBankAccounts.Count,
            provider.Source);
    }

    /// <summary>
    /// Identifies the given bank accounts whose data is stale or must be synchronized because a force synchronization is required.
    /// Currency information is used as an indicator of whether metadata needs to be fetched for the account, as this information requires additional API calls to be retrieved but doesn't usually change.
    /// </summary>
    /// <param name="source">The data source the accounts originate from.</param>
    /// <param name="bankAccounts">The bank accounts to evaluate for staleness.</param>
    /// <param name="forceSync">Indicates whether synchronization should be forced regardless of staleness status.</param>
    /// <returns>A dictionary where the key is the external id of a bank account and the value indicates whether its metadata needs to be fetched.</returns>
    private async Task<Dictionary<string, bool>> DetermineStaleBankAccounts(DataSource source,
        IEnumerable<BankAccount> bankAccounts, bool forceSync)
    {
        var accountsToFetch = new Dictionary<string, bool>();

        foreach (var bankAccount in bankAccounts)
        {
            var dataIsStale =
                await dataRetrievalMetadataService.DataIsStale(source, ThirdPartyDataType.BankAccounts, bankAccount.Id);
            if (dataIsStale || forceSync)
            {
                accountsToFetch.Add(bankAccount.ExternalId!, bankAccount.Currency == null);
            }
            else
            {
                logger.LogDebug(
                    "Bank account data for account with id {bankAccountId} is not stale. Skipping synchronization with third party.",
                    bankAccount.Id);
            }
        }

        return accountsToFetch;
    }

    /// <summary>
    /// Adds new bank accounts or updates existing ones in the database based on the provided list of bank accounts.
    /// If a bank account with a matching external id exists, its information will be updated; otherwise,
    /// a new bank account record will be created.
    /// </summary>
    /// <param name="source">The data source the accounts were retrieved from.</param>
    /// <param name="bankAccounts">The list of bank accounts to add or update in the database.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of bank accounts
    /// that were added or updated in the database.</returns>
    private async Task<List<BankAccount>> AddOrUpdateBankAccounts(DataSource source, List<BankAccount> bankAccounts)
    {
        var accountIds = bankAccounts.Select(account => account.ExternalId!).Distinct().ToList();
        var existingAccounts = await dbContext.BankAccounts
            .Include(account => account.AssociatedAuthenticationRequests)
            .Include(account => account.AssociatedInstitution)
            .Where(account => account.Source == source && accountIds.Contains(account.ExternalId!))
            .ToDictionaryAsync(account => account.ExternalId!);

        var requestIds = bankAccounts
            .SelectMany(account => account.AssociatedAuthenticationRequests)
            .Select(request => request.ExternalId!)
            .Distinct()
            .ToList();
        var trackedRequests = await dbContext.AuthenticationRequests
            .Where(request => request.Source == source && requestIds.Contains(request.ExternalId!))
            .ToDictionaryAsync(request => request.ExternalId!);

        var institutionIds = bankAccounts
            .Select(account => account.AssociatedInstitution?.ExternalId)
            .OfType<string>()
            .Distinct()
            .ToList();
        var trackedInstitutions = await dbContext.BankingInstitutions
            .Where(institution => institution.Source == source && institutionIds.Contains(institution.ExternalId!))
            .ToDictionaryAsync(institution => institution.ExternalId!);

        var updatedBankAccounts = new List<BankAccount>();
        foreach (var updatedBankAccount in bankAccounts)
        {
            if (existingAccounts.TryGetValue(updatedBankAccount.ExternalId!, out var existingBankAccount))
            {
                existingBankAccount.UpdateNonNavigationProperties(updatedBankAccount);
                logger.LogInformation(
                    "Updated existing bank account {name} with id {bankAccountId}. New balance: {balance}.",
                    existingBankAccount.Name, existingBankAccount.Id, updatedBankAccount.Balance);
            }
            else
            {
                existingBankAccount = BankAccount.CreateWithoutNavigationProperties(updatedBankAccount);
                dbContext.BankAccounts.Add(existingBankAccount);
                existingAccounts[existingBankAccount.ExternalId!] = existingBankAccount;
            }

            UpdateAssociatedAuthenticationRequests(updatedBankAccount, existingBankAccount, trackedRequests);
            UpdateAssociatedInstitution(updatedBankAccount, existingBankAccount, trackedInstitutions);
            updatedBankAccounts.Add(existingBankAccount);
        }

        await dbContext.SaveChangesAsync();

        return updatedBankAccounts;
    }

    /// <summary>
    /// Updates the authentication requests associated with an existing bank account based on the authentication requests of the updated bank account.
    /// </summary>
    /// <param name="updatedBankAccount">The bank account containing the updated authentication requests.</param>
    /// <param name="existingBankAccount">The existing bank account to which the updated authentication requests will be linked.</param>
    /// <param name="trackedRequests">The authentication requests already in the database, keyed by their external id.
    /// Newly added requests are added to it.</param>
    private void UpdateAssociatedAuthenticationRequests(BankAccount updatedBankAccount,
        BankAccount existingBankAccount, Dictionary<string, AuthenticationRequest> trackedRequests)
    {
        foreach (var request in updatedBankAccount.AssociatedAuthenticationRequests)
        {
            if (!trackedRequests.TryGetValue(request.ExternalId!, out var trackedRequest))
            {
                trackedRequest = request;
                dbContext.AuthenticationRequests.Add(trackedRequest);
                trackedRequests[trackedRequest.ExternalId!] = trackedRequest;
            }

            var associationDoesNotExistYet = existingBankAccount.AssociatedAuthenticationRequests
                .All(existingRequest => existingRequest.ExternalId != trackedRequest.ExternalId);
            if (associationDoesNotExistYet)
            {
                existingBankAccount.AssociatedAuthenticationRequests.Add(trackedRequest);
            }
        }
    }

    /// <summary>
    /// Updates the associated institution of the bank account if it has changed. If the institution does not exist in the database, it will be added.
    /// </summary>
    /// <param name="updatedBankAccount">The bank account with the updated data.</param>
    /// <param name="existingBankAccount">The existing bank account in the database.</param>
    /// <param name="trackedInstitutions">The institutions already in the database, keyed by their external id.
    /// Newly added institutions are added to it.</param>
    private void UpdateAssociatedInstitution(BankAccount updatedBankAccount, BankAccount existingBankAccount,
        Dictionary<string, BankingInstitution> trackedInstitutions)
    {
        var institution = updatedBankAccount.AssociatedInstitution;
        if (institution == null)
            return;

        if (!trackedInstitutions.TryGetValue(institution.ExternalId!, out var trackedInstitution))
        {
            trackedInstitution = institution;
            dbContext.BankingInstitutions.Add(trackedInstitution);
            trackedInstitutions[trackedInstitution.ExternalId!] = trackedInstitution;
        }

        existingBankAccount.AssociatedInstitution = trackedInstitution;
    }
}