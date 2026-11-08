using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RobinTTY.PersonalFinanceDashboard.Core.Models;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.EntityConfigurations;

public class BankAccountTypeConfiguration : IEntityTypeConfiguration<BankAccount>
{
    public void Configure(EntityTypeBuilder<BankAccount> builder)
    {
        // Manually created accounts have an empty third party id and are excluded
        builder.HasIndex(entity => entity.ThirdPartyId)
            .IsUnique()
            .HasFilter($"\"{nameof(BankAccount.ThirdPartyId)}\" <> '{Guid.Empty}'");
    }
}