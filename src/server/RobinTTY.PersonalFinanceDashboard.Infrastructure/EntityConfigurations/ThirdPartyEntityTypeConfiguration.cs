using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RobinTTY.PersonalFinanceDashboard.Core.Models.Base;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.EntityConfigurations;

public class ThirdPartyEntityTypeConfiguration : IEntityTypeConfiguration<ThirdPartyEntity>
{
    public void Configure(EntityTypeBuilder<ThirdPartyEntity> builder)
    {
        builder.HasIndex(entity => entity.ThirdPartyId);
    }
}
