namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Services.DataSynchronization.Interfaces;

public interface IAuthenticationRequestSyncHandler
{
    Task<bool> SynchronizeData(Guid? authenticationRequestId = null, bool forceThirdPartySync = false);
}