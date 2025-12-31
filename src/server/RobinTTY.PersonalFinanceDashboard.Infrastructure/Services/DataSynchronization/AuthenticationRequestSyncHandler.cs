using Microsoft.Extensions.Logging;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Extensions;
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

            await dbContext.AddOrUpdateAuthenticationRequests(authenticationRequests);
            await dbContext.RemoveNotIncludedAuthenticationRequests(authenticationRequests);
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
}