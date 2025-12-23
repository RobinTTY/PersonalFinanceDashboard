namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Services.DataSynchronization.Interfaces;

public interface IAuthenticationRequestSyncHandler
{
    Task<bool> SynchronizeData(bool forceThirdPartySync = false);
}