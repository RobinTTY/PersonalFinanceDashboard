using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RobinTTY.PersonalFinanceDashboard.Core.Models;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.EntityConfigurations;

public class BankAccountTypeConfiguration : IEntityTypeConfiguration<BankAccount>
{
    public void Configure(EntityTypeBuilder<BankAccount> builder)
    {
        builder.HasOne(account => account.AssociatedInstitution)
            .WithMany()
            .OnDelete(DeleteBehavior.SetNull);
        builder.ConfigureSource();
    }
}