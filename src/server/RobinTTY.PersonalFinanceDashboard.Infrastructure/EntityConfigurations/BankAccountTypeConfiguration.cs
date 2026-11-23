using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RobinTTY.PersonalFinanceDashboard.Core.Models;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.EntityConfigurations;

/// <summary>
/// Configures how <see cref="BankAccount"/>s are stored.
/// </summary>
public class BankAccountTypeConfiguration : IEntityTypeConfiguration<BankAccount>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<BankAccount> builder)
    {
        builder.HasOne(account => account.AssociatedInstitution)
            .WithMany()
            .OnDelete(DeleteBehavior.SetNull);
        builder.ConfigureSource();
    }
}
