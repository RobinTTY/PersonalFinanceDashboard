using Riok.Mapperly.Abstractions;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Entities;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Mappers;

/// <summary>
/// TODO
/// </summary>
[Mapper]
public partial class BankingInstitutionMapper
{
    public partial BankingInstitution EntityToModel(BankingInstitutionEntity transaction);
    public partial BankingInstitutionEntity ModelToEntity(BankingInstitution transaction);
}
