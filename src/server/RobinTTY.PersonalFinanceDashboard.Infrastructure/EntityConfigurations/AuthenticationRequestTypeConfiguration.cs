using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RobinTTY.PersonalFinanceDashboard.Core.Models;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.EntityConfigurations;

public class AuthenticationRequestTypeConfiguration : IEntityTypeConfiguration<AuthenticationRequest>
{
    public void Configure(EntityTypeBuilder<AuthenticationRequest> builder)
    {
        builder.HasIndex(entity => entity.ThirdPartyId);
    }
}
