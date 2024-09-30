using Riok.Mapperly.Abstractions;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Entities;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Mappers;

/// <summary>
/// TODO
/// </summary>
[Mapper]
public partial class BankAccountMapper
{
    public partial BankAccount EntityToModel(BankAccountEntity bankAccountEntity);
    public partial BankAccountEntity ModelToEntity(BankAccount bankAccount);
}
