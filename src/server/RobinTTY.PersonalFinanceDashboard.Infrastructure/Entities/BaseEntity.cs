namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Entities;

// TODO: The base entity should include the database id and any other general properties of ef entities
// All other entities should inherit from the base entity
/// <summary>
/// TODO
/// </summary>
public class BaseEntity(Guid id)
{
    public Guid Id { get; set; } = id;
}
