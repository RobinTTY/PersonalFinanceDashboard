using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Services.DataSynchronization.Interfaces;
using RobinTTY.PersonalFinanceDashboard.ThirdPartyDataProviders;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Services.DataSynchronization;

public class AuthenticationRequestSyncHandler(
    ApplicationDbContext dbContext,
    GoCardlessDataProviderService dataProvider,
    ThirdPartyDataRetrievalMetadataService dataRetrievalMetadataService,
    ILogger<AuthenticationRequestSyncHandler> logger) : IAuthenticationRequestSyncHandler
{
    public async Task<bool> SynchronizeData(Guid? authenticationRequestId = null, bool forceThirdPartySync = false)
    {
        var dataIsStale = await dataRetrievalMetadataService.DataIsStale(ThirdPartyDataType.AuthenticationRequests);
        if (dataIsStale || forceThirdPartySync)
        {
            string? externalId = null;
            if (authenticationRequestId.HasValue)
            {
                externalId = (await dbContext.AuthenticationRequests
                    .SingleOrDefaultAsync(authRequest => authRequest.Id == authenticationRequestId))?.ExternalId;
                if (externalId == null)
                {
                    logger.LogWarning("Authentication request with id {id} does not exist.", authenticationRequestId);
                    return false;
                }
            }

            var authenticationRequests = await GetAuthenticationRequests(externalId);
            if (authenticationRequests == null)
            {
                return false;
            }

            var numOfAddedAuthRequests = await AddOrUpdateAuthenticationRequests(authenticationRequests);
            var numOfRemovedAuthRequests = 0;
            if (externalId == null)
            {
                numOfRemovedAuthRequests = await RemoveNotIncludedAuthenticationRequests(authenticationRequests);
                await dataRetrievalMetadataService.ResetDataExpiry(ThirdPartyDataType.AuthenticationRequests);
            }

            LogSynchronizationResults(authenticationRequests.Count, numOfAddedAuthRequests, numOfRemovedAuthRequests);
        }
        else
        {
            logger.LogDebug("{dataType} data is not stale. Skipping synchronization with third party.",
                ThirdPartyDataType.AuthenticationRequests);
        }

        return true;
    }

    private async Task<List<AuthenticationRequest>?> GetAuthenticationRequests(string? externalId)
    {
        List<AuthenticationRequest>? authRequests = null;

        if (externalId != null)
        {
            var response = await dataProvider.GetAuthenticationRequest(externalId);
            if (response.IsSuccessful)
                authRequests = [response.Result];
        }
        else
        {
            var response = await dataProvider.GetAuthenticationRequests();
            if (response.IsSuccessful)
                authRequests = [.. response.Result];
        }

        return authRequests;
    }

    /// <summary>
    /// Adds new authentication requests to the database or updates existing ones based on their third-party IDs.
    /// </summary>
    /// <param name="authenticationRequests">A list of authentication requests to add or update in the database.</param>
    /// <returns>The number of newly added authentication requests.</returns>
    private async Task<int> AddOrUpdateAuthenticationRequests(params List<AuthenticationRequest> authenticationRequests)
    {
        var requestIds = authenticationRequests.Select(request => request.ExternalId!).Distinct().ToList();
        var existingRequests = await dbContext.AuthenticationRequests
            .Include(request => request.AssociatedAccounts)
            .Where(request => request.Source == DataSource.GoCardless && requestIds.Contains(request.ExternalId!))
            .ToDictionaryAsync(request => request.ExternalId!);

        // An account can belong to multiple requests (e.g. after renewing consent), so it's only looked up once
        var accountIds = authenticationRequests
            .SelectMany(request => request.AssociatedAccounts)
            .Select(account => account.ExternalId!)
            .Distinct()
            .ToList();
        var trackedAccounts = await dbContext.BankAccounts
            .Where(account => account.Source == DataSource.GoCardless && accountIds.Contains(account.ExternalId!))
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
    /// Removes all authentication requests from the database that are not included in the provided collection
    /// of authentication requests.
    /// </summary>
    /// <param name="authenticationRequests">The authentication requests to compare the ones stored in the
    /// database to.</param>
    /// <returns>The number of removed authentication requests.</returns>
    private async Task<int> RemoveNotIncludedAuthenticationRequests(List<AuthenticationRequest> authenticationRequests)
    {
        var externalIds = authenticationRequests.Select(req => req.ExternalId!).ToList();

        return await dbContext.AuthenticationRequests
            .Where(req => req.Source == DataSource.GoCardless && !externalIds.Contains(req.ExternalId!))
            .ExecuteDeleteAsync();
    }

    private void LogSynchronizationResults(int numOfSynchronizedRequests, int numOfAddedAuthRequests,
        int numOfRemovedAuthRequests)
    {
        logger.LogInformation(
            "Synchronized {numOfSynchronizedRequests} authentication requests. Added: {numOfAddedAuthRequests}, Removed: {numOfRemovedAuthRequests}",
            numOfSynchronizedRequests, numOfAddedAuthRequests, numOfRemovedAuthRequests);
    }
}