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
    ILogger<BankingInstitutionSyncHandler> logger) : IBankingInstitutionSyncHandler
{
    public async Task<bool> SynchronizeData(bool forceThirdPartySync = false,
        CancellationToken cancellationToken = default)
    {
        var success = true;
        foreach (var provider in providers.All)
            success &= await SynchronizeAllInstitutions(provider, forceThirdPartySync, cancellationToken);

        return success;
    }

    private async Task<bool> SynchronizeAllInstitutions(IBankDataProvider provider, bool forceThirdPartySync,
        CancellationToken cancellationToken)
    {
        if (!await IsSynchronizationDue(provider.Source, forceThirdPartySync, cancellationToken))
            return true;

        var response = await provider.GetBankingInstitutions(cancellationToken: cancellationToken);
        if (!response.IsSuccessful)
        {
            LogFailure(provider.Source, response.Error);
            await dataRetrievalMetadataService.RecordFailedRetrieval(provider.Source,
                ThirdPartyDataType.BankingInstitutions, null, response.Error, cancellationToken);
            return false;
        }

        await AddUpdateOrDeleteBankingInstitutions(provider.Source, response.Result, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        await dataRetrievalMetadataService.ResetDataExpiry(provider.Source, ThirdPartyDataType.BankingInstitutions,
            cancellationToken: cancellationToken);
        logger.LogInformation("Synced {Count} banking institutions from {dataSource}", response.Result.Count,
            provider.Source);

        return true;
    }

    private async Task<bool> IsSynchronizationDue(DataSource source, bool forceThirdPartySync,
        CancellationToken cancellationToken)
    {
        if (forceThirdPartySync ||
            await dataRetrievalMetadataService.IsRetrievalDue(source, ThirdPartyDataType.BankingInstitutions,
                cancellationToken: cancellationToken))
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
    /// <param name="cancellationToken">A token to signal cancellation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task AddUpdateOrDeleteBankingInstitutions(DataSource source,
        IReadOnlyList<BankingInstitution> bankingInstitutions, CancellationToken cancellationToken)
    {
        // Institutions of other data sources (e.g. manually created ones) aren't managed by this sync
        var allInstitutionsInDb = await dbContext.BankingInstitutions
            .Where(institution => institution.Source == source).ToListAsync(cancellationToken);
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

        AddOrUpdateBankingInstitutions(bankingInstitutions, dbInstitutionsDict);
    }

    private void AddOrUpdateBankingInstitutions(IReadOnlyList<BankingInstitution> bankingInstitutions,
        Dictionary<string, BankingInstitution> dbInstitutionsDict)
    {
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