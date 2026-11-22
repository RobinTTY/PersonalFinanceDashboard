using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.Core.Models.Base;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.EntityConfigurations;

/// <summary>
/// Configures the properties shared by all <see cref="SourcedEntity"/>s.
/// </summary>
internal static class SourcedEntityConfiguration
{
    /// <summary>
    /// Stores the data source by name, ensures only manual entities lack an external id and makes external ids
    /// unique per data source.
    /// </summary>
    /// <param name="builder">The builder of the entity type to configure.</param>
    /// <param name="scope">Additional properties the external id is only unique in combination with,
    /// e.g. the account of a transaction.</param>
    public static void ConfigureSource<TEntity>(this EntityTypeBuilder<TEntity> builder, params string[] scope)
        where TEntity : SourcedEntity
    {
        const string source = nameof(SourcedEntity.Source);
        const string externalId = nameof(SourcedEntity.ExternalId);

        builder.Property(entity => entity.Source).HasConversion<string>();
        builder.ToTable(table => table.HasCheckConstraint($"CK_{typeof(TEntity).Name}_ExternalIdMatchesSource",
            $"(\"{source}\" = '{DataSource.Manual}') = (\"{externalId}\" IS NULL)"));
        builder.HasIndex([.. scope, source, externalId])
            .IsUnique()
            .HasFilter($"\"{externalId}\" IS NOT NULL");
    }
}
