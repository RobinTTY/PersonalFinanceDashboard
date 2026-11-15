using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RobinTTY.PersonalFinanceDashboard.Core.DataProviders;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Services.DataSynchronization.Interfaces;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Services.DataSynchronization;

/// <summary>
/// Synchronizes the authentication requests of the registered data providers, and the accounts they grant access
/// to, with the database.
/// </summary>
public class AuthenticationRequestSyncHandler(
    ApplicationDbContext dbContext,
    BankDataProviderRegistry providers,
    ThirdPartyDataRetrievalMetadataService dataRetrievalMetadataService,
    ILogger<AuthenticationRequestSyncHandler> logger) : IAuthenticationRequestSyncHandler
{
    public async Task<bool> SynchronizeData(Guid? authenticationRequestId = null, bool forceThirdPartySync = false)
    {
        if (authenticationRequestId.HasValue)
            return await SynchronizeAuthenticationRequest(authenticationRequestId.Value, forceThirdPartySync);

        var success = true;
        foreach (var provider in providers.All)
            success &= await SynchronizeAllAuthenticationRequests(provider, forceThirdPartySync);

        return success;
    }

    private async Task<bool> SynchronizeAuthenticationRequest(Guid authenticationRequestId, bool forceThirdPartySync)
    {
        var authenticationRequest = await dbContext.AuthenticationRequests
            .SingleOrDefaultAsync(authRequest => authRequest.Id == authenticationRequestId);
        if (authenticationRequest == null)
        {
            logger.LogWarning("Authentication request with id {id} does not exist.", authenticationRequestId);
            return false;
        }

        if (!providers.TryGet(authenticationRequest.Source, out var provider) ||
            !await IsSynchronizationDue(provider.Source, forceThirdPartySync))
            return true;

        var response = await provider.GetAuthenticationRequest(authenticationRequest.ExternalId!);
        if (!response.IsSuccessful)
        {
            LogFailure(provider.Source, response.Error);
            return false;
        }

        var numOfAddedAuthRequests = await AddOrUpdateAuthenticationRequests(provider.Source, [response.Result]);
        LogSynchronizationResults(provider.Source, 1, numOfAddedAuthRequests, 0);

        return true;
    }

    private async Task<bool> SynchronizeAllAuthenticationRequests(IBankDataProvider provider,
        bool forceThirdPartySync)
    {
        if (!await IsSynchronizationDue(provider.Source, forceThirdPartySync))
            return true;

        var response = await provider.GetAuthenticationRequests();
        if (!response.IsSuccessful)
        {
            LogFailure(provider.Source, response.Error);
            return false;
        }

        var authenticationRequests = response.Result;
        var numOfAddedAuthRequests = await AddOrUpdateAuthenticationRequests(provider.Source, authenticationRequests);
        var numOfRemovedAuthRequests =
            await RemoveNotIncludedAuthenticationRequests(provider.Source, authenticationRequests);
        await dataRetrievalMetadataService.ResetDataExpiry(provider.Source, ThirdPartyDataType.AuthenticationRequests);

        LogSynchronizationResults(provider.Source, authenticationRequests.Count, numOfAddedAuthRequests,
            numOfRemovedAuthRequests);

        return true;
    }

    private async Task<bool> IsSynchronizationDue(DataSource source, bool forceThirdPartySync)
    {
        if (forceThirdPartySync ||
            await dataRetrievalMetadataService.DataIsStale(source, ThirdPartyDataType.AuthenticationRequests))
            return true;

        logger.LogDebug("{dataType} data of {dataSource} is not stale. Skipping synchronization with third party.",
            ThirdPartyDataType.AuthenticationRequests, source);
        return false;
    }

    /// <summary>
    /// Adds new authentication requests to the database or updates existing ones based on their external ids.
    /// </summary>
    /// <param name="source">The data source the authentication requests were retrieved from.</param>
    /// <param name="authenticationRequests">A list of authentication requests to add or update in the database.</param>
    /// <returns>The number of newly added authentication requests.</returns>
    private async Task<int> AddOrUpdateAuthenticationRequests(DataSource source,
        IReadOnlyCollection<AuthenticationRequest> authenticationRequests)
    {
        var requestIds = authenticationRequests.Select(request => request.ExternalId!).Distinct().ToList();
        var existingRequests = await dbContext.AuthenticationRequests
            .Include(request => request.AssociatedAccounts)
            .Where(request => request.Source == source && requestIds.Contains(request.ExternalId!))
            .ToDictionaryAsync(request => request.ExternalId!);

        // An account can belong to multiple requests (e.g. after renewing consent), so it's only looked up once
        var accountIds = authenticationRequests
            .SelectMany(request => request.AssociatedAccounts)
            .Select(account => account.ExternalId!)
            .Distinct()
            .ToList();
        var trackedAccounts = await dbContext.BankAccounts
            .Where(account => account.Source == source && accountIds.Contains(account.ExternalId!))
            .ToDictionaryAsync(account => account.ExternalId!);

        var numOfAddedAuthRequests = 0;
        foreach (var updatedAuthenticationRequest in authenticationRequests)
        {
            if (existingRequests.TryGetValue(updatedAuthenticationRequest.ExternalId!, out var existingRequest))
            {
                existingRequest.Status = updatedAuthenticationRequest.Status;
                existingRequest.AuthenticationLink = updatedAuthenticationRequest.AuthenticationLink;
            }
            else
            {
                existingRequest =
                    AuthenticationRequest.CreateWithoutNavigationProperties(updatedAuthenticationRequest);
                dbContext.AuthenticationRequests.Add(existingRequest);
                existingRequests[existingRequest.ExternalId!] = existingRequest;

                numOfAddedAuthRequests++;
            }

            UpdateAssociatedBankAccounts(updatedAuthenticationRequest, existingRequest, trackedAccounts);
        }

        await dbContext.SaveChangesAsync();

        return numOfAddedAuthRequests;
    }

    private void UpdateAssociatedBankAccounts(AuthenticationRequest updatedAuthenticationRequest,
        AuthenticationRequest existingRequest, Dictionary<string, BankAccount> trackedAccounts)
    {
        foreach (var account in updatedAuthenticationRequest.AssociatedAccounts)
        {
            if (!trackedAccounts.TryGetValue(account.ExternalId!, out var trackedAccount))
            {
                trackedAccount = account;
                dbContext.BankAccounts.Add(trackedAccount);
                trackedAccounts[trackedAccount.ExternalId!] = trackedAccount;
            }

            // If the account is already tracked, check first if the existing authentication request already contains the association
            var associationDoesNotExistYet = existingRequest.AssociatedAccounts
                .All(bankAccount => bankAccount.ExternalId != trackedAccount.ExternalId);
            if (associationDoesNotExistYet)
            {
                existingRequest.AssociatedAccounts.Add(trackedAccount);
            }
        }
    }

    /// <summary>
    /// Removes all authentication requests of the data source from the database that are not included in the
    /// provided collection of authentication requests.
    /// </summary>
    /// <param name="source">The data source the authentication requests were retrieved from.</param>
    /// <param name="authenticationRequests">The authentication requests to compare the ones stored in the
    /// database to.</param>
    /// <returns>The number of removed authentication requests.</returns>
    private async Task<int> RemoveNotIncludedAuthenticationRequests(DataSource source,
        IReadOnlyCollection<AuthenticationRequest> authenticationRequests)
    {
        var externalIds = authenticationRequests.Select(req => req.ExternalId!).ToList();

        return await dbContext.AuthenticationRequests
            .Where(req => req.Source == source && !externalIds.Contains(req.ExternalId!))
            .ExecuteDeleteAsync();
    }

    private void LogFailure(DataSource source, ThirdPartyError error)
    {
        logger.LogWarning(
            "Could not retrieve authentication requests from {dataSource}. Summary: {errorSummary} Detail: {errorDetail}",
            source, error.Summary, error.Detail);
    }

    private void LogSynchronizationResults(DataSource source, int numOfSynchronizedRequests,
        int numOfAddedAuthRequests, int numOfRemovedAuthRequests)
    {
        logger.LogInformation(
            "Synchronized {numOfSynchronizedRequests} authentication requests from {dataSource}. Added: {numOfAddedAuthRequests}, Removed: {numOfRemovedAuthRequests}",
            numOfSynchronizedRequests, source, numOfAddedAuthRequests, numOfRemovedAuthRequests);
    }
}