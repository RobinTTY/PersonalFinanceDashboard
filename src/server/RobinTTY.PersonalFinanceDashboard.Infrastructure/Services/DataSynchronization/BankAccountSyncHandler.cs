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
    /// <inheritdoc />
    public async Task<bool> SynchronizeData(Guid? internalBankAccountId = null, bool forceThirdPartySync = false,
        CancellationToken cancellationToken = default)
    {
        if (internalBankAccountId.HasValue)
        {
            var bankAccount = await dbContext.BankAccounts
                .SingleOrDefaultAsync(account => account.Id == internalBankAccountId, cancellationToken);
            if (bankAccount == null)
            {
                logger.LogWarning("Bank account with id {BankAccountId} does not exist.", internalBankAccountId);
                return false;
            }

            // Accounts without a provider (e.g. manually created ones) aren't synchronized
            if (providers.TryGet(bankAccount.Source, out var provider))
                await SynchronizeBankAccounts(provider, [bankAccount], forceThirdPartySync, cancellationToken);

            return true;
        }

        var accountsBySource = (await GetAccountsOfActiveAuthenticationRequests(cancellationToken))
            .GroupBy(account => account.Source);
        foreach (var accounts in accountsBySource)
        {
            if (providers.TryGet(accounts.Key, out var provider))
                await SynchronizeBankAccounts(provider, accounts.ToList(), forceThirdPartySync, cancellationToken);
        }

        return true;
    }

    /// <summary>
    /// Gets the bank accounts of authentication requests that currently grant access.
    /// </summary>
    private async Task<List<BankAccount>> GetAccountsOfActiveAuthenticationRequests(
        CancellationToken cancellationToken)
    {
        var authRequests = await dbContext.AuthenticationRequests
            .Include(authenticationRequest => authenticationRequest.AssociatedAccounts)
            .ToListAsync(cancellationToken);

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
    /// <param name="cancellationToken">A token to signal cancellation.</param>
    private async Task SynchronizeBankAccounts(IBankDataProvider provider,
        IReadOnlyCollection<BankAccount> bankAccounts,
        bool forceSync, CancellationToken cancellationToken)
    {
        var accountsToFetch =
            await DetermineStaleBankAccounts(provider.Source, bankAccounts, forceSync, cancellationToken);
        if (accountsToFetch.Count == 0)
            return;

        var accountIds = bankAccounts.ToDictionary(account => account.ExternalId!, account => account.Id);
        var fetchedBankAccounts = new List<BankAccount>();
        foreach (var (externalId, response) in await provider.GetBankAccounts(accountsToFetch, cancellationToken))
        {
            if (response.IsSuccessful)
            {
                fetchedBankAccounts.Add(response.Result);
                continue;
            }

            var accountId = accountIds[externalId];
            var nextRetryTime = await dataRetrievalMetadataService.RecordFailedRetrieval(provider.Source,
                ThirdPartyDataType.BankAccounts, accountId, response.Error, cancellationToken);
            logger.LogWarning(
                "Encountered error while getting bank account with id {BankAccountId} from {DataSource}, retrying after {NextRetryTime}. Summary: {ErrorSummary} Detail: {ErrorDetail}",
                accountId, provider.Source, nextRetryTime, response.Error.Summary, response.Error.Detail);
        }

        await TakeOverPredecessors(provider.Source, fetchedBankAccounts, cancellationToken);
        var updatedBankAccounts =
            await AddOrUpdateBankAccounts(provider.Source, fetchedBankAccounts, cancellationToken);
        foreach (var bankAccount in updatedBankAccounts)
        {
            logger.LogDebug("Resetting data expiry for bank account with id {BankAccountId}", bankAccount.Id);
            await dataRetrievalMetadataService.ResetDataExpiry(provider.Source, ThirdPartyDataType.BankAccounts,
                bankAccount.Id, cancellationToken);
        }

        logger.LogInformation("Synced {Count} bank accounts from {DataSource}", fetchedBankAccounts.Count,
            provider.Source);
    }

    /// <summary>
    /// After a bank is reconnected, the provider assigns its accounts new ids. Each fetched account takes the place
    /// of the earlier account it replaces (see <see cref="FindPredecessor"/>), so that account keeps its id,
    /// transactions and settings. The placeholder created for the new id is removed.
    /// </summary>
    /// <param name="source">The data source the accounts were retrieved from.</param>
    /// <param name="fetchedBankAccounts">The accounts retrieved from the data source.</param>
    /// <param name="cancellationToken">A token to signal cancellation.</param>
    private async Task TakeOverPredecessors(DataSource source, List<BankAccount> fetchedBankAccounts,
        CancellationToken cancellationToken)
    {
        foreach (var fetched in fetchedBankAccounts)
        {
            var predecessor = await FindPredecessor(source, fetched, cancellationToken);
            if (predecessor == null)
                continue;

            var placeholder = await dbContext.BankAccounts
                .Include(account => account.AssociatedAuthenticationRequests)
                .SingleOrDefaultAsync(account => account.Source == source && account.ExternalId == fetched.ExternalId,
                    cancellationToken);
            if (placeholder != null)
            {
                foreach (var request in placeholder.AssociatedAuthenticationRequests
                             .Where(request => !predecessor.AssociatedAuthenticationRequests.Contains(request)))
                    predecessor.AssociatedAuthenticationRequests.Add(request);

                await dataRetrievalMetadataService.RemoveMetadata(source, placeholder.Id!.Value, cancellationToken);
                dbContext.BankAccounts.Remove(placeholder);
                // Frees the external id before the predecessor takes it
                await dbContext.SaveChangesAsync(cancellationToken);
            }

            // Transactions the earlier connection retrieved have different ids, so they aren't retrieved again
            var lastTransactionDate = await dbContext.Transactions
                .Where(transaction => transaction.BankAccount!.Id == predecessor.Id)
                .MaxAsync(transaction => transaction.ValueDate, cancellationToken);
            predecessor.ExternalId = fetched.ExternalId;
            await dbContext.SaveChangesAsync(cancellationToken);
            // Earlier retrieval failures and expiries don't apply to the new connection
            await dataRetrievalMetadataService.RemoveMetadata(source, predecessor.Id!.Value, cancellationToken);
            if (lastTransactionDate != null)
                await dataRetrievalMetadataService.SetRetrieveFrom(source, ThirdPartyDataType.Transactions,
                    predecessor.Id!.Value, lastTransactionDate.Value.Date, cancellationToken);

            logger.LogInformation(
                "Bank account with id {BankAccountId} was reconnected and now has the external id {ExternalId}.",
                predecessor.Id, predecessor.ExternalId);
        }
    }

    /// <summary>
    /// Finds the earlier account a fetched account replaces: one that is no longer synchronized and has the same
    /// IBAN or, at the same institution, the same BBAN or masked card number. These are less distinctive than an
    /// IBAN, so they only match if exactly one earlier account has them.
    /// </summary>
    /// <param name="source">The data source the account was retrieved from.</param>
    /// <param name="fetched">The account retrieved from the data source.</param>
    /// <param name="cancellationToken">A token to signal cancellation.</param>
    /// <returns>The earlier account, or <see langword="null"/> if there is none.</returns>
    private async Task<BankAccount?> FindPredecessor(DataSource source, BankAccount fetched,
        CancellationToken cancellationToken)
    {
        var institutionId = fetched.AssociatedInstitution?.ExternalId;
        var candidates = dbContext.BankAccounts
            .Include(account => account.AssociatedAuthenticationRequests)
            .Where(account => account.Source == source && account.ExternalId != fetched.ExternalId);
        if (fetched.Iban != null)
            candidates = candidates.Where(account => account.Iban == fetched.Iban);
        else if (institutionId != null && fetched.Bban != null)
            candidates = candidates.Where(account =>
                account.AssociatedInstitution!.ExternalId == institutionId && account.Bban == fetched.Bban);
        else if (institutionId != null && fetched.MaskedPan != null)
            candidates = candidates.Where(account =>
                account.AssociatedInstitution!.ExternalId == institutionId && account.MaskedPan == fetched.MaskedPan);
        else
            return null;

        // Accounts that are still synchronized through another connection aren't replaced
        var utcNow = timeProvider.GetUtcNow().UtcDateTime;
        var predecessors = (await candidates.ToListAsync(cancellationToken))
            .Where(account => !account.AssociatedAuthenticationRequests.Any(request => request.IsActive(utcNow)))
            .ToList();
        return fetched.Iban != null || predecessors.Count == 1 ? predecessors.FirstOrDefault() : null;
    }

    /// <summary>
    /// Identifies the given bank accounts whose data is stale or must be synchronized because a force synchronization is required.
    /// The metadata of an account requires additional API calls and doesn't usually change, so it's only fetched while
    /// the currency is unknown, or no identifier is known that recognizes the account after a reconnect.
    /// </summary>
    /// <param name="source">The data source the accounts originate from.</param>
    /// <param name="bankAccounts">The bank accounts to evaluate for staleness.</param>
    /// <param name="forceSync">Indicates whether synchronization should be forced regardless of staleness status.</param>
    /// <param name="cancellationToken">A token to signal cancellation.</param>
    /// <returns>A dictionary where the key is the external id of a bank account and the value indicates whether its metadata needs to be fetched.</returns>
    private async Task<Dictionary<string, bool>> DetermineStaleBankAccounts(DataSource source,
        IEnumerable<BankAccount> bankAccounts, bool forceSync, CancellationToken cancellationToken)
    {
        var accountsToFetch = new Dictionary<string, bool>();

        foreach (var bankAccount in bankAccounts)
        {
            var retrievalIsDue = await dataRetrievalMetadataService.IsRetrievalDue(source,
                ThirdPartyDataType.BankAccounts, bankAccount.Id, cancellationToken);
            if (retrievalIsDue || forceSync)
            {
                accountsToFetch.Add(bankAccount.ExternalId!,
                    bankAccount is { Currency: null } or { Iban: null, Bban: null, MaskedPan: null });
            }
            else
            {
                logger.LogDebug(
                    "Bank account data for account with id {BankAccountId} is not stale or waiting to be retried. Skipping synchronization with third party.",
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
    /// <param name="cancellationToken">A token to signal cancellation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of bank accounts
    /// that were added or updated in the database.</returns>
    private async Task<List<BankAccount>> AddOrUpdateBankAccounts(DataSource source, List<BankAccount> bankAccounts,
        CancellationToken cancellationToken)
    {
        var accountIds = bankAccounts.Select(account => account.ExternalId!).Distinct().ToList();
        var existingAccounts = await dbContext.BankAccounts
            .Include(account => account.AssociatedAuthenticationRequests)
            .Include(account => account.AssociatedInstitution)
            .Where(account => account.Source == source && accountIds.Contains(account.ExternalId!))
            .ToDictionaryAsync(account => account.ExternalId!, cancellationToken);

        var requestIds = bankAccounts
            .SelectMany(account => account.AssociatedAuthenticationRequests)
            .Select(request => request.ExternalId!)
            .Distinct()
            .ToList();
        var trackedRequests = await dbContext.AuthenticationRequests
            .Where(request => request.Source == source && requestIds.Contains(request.ExternalId!))
            .ToDictionaryAsync(request => request.ExternalId!, cancellationToken);

        var institutionIds = bankAccounts
            .Select(account => account.AssociatedInstitution?.ExternalId)
            .OfType<string>()
            .Distinct()
            .ToList();
        var trackedInstitutions = await dbContext.BankingInstitutions
            .Where(institution => institution.Source == source && institutionIds.Contains(institution.ExternalId!))
            .ToDictionaryAsync(institution => institution.ExternalId!, cancellationToken);

        var updatedBankAccounts = new List<BankAccount>();
        foreach (var updatedBankAccount in bankAccounts)
        {
            if (existingAccounts.TryGetValue(updatedBankAccount.ExternalId!, out var existingBankAccount))
            {
                existingBankAccount.UpdateNonNavigationProperties(updatedBankAccount);
                logger.LogInformation(
                    "Updated existing bank account {Name} with id {BankAccountId}. New balance: {Balance}.",
                    existingBankAccount.Name ?? existingBankAccount.ProviderName, existingBankAccount.Id,
                    updatedBankAccount.Balance);
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

        await dbContext.SaveChangesAsync(cancellationToken);

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
