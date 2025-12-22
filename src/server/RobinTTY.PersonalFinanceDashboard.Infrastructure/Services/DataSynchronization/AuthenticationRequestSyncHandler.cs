using Microsoft.Extensions.Logging;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Extensions;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Interfaces;
using RobinTTY.PersonalFinanceDashboard.ThirdPartyDataProviders;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Services.DataSynchronization;

public class AuthenticationRequestSyncHandler(
    ApplicationDbContext dbContext,
    GoCardlessDataProviderService dataProvider,
    ThirdPartyDataRetrievalMetadataService dataRetrievalMetadataService,
    ILogger<AuthenticationRequestSyncHandler> logger) : IDataSyncHandler
{
    // TODO: There should probably be a SynchronizeAll and Synchronize distinction to optimize response times
    // So we can distinguish between updating a single entity and all entities
    /// <inheritdoc />
    public async Task<bool> SynchronizeData(bool forceThirdPartySync = false)
    {
        var dataIsStale = await dataRetrievalMetadataService.DataIsStale(ThirdPartyDataType.AuthenticationRequests);
        if (dataIsStale || forceThirdPartySync)
        {
            var authenticationRequests = await GetAuthenticationRequests();
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
    
    private async Task<List<AuthenticationRequest>?> GetAuthenticationRequests()
    {
        // TODO: Limit should be different
        var response = await dataProvider.GetAuthenticationRequests(100);
        return !response.IsSuccessful ? null : response.Result.ToList();
    }
}