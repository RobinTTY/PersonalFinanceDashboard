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

            if (bankingInstitutionId.HasValue)
                await AddOrUpdateBankingInstitutions(institutions);
            else
                await ReplaceBankingInstitutions(institutions);

            await dbContext.SaveChangesAsync();
            await dataRetrievalMetadataService.ResetDataExpiry(ThirdPartyDataType.BankingInstitutions);

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
    private async Task ReplaceBankingInstitutions(List<BankingInstitution> bankingInstitutions)
    {
        var allInstitutionsInDb = await dbContext.BankingInstitutions.ToListAsync();
        // Optimization for initial seeding
        if (allInstitutionsInDb.Count == 0)
        {
            dbContext.AddRange(bankingInstitutions);
            return;
        }

        var incomingInstitutionsDict = bankingInstitutions
            .DistinctBy(bi => bi.ThirdPartyId)
            .ToDictionary(bi => bi.ThirdPartyId);
        var dbInstitutionsDict = allInstitutionsInDb.ToDictionary(bi => bi.ThirdPartyId);

        // Institutions to delete
        var institutionsToDelete = allInstitutionsInDb
            .Where(db => !incomingInstitutionsDict.ContainsKey(db.ThirdPartyId)).ToList();
        dbContext.BankingInstitutions.RemoveRange(institutionsToDelete);

        await AddOrUpdateBankingInstitutions(bankingInstitutions, dbInstitutionsDict);
    }

    private async Task AddOrUpdateBankingInstitutions(List<BankingInstitution> bankingInstitutions,
        Dictionary<string, BankingInstitution>? dbInstitutionsDict = null)
    {
        dbInstitutionsDict ??= await dbContext.BankingInstitutions
            .Where(bi => bankingInstitutions.Select(i => i.ThirdPartyId).Contains(bi.ThirdPartyId))
            .ToDictionaryAsync(bi => bi.ThirdPartyId);

        foreach (var incoming in bankingInstitutions.DistinctBy(bi => bi.ThirdPartyId))
        {
            if (dbInstitutionsDict.TryGetValue(incoming.ThirdPartyId, out var existing))
            {
                existing.Bic = incoming.Bic;
                existing.Name = incoming.Name;
                existing.LogoUri = incoming.LogoUri;
                existing.Countries = incoming.Countries;
            }
            else
            {
                dbContext.BankingInstitutions.Add(incoming);
            }
        }
    }
}