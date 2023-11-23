using Microsoft.EntityFrameworkCore;
using RobinTTY.PersonalFinanceDashboard.API.EfModels;

namespace RobinTTY.PersonalFinanceDashboard.API;

/// <summary>
/// The database context which provides access to the underlying data of the application.
/// </summary>
public sealed class ApplicationDbContext : DbContext
{
    /// <summary>
    /// <see cref="DbSet{TEntity}"/> holding transactions of the application.
    /// </summary>
    public DbSet<TransactionDto> Transactions => Set<TransactionDto>();
    /// <summary>
    /// <see cref="DbSet{TEntity}"/> holding tags of the application.
    /// </summary>
    public DbSet<TagDto> Tags => Set<TagDto>();

    /// <summary>
    /// Creates a new instance of <see cref="ApplicationDbContext"/>.
    /// </summary>
    /// <param name="options">The <see cref="DbContextOptions"/> to use.</param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
}
