namespace RobinTTY.PersonalFinanceDashboard.Core.Models.Base;

/// <summary>
/// An entity that is either created by the user or retrieved from an external data source.
/// </summary>
public abstract class SourcedEntity : DatabaseEntity
{
    /// <summary>
    /// Where the entity originates from. Entities of external data sources are kept in sync with them,
    /// manual entities are only changed by the user.
    /// </summary>
    public DataSource Source { get; set; } = DataSource.Manual;

    /// <summary>
    /// The id the <see cref="Source"/> assigned to the entity, <see langword="null"/> for manual entities.
    /// </summary>
    public string? ExternalId { get; set; }

    /// <summary>
    /// Creates a new manual entity.
    /// </summary>
    protected SourcedEntity()
    {
    }

    /// <summary>
    /// Creates a new entity originating from the given data source.
    /// </summary>
    /// <param name="source">Where the entity originates from.</param>
    /// <param name="externalId">The id the data source assigned to the entity, <see langword="null"/> for
    /// manual entities.</param>
    /// <exception cref="ArgumentException">Thrown if a manual entity has an external id or an external
    /// entity has none.</exception>
    protected SourcedEntity(DataSource source, string? externalId)
    {
        if (source == DataSource.Manual != string.IsNullOrEmpty(externalId))
            throw new ArgumentException(source == DataSource.Manual
                ? "Manual entities can't have an external id."
                : $"Entities from {source} need an external id.", nameof(externalId));

        Source = source;
        ExternalId = externalId;
    }
}
