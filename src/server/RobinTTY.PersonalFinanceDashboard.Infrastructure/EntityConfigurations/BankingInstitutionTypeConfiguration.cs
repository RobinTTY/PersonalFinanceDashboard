using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RobinTTY.PersonalFinanceDashboard.Core.Models;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.EntityConfigurations;

public class BankingInstitutionTypeConfiguration : IEntityTypeConfiguration<BankingInstitution>
{
    public void Configure(EntityTypeBuilder<BankingInstitution> builder)
    {
        builder.HasIndex(entity => entity.ThirdPartyId);
    }
}
