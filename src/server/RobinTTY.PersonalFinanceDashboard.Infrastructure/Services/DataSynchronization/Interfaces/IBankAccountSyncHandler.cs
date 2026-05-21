namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Services.DataSynchronization.Interfaces;

public interface IBankAccountSyncHandler
{
    Task<bool> SynchronizeData(Guid? internalBankAccountId = null, bool forceThirdPartySync = false);
}