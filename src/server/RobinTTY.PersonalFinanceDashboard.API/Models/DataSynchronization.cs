namespace RobinTTY.PersonalFinanceDashboard.Api.Models;

/// <summary>
/// A synchronization that retrieved data from a data provider.
/// </summary>
public class DataSynchronization
{
    /// <summary>
    /// When the synchronization completed.
    /// </summary>
    public DateTime CompletedAt { get; init; }
}