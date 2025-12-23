namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Services.DataSynchronization.Interfaces;

public interface IBankingInstitutionSyncHandler
{
    Task<bool> SynchronizeData(bool forceThirdPartySync = false);
}