using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Interfaces;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Models;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Services.DataSynchronization;

public class DataSynchronizationService(
    IServiceProvider serviceProvider,
    ILogger<DataSynchronizationService> logger,
    ThirdPartyDataRetrievalMetadataService metadataService)
{
    private readonly Dictionary<SyncDataType, DataSyncDependency> dependencies = GetDataDependencies();

    public async Task SyncData(List<SyncDataType> dataTypes)
    {
        var staleTypes = new List<SyncDataType>();

        foreach (var dataType in dataTypes)
        {
            var dataIsStale = await metadataService.DataIsStale(ConvertToThirdPartyDataType(dataType));
            if (dataIsStale)
            {
                staleTypes.Add(dataType);
            }
        }

        if (staleTypes.Count == 0)
        {
            logger.LogInformation("No stale data found, skipping sync");
            return;
        }

        logger.LogInformation("Syncing data types in order: {DataTypes}", string.Join(", ", staleTypes));

        foreach (var dataType in staleTypes)
        {
            await SynchronizeDataType(dataType);
        }
    }
    
    // TODO: Later inject this as options, configurable from config?
    private static Dictionary<SyncDataType, DataSyncDependency> GetDataDependencies()
    {
        return new Dictionary<SyncDataType, DataSyncDependency>
        {
            [SyncDataType.BankingInstitutions] = new()
            {
                DataType = SyncDataType.BankingInstitutions,
                Dependencies = []
            },
            [SyncDataType.AuthenticationRequests] = new()
            {
                DataType = SyncDataType.AuthenticationRequests,
                Dependencies = [SyncDataType.BankAccounts],
            },
            [SyncDataType.BankAccounts] = new()
            {
                DataType = SyncDataType.BankAccounts,
                Dependencies =
                    [SyncDataType.BankingInstitutions, SyncDataType.AuthenticationRequests, SyncDataType.Transactions]
            },
            [SyncDataType.Transactions] = new()
            {
                DataType = SyncDataType.Transactions,
                Dependencies = [SyncDataType.BankAccounts],
            }
        };
    }

    private async Task SynchronizeDataType(SyncDataType dataType)
    {
        try
        {
            var syncHandler = GetSyncHandler(dataType);
            await syncHandler.SynchronizeData();
            await metadataService.ResetDataExpiry(ConvertToThirdPartyDataType(dataType));
            
            logger.LogInformation("Successfully synced {DataType}", dataType);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to sync {DataType}", dataType);
            throw;
        }
    }
    
    private IDataSynchronizationHandler GetSyncHandler(SyncDataType dataType)
    {
        return dataType switch
        {
            SyncDataType.BankingInstitutions => serviceProvider.GetRequiredService<BankingInstitutionSyncHandler>(),
            // SyncDataType.BankAccounts => serviceProvider.GetRequiredService<BankAccountSyncHandler>(),
            // SyncDataType.Transactions => serviceProvider.GetRequiredService<TransactionSyncHandler>(),
            // SyncDataType.AuthenticationRequests => serviceProvider.GetRequiredService<AuthenticationRequestSyncHandler>(),
            // SyncDataType.Balances => serviceProvider.GetRequiredService<BalanceSyncHandler>(),
            _ => throw new ArgumentException($"Unknown data type: {dataType}")
        };
    }

    private static ThirdPartyDataType ConvertToThirdPartyDataType(SyncDataType syncDataType)
    {
        return syncDataType switch
        {
            SyncDataType.BankingInstitutions => ThirdPartyDataType.BankingInstitutions,
            SyncDataType.BankAccounts => ThirdPartyDataType.BankAccounts,
            SyncDataType.AuthenticationRequests => ThirdPartyDataType.AuthenticationRequests,
            SyncDataType.Transactions => ThirdPartyDataType.Transactions,
            _ => throw new ArgumentException($"No mapping for {syncDataType}")
        };
    }
}