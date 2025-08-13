namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Interfaces;

public interface IDataSynchronizationHandler
{
    /// <summary>
    /// Synchronizes data from the third party data source.
    /// </summary>
    /// <returns>Returns a Task that represents the asynchronous data synchronization operation.</returns>
    Task SynchronizeData();
}