using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RobinTTY.PersonalFinanceDashboard.Core.Models;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.EntityConfigurations;

public class BankingInstitutionTypeConfiguration : IEntityTypeConfiguration<BankingInstitution>
{
    public void Configure(EntityTypeBuilder<BankingInstitution> builder)
    {
        // Manually created institutions have an empty third party id and are excluded
        builder.HasIndex(entity => entity.ThirdPartyId)
            .IsUnique()
            .HasFilter($"\"{nameof(BankingInstitution.ThirdPartyId)}\" <> ''");
    }
}