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
            Guid? thirdPartyId = null;
            if (authenticationRequestId.HasValue)
            {
                thirdPartyId = dbContext.AuthenticationRequests
                    .SingleOrDefault(authRequest => authRequest.Id == authenticationRequestId)?.ThirdPartyId;
                if (thirdPartyId == null)
                {
                    logger.LogWarning("Authentication request with id {id} does not exist.", authenticationRequestId);
                    return false;
                }
            }

            var authenticationRequests = await GetAuthenticationRequests(thirdPartyId);
            if (authenticationRequests == null)
            {
                return false;
            }

            var numOfAddedAuthRequests = await AddOrUpdateAuthenticationRequests(authenticationRequests);
            var numOfRemovedAuthRequests = 0;
            if (!thirdPartyId.HasValue)
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

    private async Task<List<AuthenticationRequest>?> GetAuthenticationRequests(Guid? authenticationRequestId)
    {
        List<AuthenticationRequest>? authRequests = null;

        if (authenticationRequestId.HasValue)
        {
            var response = await dataProvider.GetAuthenticationRequest(authenticationRequestId.Value);
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
        var requestIds = authenticationRequests.Select(request => request.ThirdPartyId).Distinct().ToList();
        var existingRequests = await dbContext.AuthenticationRequests
            .Include(request => request.AssociatedAccounts)
            .Where(request => requestIds.Contains(request.ThirdPartyId))
            .ToDictionaryAsync(request => request.ThirdPartyId);

        // An account can belong to multiple requests (e.g. after renewing consent), so it's only looked up once
        var accountIds = authenticationRequests
            .SelectMany(request => request.AssociatedAccounts)
            .Select(account => account.ThirdPartyId)
            .Distinct()
            .ToList();
        var trackedAccounts = await dbContext.BankAccounts
            .Where(account => accountIds.Contains(account.ThirdPartyId))
            .ToDictionaryAsync(account => account.ThirdPartyId);

        var numOfAddedAuthRequests = 0;
        foreach (var updatedAuthenticationRequest in authenticationRequests)
        {
            if (existingRequests.TryGetValue(updatedAuthenticationRequest.ThirdPartyId, out var existingRequest))
            {
                existingRequest.Status = updatedAuthenticationRequest.Status;
                existingRequest.AuthenticationLink = updatedAuthenticationRequest.AuthenticationLink;
            }
            else
            {
                existingRequest =
                    AuthenticationRequest.CreateWithoutNavigationProperties(updatedAuthenticationRequest);
                dbContext.AuthenticationRequests.Add(existingRequest);
                existingRequests[existingRequest.ThirdPartyId] = existingRequest;

                numOfAddedAuthRequests++;
            }

            UpdateAssociatedBankAccounts(updatedAuthenticationRequest, existingRequest, trackedAccounts);
        }

        await dbContext.SaveChangesAsync();

        return numOfAddedAuthRequests;
    }

    private void UpdateAssociatedBankAccounts(AuthenticationRequest updatedAuthenticationRequest,
        AuthenticationRequest existingRequest, Dictionary<Guid, BankAccount> trackedAccounts)
    {
        foreach (var account in updatedAuthenticationRequest.AssociatedAccounts)
        {
            if (!trackedAccounts.TryGetValue(account.ThirdPartyId, out var trackedAccount))
            {
                trackedAccount = account;
                dbContext.BankAccounts.Add(trackedAccount);
                trackedAccounts[trackedAccount.ThirdPartyId] = trackedAccount;
            }

            // If the account is already tracked, check first if the existing authentication request already contains the association
            var associationDoesNotExistYet = existingRequest.AssociatedAccounts
                .All(bankAccount => bankAccount.ThirdPartyId != trackedAccount.ThirdPartyId);
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
        var thirdPartyIds = authenticationRequests.Select(req => req.ThirdPartyId).ToList();

        return await dbContext.AuthenticationRequests
            .Where(req => !thirdPartyIds.Contains(req.ThirdPartyId))
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