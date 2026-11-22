using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using RobinTTY.PersonalFinanceDashboard.Infrastructure;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Services.DataSynchronization;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Tests.TestUtilities;

namespace RobinTTY.PersonalFinanceDashboard.API.Tests.TestUtilities;

/// <summary>
/// Runs the API in memory with its own SQLite database file and a fake GoCardless API.
/// </summary>
public class ApiFactory : WebApplicationFactory<Program>
{
    private readonly string _databasePath = Path.Combine(Path.GetTempPath(), $"pfd-api-tests-{Guid.NewGuid()}.db");

    internal FakeGoCardless GoCardless { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Not Development, so logs aren't sent to Seq
        builder.UseEnvironment("Testing");
        builder.UseSetting("AppConfiguration:DatabaseConfiguration:ConnectionString", $"Data Source={_databasePath}");
        builder.UseSetting("AppConfiguration:GoCardlessConfiguration:SecretId", "secret-id");
        builder.UseSetting("AppConfiguration:GoCardlessConfiguration:SecretKey", "secret-key");
        builder.ConfigureTestServices(services =>
        {
            services.AddSingleton(GoCardless.CreateService());
            // Tests decide when data is synchronized
            services.Remove(services.Single(service =>
                service.ImplementationType == typeof(DataSynchronizationBackgroundService)));
        });
    }

    /// <summary>
    /// Saves the entity to the API's database and returns it with its generated id.
    /// </summary>
    internal async Task<TEntity> Seed<TEntity>(TEntity entity) where TEntity : class
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        dbContext.Add(entity);
        await dbContext.SaveChangesAsync();
        return entity;
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        // Pooled connections keep the file open
        SqliteConnection.ClearAllPools();
        File.Delete(_databasePath);
    }
}