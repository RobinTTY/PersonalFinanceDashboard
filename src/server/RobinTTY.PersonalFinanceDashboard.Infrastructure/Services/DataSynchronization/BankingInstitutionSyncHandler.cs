using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RobinTTY.PersonalFinanceDashboard.Core.DataProviders;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Services.DataSynchronization.Interfaces;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Services.DataSynchronization;

/// <summary>
/// Synchronizes the banking institutions of the registered data providers with the database.
/// </summary>
public class BankingInstitutionSyncHandler(
    ApplicationDbContext dbContext,
    BankDataProviderRegistry providers,
    ThirdPartyDataRetrievalMetadataService dataRetrievalMetadataService,
    DataSynchronizationLock synchronizationLock,
    ILogger<BankingInstitutionSyncHandler> logger) : IBankingInstitutionSyncHandler
{
    public async Task<bool> SynchronizeData(Guid? bankingInstitutionId = null, bool forceThirdPartySync = false)
    {
        using var acquiredLock = await synchronizationLock.Acquire();
        if (bankingInstitutionId.HasValue)
            return await SynchronizeInstitution(bankingInstitutionId.Value, forceThirdPartySync);

        var success = true;
        foreach (var provider in providers.All)
            success &= await SynchronizeAllInstitutions(provider, forceThirdPartySync);

        return success;
    }

    private async Task<bool> SynchronizeInstitution(Guid bankingInstitutionId, bool forceThirdPartySync)
    {
        var institution = await dbContext.BankingInstitutions
            .SingleOrDefaultAsync(bankingInstitution => bankingInstitution.Id == bankingInstitutionId);
        if (institution == null)
        {
            logger.LogWarning("Banking institution with id {id} does not exist.", bankingInstitutionId);
            return false;
        }

        // Institutions without a provider (e.g. manually created ones) aren't synchronized
        if (!providers.TryGet(institution.Source, out var provider) ||
            !await IsSynchronizationDue(provider.Source, forceThirdPartySync))
            return true;

        var response = await provider.GetBankingInstitution(institution.ExternalId!);
        if (!response.IsSuccessful)
        {
            LogFailure(provider.Source, response.Error);
            return false;
        }

        await AddOrUpdateBankingInstitutions(provider.Source, [response.Result]);
        await dbContext.SaveChangesAsync();
        logger.LogInformation("Synced banking institution with id {id} from {dataSource}", bankingInstitutionId,
            provider.Source);

        return true;
    }

    private async Task<bool> SynchronizeAllInstitutions(IBankDataProvider provider, bool forceThirdPartySync)
    {
        if (!await IsSynchronizationDue(provider.Source, forceThirdPartySync))
            return true;

        var response = await provider.GetBankingInstitutions();
        if (!response.IsSuccessful)
        {
            LogFailure(provider.Source, response.Error);
            await dataRetrievalMetadataService.RecordFailedRetrieval(provider.Source,
                ThirdPartyDataType.BankingInstitutions, null, response.Error);
            return false;
        }

        await AddUpdateOrDeleteBankingInstitutions(provider.Source, response.Result);
        await dbContext.SaveChangesAsync();
        await dataRetrievalMetadataService.ResetDataExpiry(provider.Source, ThirdPartyDataType.BankingInstitutions);
        logger.LogInformation("Synced {Count} banking institutions from {dataSource}", response.Result.Count,
            provider.Source);

        return true;
    }

    private async Task<bool> IsSynchronizationDue(DataSource source, bool forceThirdPartySync)
    {
        if (forceThirdPartySync ||
            await dataRetrievalMetadataService.IsRetrievalDue(source, ThirdPartyDataType.BankingInstitutions))
            return true;

        logger.LogDebug(
            "{dataType} data of {dataSource} is not stale or waiting to be retried. Skipping synchronization with third party.",
            ThirdPartyDataType.BankingInstitutions, source);
        return false;
    }

    private void LogFailure(DataSource source, ThirdPartyError error)
    {
        logger.LogWarning(
            "Could not retrieve banking institutions from {dataSource}. Summary: {errorSummary} Detail: {errorDetail}",
            source, error.Summary, error.Detail);
    }

    /// <summary>
    /// Uses the provided banking institutions to update the database. Upserts existing entries and removes any existing entities that are
    /// not contained in the provided data.
    /// </summary>
    /// <param name="source">The data source the institutions were retrieved from.</param>
    /// <param name="bankingInstitutions">A list of banking institutions to be saved in the database.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task AddUpdateOrDeleteBankingInstitutions(DataSource source,
        IReadOnlyList<BankingInstitution> bankingInstitutions)
    {
        // Institutions of other data sources (e.g. manually created ones) aren't managed by this sync
        var allInstitutionsInDb = await dbContext.BankingInstitutions
            .Where(institution => institution.Source == source).ToListAsync();
        // Optimization for initial seeding (empty db)
        if (allInstitutionsInDb.Count == 0)
        {
            dbContext.AddRange(bankingInstitutions.DistinctBy(bi => bi.ExternalId));
            return;
        }

        var incomingInstitutionsDict = bankingInstitutions
            .DistinctBy(bi => bi.ExternalId)
            .ToDictionary(bi => bi.ExternalId!);
        var dbInstitutionsDict = allInstitutionsInDb.ToDictionary(bi => bi.ExternalId!);

        var institutionsToDelete = allInstitutionsInDb
            .Where(db => !incomingInstitutionsDict.ContainsKey(db.ExternalId!)).ToList();
        dbContext.BankingInstitutions.RemoveRange(institutionsToDelete);

        await AddOrUpdateBankingInstitutions(source, bankingInstitutions, dbInstitutionsDict);
    }

    private async Task AddOrUpdateBankingInstitutions(DataSource source,
        IReadOnlyList<BankingInstitution> bankingInstitutions,
        Dictionary<string, BankingInstitution>? dbInstitutionsDict = null)
    {
        var externalIds = bankingInstitutions.Select(bi => bi.ExternalId!).ToList();
        dbInstitutionsDict ??= await dbContext.BankingInstitutions
            .Where(bi => bi.Source == source && externalIds.Contains(bi.ExternalId!))
            .ToDictionaryAsync(bi => bi.ExternalId!);

        foreach (var incoming in bankingInstitutions.DistinctBy(bi => bi.ExternalId))
        {
            if (dbInstitutionsDict.TryGetValue(incoming.ExternalId!, out var existing))
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