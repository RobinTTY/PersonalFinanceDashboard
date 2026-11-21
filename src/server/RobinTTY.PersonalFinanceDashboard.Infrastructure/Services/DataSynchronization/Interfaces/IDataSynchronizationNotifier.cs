namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Services.DataSynchronization.Interfaces;

/// <summary>
/// Is notified when a synchronization retrieved data from a data provider, so clients can refetch it.
/// </summary>
public interface IDataSynchronizationNotifier
{
    /// <summary>
    /// Notifies that a synchronization retrieved data, successfully or not.
    /// </summary>
    Task NotifyDataSynchronized();
}