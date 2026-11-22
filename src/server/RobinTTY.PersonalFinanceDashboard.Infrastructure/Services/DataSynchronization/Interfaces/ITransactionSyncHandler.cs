namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Services.DataSynchronization.Interfaces;

public interface ITransactionSyncHandler
{
    Task<bool> SynchronizeData(bool forceThirdPartySync = false, CancellationToken cancellationToken = default);
}
