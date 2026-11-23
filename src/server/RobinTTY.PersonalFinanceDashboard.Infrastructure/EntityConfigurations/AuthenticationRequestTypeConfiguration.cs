using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RobinTTY.PersonalFinanceDashboard.Core.Models;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.EntityConfigurations;

/// <summary>
/// Configures how <see cref="AuthenticationRequest"/>s are stored.
/// </summary>
public class AuthenticationRequestTypeConfiguration : IEntityTypeConfiguration<AuthenticationRequest>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<AuthenticationRequest> builder)
    {
        builder.HasOne(request => request.Institution)
            .WithMany()
            .OnDelete(DeleteBehavior.SetNull);
        builder.ConfigureSource();
        builder.Property(entity => entity.CreatedAt).HasConversion<UtcDateTimeConverter>();
    }
}
