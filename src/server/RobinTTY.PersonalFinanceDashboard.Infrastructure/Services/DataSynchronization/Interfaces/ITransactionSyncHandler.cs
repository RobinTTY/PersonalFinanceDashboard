namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Services.DataSynchronization.Interfaces;

public interface ITransactionSyncHandler
{
    Task<bool> SynchronizeData(Guid internalAccountId, bool forceThirdPartySync = false);
}