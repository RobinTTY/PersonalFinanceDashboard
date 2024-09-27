using Microsoft.EntityFrameworkCore;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Entities;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure;

/// <summary>
/// The database context which provides access to the underlying data of the application.
/// </summary>
public sealed class ApplicationDbContext : DbContext
{
    /// <summary>
    /// <see cref="DbSet{TEntity}"/> holding transactions of the application.
    /// </summary>
    public DbSet<TransactionEntity> Transactions => Set<TransactionEntity>();

    /// <summary>
    /// <see cref="DbSet{TEntity}"/> holding tags of the application.
    /// </summary>
    public DbSet<TagEntity> Tags => Set<TagEntity>();

    /// <summary>
    /// <see cref="DbSet{TEntity}"/> holding banking institutions of the application.
    /// </summary>
    public DbSet<BankingInstitutionEntity> BankingInstitutions => Set<BankingInstitutionEntity>();

    public DbSet<ThirdPartyDataRetrievalMetadataEntity> ThirdPartyDataRetrievalMetadata =>
        Set<ThirdPartyDataRetrievalMetadataEntity>();

    /// <summary>
    /// Creates a new instance of <see cref="ApplicationDbContext"/>.
    /// </summary>
    /// <param name="options">The <see cref="DbContextOptions"/> to use.</param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
}
