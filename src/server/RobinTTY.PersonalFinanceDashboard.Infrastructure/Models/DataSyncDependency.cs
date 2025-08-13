namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Models;

public class DataSyncDependency
{
    public SyncDataType DataType { get; set; }
    public List<SyncDataType> Dependencies { get; set; } = new();
}