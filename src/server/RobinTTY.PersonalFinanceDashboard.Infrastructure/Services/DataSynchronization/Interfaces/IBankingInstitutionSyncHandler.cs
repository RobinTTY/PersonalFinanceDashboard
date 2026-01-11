namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Services.DataSynchronization.Interfaces;

public interface IBankingInstitutionSyncHandler
{
    Task<bool> SynchronizeData(Guid? bankingInstitutionId = null, bool forceThirdPartySync = false);
}