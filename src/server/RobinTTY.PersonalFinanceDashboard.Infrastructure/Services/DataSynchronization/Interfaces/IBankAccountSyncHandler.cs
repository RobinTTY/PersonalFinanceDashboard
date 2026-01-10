namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Services.DataSynchronization.Interfaces;

public interface IBankAccountSyncHandler
{
    Task<bool> SynchronizeData(Guid? bankAccountId = null, bool forceThirdPartySync = false);
}