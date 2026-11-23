using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RobinTTY.PersonalFinanceDashboard.Core.Models;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.EntityConfigurations;

/// <summary>
/// Configures how <see cref="BankingInstitution"/>s are stored.
/// </summary>
public class BankingInstitutionTypeConfiguration : IEntityTypeConfiguration<BankingInstitution>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<BankingInstitution> builder)
    {
        builder.ConfigureSource();
    }
}
