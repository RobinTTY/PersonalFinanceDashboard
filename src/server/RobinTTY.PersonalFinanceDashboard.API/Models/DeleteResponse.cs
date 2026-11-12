namespace RobinTTY.PersonalFinanceDashboard.Api.Models;

/// <summary>
/// The result of a successful delete mutation.
/// </summary>
public class DeleteResponse
{
    /// <summary>
    /// The id of the deleted entity.
    /// </summary>
    public Guid DeletedId { get; init; }
}