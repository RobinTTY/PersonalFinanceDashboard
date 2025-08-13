using Microsoft.Extensions.Logging;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Extensions;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Interfaces;
using RobinTTY.PersonalFinanceDashboard.ThirdPartyDataProviders;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Services.DataSynchronization;

public class BankingInstitutionSyncHandler(
    ApplicationDbContext dbContext,
    GoCardlessDataProviderService dataProvider,
    ILogger<BankingInstitutionSyncHandler> logger)
    : IDataSynchronizationHandler
{
    public async Task SynchronizeData()
    {
        var response = await dataProvider.GetBankingInstitutions();
        if (!response.IsSuccessful)
        {
            throw new InvalidOperationException($"Failed to fetch banking institutions: {response.Error?.Summary}");
        }

        var institutions = response.Result?.ToList() ?? [];
        
        foreach (var institution in institutions)
        {
            await dbContext.AddOrUpdateBankingInstitution(institution);
        }

        await dbContext.SaveChangesAsync();
        logger.LogInformation("Synced {Count} banking institutions", institutions.Count);
    }
}