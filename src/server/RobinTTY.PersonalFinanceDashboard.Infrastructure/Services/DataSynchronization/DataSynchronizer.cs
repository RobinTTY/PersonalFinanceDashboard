using Microsoft.EntityFrameworkCore;
using RobinTTY.PersonalFinanceDashboard.Core.Extensions;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Services.DataSynchronization.Interfaces;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Services.DataSynchronization;

/// <inheritdoc />
public class DataSynchronizer(
    ApplicationDbContext dbContext,
    DataSynchronizationLock synchronizationLock,
    DataSynchronizationTrigger trigger,
    IBankingInstitutionSyncHandler bankingInstitutionSyncHandler,
    IAuthenticationRequestSyncHandler authenticationRequestSyncHandler,
    IBankAccountSyncHandler bankAccountSyncHandler,
    ITransactionSyncHandler transactionSyncHandler,
    ThirdPartyDataRetrievalMetadataService dataRetrievalMetadataService,
    IDataSynchronizationNotifier notifier,
    TimeProvider timeProvider) : IDataSynchronizer
{
    // Authentication requests add the accounts whose details and transactions are retrieved afterward
    private static readonly ThirdPartyDataType[] SynchronizationOrder =
    [
        ThirdPartyDataType.BankingInstitutions, ThirdPartyDataType.AuthenticationRequests,
        ThirdPartyDataType.BankAccounts, ThirdPartyDataType.Transactions
    ];

    public Task<bool> SynchronizeAll(bool forceThirdPartySync = false) => Run(async () =>
    {
        var success = true;
        foreach (var dataType in SynchronizationOrder)
            success &= await SynchronizeDataType(dataType, forceThirdPartySync);

        return success;
    });

    public Task<bool> Synchronize(ThirdPartyDataType dataType, bool forceThirdPartySync = false) =>
        Run(() => SynchronizeDataType(dataType, forceThirdPartySync));

    public Task<bool> RefreshAuthenticationRequest(Guid authenticationRequestId) => Run(async () =>
    {
        if (!await authenticationRequestSyncHandler.SynchronizeData(authenticationRequestId, true))
            return false;

        var request = await dbContext.AuthenticationRequests
            .Include(request => request.AssociatedAccounts)
            .SingleOrDefaultAsync(request => request.Id == authenticationRequestId);
        if (request == null || !request.IsActive(timeProvider.GetUtcNow().UtcDateTime) ||
            request.AssociatedAccounts.Count == 0)
            return true;

        // Accounts of a reconnected bank thereby take the place of the earlier ones before they are returned
        foreach (var accountId in request.AssociatedAccounts.Select(account => account.Id!.Value).ToList())
            await bankAccountSyncHandler.SynchronizeData(accountId);

        trigger.RequestSynchronization();
        return true;
    });

    private Task<bool> SynchronizeDataType(ThirdPartyDataType dataType, bool forceThirdPartySync) => dataType switch
    {
        ThirdPartyDataType.BankingInstitutions =>
            bankingInstitutionSyncHandler.SynchronizeData(forceThirdPartySync: forceThirdPartySync),
        ThirdPartyDataType.AuthenticationRequests =>
            authenticationRequestSyncHandler.SynchronizeData(forceThirdPartySync: forceThirdPartySync),
        ThirdPartyDataType.BankAccounts =>
            bankAccountSyncHandler.SynchronizeData(forceThirdPartySync: forceThirdPartySync),
        ThirdPartyDataType.Transactions =>
            transactionSyncHandler.SynchronizeData(forceThirdPartySync: forceThirdPartySync),
        _ => throw new ArgumentOutOfRangeException(nameof(dataType), dataType, null)
    };

    private async Task<bool> Run(Func<Task<bool>> synchronize)
    {
        bool success;
        bool retrievedData;
        using (await synchronizationLock.Acquire())
        {
            var recordedBefore = dataRetrievalMetadataService.RecordedRetrievals;
            success = await synchronize();
            retrievedData = dataRetrievalMetadataService.RecordedRetrievals != recordedBefore;
        }

        if (retrievedData)
            await notifier.NotifyDataSynchronized();

        return success;
    }
}