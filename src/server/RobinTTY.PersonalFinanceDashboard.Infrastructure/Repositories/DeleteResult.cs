namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Repositories;

/// <summary>
/// The outcome of deleting an entity.
/// </summary>
public enum DeleteResult
{
    /// <summary>
    /// The entity was deleted.
    /// </summary>
    Deleted,

    /// <summary>
    /// The entity doesn't exist.
    /// </summary>
    NotFound,

    /// <summary>
    /// The entity wasn't deleted, since it's still synchronized with its data source, which would recreate it.
    /// </summary>
    Synchronized
}
