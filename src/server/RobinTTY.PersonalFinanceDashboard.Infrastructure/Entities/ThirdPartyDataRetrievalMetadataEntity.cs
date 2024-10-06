﻿using RobinTTY.PersonalFinanceDashboard.ThirdPartyDataProviders.Models;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Entities;

public class ThirdPartyDataRetrievalMetadataEntity(
    Guid id,
    ThirdPartyDataType dataType,
    ThirdPartyDataSource dataSource,
    DateTime lastRetrievalTime,
    TimeSpan retrievalInterval) : BaseEntity(id)
{
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