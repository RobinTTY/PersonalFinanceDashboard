using RobinTTY.PersonalFinanceDashboard.Core.Models.Base;

namespace RobinTTY.PersonalFinanceDashboard.Core.Models;

/// <summary>
/// Tracks when data of a data provider was retrieved, so it's only retrieved again once its retrieval interval
/// passed, and failed retrievals are retried with an increasing delay.
/// </summary>
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
    /// The earliest time a failed retrieval is retried. Takes precedence over the configured retrieval interval
    /// while set, and is reset by a successful retrieval.
    /// </summary>
    public DateTime? NextRetryTime { get; set; }

    /// <summary>
    /// The entity the data belongs to, e.g. the account whose transactions are retrieved, so each one is retrieved
    /// on its own schedule. <see langword="null"/> for data retrieved as a whole, e.g. banking institutions.
    /// </summary>
    public Guid? SynchronizationEntityId { get; set; }
}
