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

    public Task<bool> SynchronizeAll(bool forceThirdPartySync = false, CancellationToken cancellationToken = default) =>
        Run(async () =>
        {
            var success = true;
            foreach (var dataType in SynchronizationOrder)
                success &= await SynchronizeDataType(dataType, forceThirdPartySync, cancellationToken);

            return success;
        }, cancellationToken);

    public Task<bool> Synchronize(ThirdPartyDataType dataType, bool forceThirdPartySync = false,
        CancellationToken cancellationToken = default) =>
        Run(() => SynchronizeDataType(dataType, forceThirdPartySync, cancellationToken), cancellationToken);

    public Task<bool> RefreshAuthenticationRequest(Guid authenticationRequestId,
        CancellationToken cancellationToken = default) => Run(async () =>
    {
        if (!await authenticationRequestSyncHandler.SynchronizeData(authenticationRequestId, true,
                cancellationToken))
            return false;

        var request = await dbContext.AuthenticationRequests
            .Include(request => request.AssociatedAccounts)
            .SingleOrDefaultAsync(request => request.Id == authenticationRequestId, cancellationToken);
        if (request == null || !request.IsActive(timeProvider.GetUtcNow().UtcDateTime) ||
            request.AssociatedAccounts.Count == 0)
            return true;

        // Accounts of a reconnected bank thereby take the place of the earlier ones before they are returned
        foreach (var accountId in request.AssociatedAccounts.Select(account => account.Id!.Value).ToList())
            await bankAccountSyncHandler.SynchronizeData(accountId, cancellationToken: cancellationToken);

        trigger.RequestSynchronization();
        return true;
    }, cancellationToken);

    private Task<bool> SynchronizeDataType(ThirdPartyDataType dataType, bool forceThirdPartySync,
        CancellationToken cancellationToken)
    {
        return dataType switch
        {
            ThirdPartyDataType.BankingInstitutions =>
                bankingInstitutionSyncHandler.SynchronizeData(forceThirdPartySync, cancellationToken),
            ThirdPartyDataType.AuthenticationRequests =>
                authenticationRequestSyncHandler.SynchronizeData(forceThirdPartySync: forceThirdPartySync,
                    cancellationToken: cancellationToken),
            ThirdPartyDataType.BankAccounts =>
                bankAccountSyncHandler.SynchronizeData(forceThirdPartySync: forceThirdPartySync,
                    cancellationToken: cancellationToken),
            ThirdPartyDataType.Transactions =>
                transactionSyncHandler.SynchronizeData(forceThirdPartySync, cancellationToken),
            _ => throw new ArgumentOutOfRangeException(nameof(dataType), dataType, null)
        };
    }

    private async Task<bool> Run(Func<Task<bool>> synchronize, CancellationToken cancellationToken)
    {
        bool success;
        bool retrievedData;
        using (await synchronizationLock.Acquire(cancellationToken))
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