using Microsoft.EntityFrameworkCore;
using RobinTTY.PersonalFinanceDashboard.Core.Models;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure;

/// <summary>
/// The database context which provides access to the underlying data of the apllication.
/// </summary>
public sealed class ApplicationDbContext : DbContext
{
    /// <summary>
    /// <see cref="DbSet{TEntity}"/> holding transactions.
    /// </summary>
    public DbSet<Transaction> Transactions => Set<Transaction>();

    /// <summary>
    /// <see cref="DbSet{TEntity}"/> holding tags.
    /// </summary>
    public DbSet<Tag> Tags => Set<Tag>();

    /// <summary>
    /// <see cref="DbSet{TEntity}"/> holding banking institutions.
    /// </summary>
    public DbSet<BankingInstitution> BankingInstitutions => Set<BankingInstitution>();

    /// <summary>
    /// <see cref="DbSet{TEntity}"/> holding the bank accounts.
    /// </summary>
    public DbSet<BankAccount> BankAccounts => Set<BankAccount>();

    /// <summary>
    /// <see cref="DbSet{TEntity}"/> holding authentication requests.
    /// </summary>
    public DbSet<AuthenticationRequest> AuthenticationRequests => Set<AuthenticationRequest>();
    
    /// <summary>
    /// <see cref="DbSet{TEntity}"/> holding the third party data retrieval metadata.
    /// </summary>
    public DbSet<ThirdPartyDataRetrievalMetadata> ThirdPartyDataRetrievalMetadata =>
        Set<ThirdPartyDataRetrievalMetadata>();

    /// <summary>
    /// Creates a new instance of <see cref="ApplicationDbContext"/>.
    /// </summary>
    /// <param name="options">The <see cref="DbContextOptions"/> to use.</param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
