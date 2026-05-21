using RobinTTY.PersonalFinanceDashboard.Core.Models.Base;

namespace RobinTTY.PersonalFinanceDashboard.Core.Models;

public class ThirdPartyDataRetrievalMetadata : DatabaseEntity
{
    /// <summary>
    /// The type of data being retrieved. 
    /// </summary>
    public required ThirdPartyDataType DataType { get; set; }

    /// <summary>
    /// The source of the data.
    /// </summary>
    public required ThirdPartyDataSource DataSource { get; set; }

    /// <summary>
    /// The <see cref="DateTime"/> at which the data was retrieved last.
    /// </summary>
    public required DateTime LastRetrievalTime { get; set; }

    // TODO: This may be removed 
    /// <summary>
    /// The interval at which the data should be retrieved again from the data source.
    /// </summary>
    public required TimeSpan RetrievalInterval { get; set; }

    /// <summary>
    /// Depending on the data type, we track individual synchronization entities. For example, for accounts,
    /// we track the account id for which the data retrieval metadata applies. This allows us to have different
    /// retrieval times and intervals for different accounts.
    /// </summary>
    public Guid? SynchronizationEntityId { get; set; }
    
    public ThirdPartyDataRetrievalMetadata() 
    {
    }

    public ThirdPartyDataRetrievalMetadata(Guid id,
        ThirdPartyDataType dataType,
        ThirdPartyDataSource dataSource,
        DateTime lastRetrievalTime,
        TimeSpan retrievalInterval)
    {
        Id = id;
        DataType = dataType;
        DataSource = dataSource;
        LastRetrievalTime = lastRetrievalTime;
        RetrievalInterval = retrievalInterval;
    }
}
