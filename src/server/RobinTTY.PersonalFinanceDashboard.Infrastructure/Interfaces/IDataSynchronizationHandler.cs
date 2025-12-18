namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Interfaces;

public interface IDataSynchronizationHandler
{
    /// <summary>
    /// Synchronizes data from the third party data source.
    /// </summary>
    /// <returns>Returns true if the sync was successful, otherwise returns false. </returns>
    Task<bool> SynchronizeData();
}