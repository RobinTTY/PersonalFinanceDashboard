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
    public required DataSource DataSource { get; set; }

    /// <summary>
    /// The <see cref="DateTime"/> at which the data was retrieved successfully the last time.
    /// </summary>
    public required DateTime LastRetrievalTime { get; set; }

    /// <summary>
    /// The number of retrievals that failed since the last successful one.
    /// </summary>
    public int ConsecutiveFailures { get; set; }

    /// <summary>
    /// The error of the last failed retrieval, <see langword="null"/> if the last retrieval was successful.
    /// </summary>
    public string? LastError { get; set; }

    /// <summary>
    /// The date from which the data is retrieved, <see langword="null"/> to retrieve all the data source provides.
    /// Set for the transactions of an account that was taken over from an earlier connection to its bank, whose
    /// transactions were already retrieved under different ids.
    /// </summary>
    public DateTime? RetrieveFrom { get; set; }

    /// <summary>
    /// The earliest time a failed retrieval is retried. Takes precedence over <see cref="RetrievalInterval"/>
    /// while set, and is reset by a successful retrieval.
    /// </summary>
    public DateTime? NextRetryTime { get; set; }

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
        DataSource dataSource,
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
