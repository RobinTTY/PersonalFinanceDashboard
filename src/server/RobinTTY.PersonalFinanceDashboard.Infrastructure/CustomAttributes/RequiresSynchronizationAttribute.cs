namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.CustomAttributes;

/// <summary>
/// Specifies that the annotated method requires data synchronization before execution.
/// </summary>
/// <remarks>
/// This attribute can be applied to methods where data synchronization is critical to ensure
/// integrity and consistency. It allows specifying the types of data that are dependent on
/// synchronization and provides an option to force a refresh of the synchronized data.
/// </remarks>
/// <param name="dependentDataTypes">
/// An array of types that represent the data models that the method depends on for synchronization.
/// </param>
[AttributeUsage(AttributeTargets.Method)]
public class RequiresSynchronizationAttribute(params Type[] dependentDataTypes) : Attribute
{
    /// <summary>
    /// Gets the list of data types that the annotated method depends on for synchronization.
    /// </summary>
    /// <remarks>
    /// This property provides the types of data that are required to be synchronized
    /// when the corresponding method is invoked.
    /// </remarks>
    public Type[] DependentDataTypes { get; } = dependentDataTypes;

    /// <summary>
    /// Gets or sets a value indicating whether the synchronization process should bypass cached data
    /// and force a refresh from the source.
    /// </summary>
    /// <remarks>
    /// This property is useful in scenarios where the latest data from the source is required,
    /// regardless of any previously stored or cached information.
    /// </remarks>
    public bool ForceRefresh { get; set; } = false;
}