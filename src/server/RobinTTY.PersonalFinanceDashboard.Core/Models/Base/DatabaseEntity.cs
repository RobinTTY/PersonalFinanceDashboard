namespace RobinTTY.PersonalFinanceDashboard.Core.Models.Base;

/// <summary>
/// An entity which is stored in a database.
/// </summary>
public abstract class DatabaseEntity
{
    /// <summary>
    /// The id of this database entity.
    /// </summary>
    public required Guid? Id { get; init; }
}
