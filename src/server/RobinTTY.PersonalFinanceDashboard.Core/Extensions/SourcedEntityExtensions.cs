using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.Core.Models.Base;

namespace RobinTTY.PersonalFinanceDashboard.Core.Extensions;

/// <summary>
/// Provides extensions for the <see cref="SourcedEntity"/> type.
/// </summary>
public static class SourcedEntityExtensions
{
    /// <summary>
    /// Whether the entity was created by the user rather than retrieved from an external data source.
    /// </summary>
    // An extension rather than a property, so it isn't exposed through GraphQL or mapped by EF Core
    public static bool IsManual(this SourcedEntity entity) => entity.Source == DataSource.Manual;
}
