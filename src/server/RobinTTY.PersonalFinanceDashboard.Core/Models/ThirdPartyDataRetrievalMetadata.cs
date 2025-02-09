using RobinTTY.PersonalFinanceDashboard.ThirdPartyDataProviders.Models;

namespace RobinTTY.PersonalFinanceDashboard.Core.Models;

public class ThirdPartyDataRetrievalMetadata(
    Guid id,
    ThirdPartyDataType dataType,
    ThirdPartyDataSource dataSource,
    DateTime lastRetrievalTime,
    TimeSpan retrievalInterval)
{
    /// <summary>
    /// Distinct id identifying this entity.
    /// </summary>
    public Guid Id { get; init; } = id;
    /// <summary>
    /// The type of data being retrieved. 
    /// </summary>
    public ThirdPartyDataType DataType { get; set; } = dataType;
    /// <summary>
    /// The source of the data.
    /// </summary>
    public ThirdPartyDataSource DataSource { get; set; } = dataSource;
    /// <summary>
    /// The <see cref="DateTime"/> at which the data was retrieved last.
    /// </summary>
    public DateTime LastRetrievalTime { get; set; } = lastRetrievalTime;
    /// <summary>
    /// The interval at which the data should be retrieved again from the data source.
    /// </summary>
    public TimeSpan RetrievalInterval { get; set; } = retrievalInterval;
}
