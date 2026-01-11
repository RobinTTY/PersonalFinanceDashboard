using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Services.DataSynchronization.Interfaces;
using RobinTTY.PersonalFinanceDashboard.ThirdPartyDataProviders;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Services.DataSynchronization;

public class BankingInstitutionSyncHandler(
    ApplicationDbContext dbContext,
    GoCardlessDataProviderService dataProvider,
    ThirdPartyDataRetrievalMetadataService dataRetrievalMetadataService,
    ILogger<BankingInstitutionSyncHandler> logger) : IBankingInstitutionSyncHandler
{
    public async Task<bool> SynchronizeData(Guid? bankingInstitutionId = null, bool forceThirdPartySync = false)
    {
        var dataIsStale = await dataRetrievalMetadataService.DataIsStale(ThirdPartyDataType.BankingInstitutions);
        if (dataIsStale || forceThirdPartySync)
        {
            string? thirdPartyBankingInstitutionId = null;
            if (bankingInstitutionId.HasValue)
                thirdPartyBankingInstitutionId = dbContext.BankingInstitutions
                    .SingleOrDefault(bankingInstitution => bankingInstitution.Id == bankingInstitutionId)?.ThirdPartyId;
            
            var institutions = await GetBankingInstitutions(thirdPartyBankingInstitutionId);
            if (institutions == null)
            {
                logger.LogWarning("Could not retrieve banking institutions from {dataProvider}",
                    nameof(GoCardlessDataProviderService));
                return false;
            }

            await ReplaceBankingInstitutions(institutions);
            await dataRetrievalMetadataService.ResetDataExpiry(ThirdPartyDataType.BankingInstitutions);
            await dbContext.SaveChangesAsync();

            logger.LogInformation("Synced {Count} banking institutions", institutions.Count);
        }

        return true;
    }

    private async Task<List<BankingInstitution>?> GetBankingInstitutions(string? bankingInstitutionId)
    {
        List<BankingInstitution>? bankingInstitutions = null;
        
        if (bankingInstitutionId != null)
        {
            var response = await dataProvider.GetBankingInstitution(bankingInstitutionId);
            if (response.IsSuccessful)
                bankingInstitutions = [response.Result];
        }
        else
        {
            var response = await dataProvider.GetBankingInstitutions();
            if (response.IsSuccessful)
                bankingInstitutions = [..response.Result];
        }

        return bankingInstitutions;
    }
    
    /// <summary>
    /// Replaces all existing banking institutions in the database with the provided list of banking institutions.
    /// </summary>
    /// <param name="bankingInstitutions">A list of banking institutions to be saved in the database, replacing the existing entries.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task ReplaceBankingInstitutions(List<BankingInstitution> bankingInstitutions)
    {
        // TODO: Optimize this query, it's slow
        var allInstitutionsInDb = dbContext.BankingInstitutions.ToList();
        var newInstitutions = allInstitutionsInDb.Except(bankingInstitutions);
    }
}