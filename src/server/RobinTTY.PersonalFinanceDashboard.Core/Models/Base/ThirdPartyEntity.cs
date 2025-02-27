namespace RobinTTY.PersonalFinanceDashboard.Core.Models.Base;

/// <summary>
/// A third party entity which has an id assigned to it by that third party service.
/// </summary>
public class ThirdPartyEntity : DatabaseEntity
{
    /// <summary>
    /// The id that was assigned by the third party service to this entity.
    /// </summary>
    public Guid ThirdPartyId { get; set; }
}