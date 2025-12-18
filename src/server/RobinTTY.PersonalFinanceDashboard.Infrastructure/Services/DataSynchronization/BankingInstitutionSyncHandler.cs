using Microsoft.Extensions.Logging;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Extensions;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Interfaces;
using RobinTTY.PersonalFinanceDashboard.ThirdPartyDataProviders;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Services.DataSynchronization;

public class BankingInstitutionSyncHandler(
    ApplicationDbContext dbContext,
    GoCardlessDataProviderService dataProvider,
    ThirdPartyDataRetrievalMetadataService dataRetrievalMetadataService,
    ILogger<BankingInstitutionSyncHandler> logger)
    : IDataSynchronizationHandler
{
    public async Task<bool> SynchronizeData()
    {
        var dataIsStale = await dataRetrievalMetadataService.DataIsStale(ThirdPartyDataType.BankingInstitutions);
        if (dataIsStale)
        {
            var institutions = await GetBankingInstitutions();
            if (institutions == null)
            {
                return false;
            }

            await dbContext.ReplaceBankingInstitutions(institutions);
            await dataRetrievalMetadataService.ResetDataExpiry(ThirdPartyDataType.BankingInstitutions);
            await dbContext.SaveChangesAsync();
            
            logger.LogInformation("Synced {Count} banking institutions", institutions.Count);
        }

        return true;
    }
    
    private async Task<List<BankingInstitution>?> GetBankingInstitutions()
    {
        var response = await dataProvider.GetBankingInstitutions();
        return !response.IsSuccessful ? null : response.Result.ToList();
    }
}