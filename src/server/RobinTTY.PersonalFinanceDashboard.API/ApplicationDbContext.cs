using Microsoft.EntityFrameworkCore;
using RobinTTY.PersonalFinanceDashboard.API.EfModels;

namespace RobinTTY.PersonalFinanceDashboard.API;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<EfTransaction> Transactions { get; set; }
    public DbSet<EfTag> Tags { get; set; }
}
