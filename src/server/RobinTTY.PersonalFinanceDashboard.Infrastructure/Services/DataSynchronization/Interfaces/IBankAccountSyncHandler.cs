namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Services.DataSynchronization.Interfaces;

public interface IBankAccountSyncHandler
{
    Task<bool> SynchronizeData(bool forceThirdPartySync = false);
}