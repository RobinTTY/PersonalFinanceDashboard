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
            if (authenticationRequestId != null)
                authenticationRequestId = dbContext.AuthenticationRequests
                    .SingleOrDefault(authRequest => authRequest.Id == authenticationRequestId)?.ThirdPartyId;
            
            var authenticationRequests = await GetAuthenticationRequests(authenticationRequestId);
            if (authenticationRequests == null)
            {
                return false;
            }

            await AddOrUpdateAuthenticationRequests(authenticationRequests);
            await RemoveNotIncludedAuthenticationRequests(authenticationRequests);
            await dataRetrievalMetadataService.ResetDataExpiry(ThirdPartyDataType.AuthenticationRequests);

            logger.LogInformation("Synced {Count} authentication requests", authenticationRequests.Count);
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
            // TODO: Limit should be different
            var response = await dataProvider.GetAuthenticationRequests(100);
            if (response.IsSuccessful)
                authRequests = [..response.Result];
        }

        return authRequests;
    }
    
    /// <summary>
    /// Adds new authentication requests to the database or updates existing ones based on their third-party IDs.
    /// </summary>
    /// <param name="authenticationRequests">A list of authentication requests to add or update in the database.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task AddOrUpdateAuthenticationRequests(params List<AuthenticationRequest> authenticationRequests)
    {
        foreach (var updatedAuthenticationRequest in authenticationRequests)
        {
            var existingRequest = dbContext.AuthenticationRequests
                .Include(req => req.AssociatedAccounts)
                .SingleOrDefault(req => req.ThirdPartyId == updatedAuthenticationRequest.ThirdPartyId);

            if (existingRequest == null)
            {
                var insertEntity = AuthenticationRequest.CreateWithoutNavigationProperties(updatedAuthenticationRequest);
                var entry = await dbContext.AuthenticationRequests.AddAsync(insertEntity);
                existingRequest = entry.Entity;
            }
            else
            {
                existingRequest.Status = updatedAuthenticationRequest.Status;
                existingRequest.AuthenticationLink = updatedAuthenticationRequest.AuthenticationLink;
            }

            await UpdateAssociatedBankAccounts(updatedAuthenticationRequest, existingRequest);
            await dbContext.SaveChangesAsync();
        }
    }
    
    private async Task UpdateAssociatedBankAccounts(AuthenticationRequest updatedAuthenticationRequest,
        AuthenticationRequest existingRequest)
    {
        var associatedAccountIds = updatedAuthenticationRequest.AssociatedAccounts
            .Select(acc => acc.ThirdPartyId).Distinct().ToList();
        var trackedAccounts = dbContext.BankAccounts
            .Where(account => associatedAccountIds.Contains(account.ThirdPartyId)).ToList();

        foreach (var account in updatedAuthenticationRequest.AssociatedAccounts)
        {
            var trackedAccount = trackedAccounts.SingleOrDefault(a => a.ThirdPartyId == account.ThirdPartyId);

            if (trackedAccount == null)
            {
                var entry = await dbContext.BankAccounts.AddAsync(account);
                trackedAccount = entry.Entity;
                trackedAccounts.Add(trackedAccount);
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
    private async Task RemoveNotIncludedAuthenticationRequests(List<AuthenticationRequest> authenticationRequests)
    {
        var thirdPartyIds = authenticationRequests.Select(req => req.ThirdPartyId);

        await dbContext.AuthenticationRequests
            .Where(req => !thirdPartyIds.Contains(req.ThirdPartyId))
            .ExecuteDeleteAsync();
    }
}