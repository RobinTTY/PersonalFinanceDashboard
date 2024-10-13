using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RobinTTY.PersonalFinanceDashboard.Infrastructure;

namespace RobinTTY.PersonalFinanceDashboard.Api.Extensions;

/// <summary>
/// Provides extensions for the <see cref="WebApplication"/> type.
/// </summary>
public static class WebApplicationExtensions
{
    /// <summary>
    /// Configures the app to use all necessary GraphQL features.
    /// </summary>
    /// <param name="app">The <see cref="WebApplication"/> instance to configure.</param>
    /// <param name="args">Command line arguments to be passed to the host.</param>
    public static void UseGraphQl(this WebApplication app, string[] args)
    {
        app.MapGraphQL();

        // Allows exporting the GraphQL Schema via the command line: dotnet run -- schema export --output schema.graphql
        // https://chillicream.com/docs/hotchocolate/v13/server/command-line
        app.RunWithGraphQLCommands(args);
    }

    /// <summary>
    /// Applies all outstanding migrations on the database.
    /// </summary>
    /// <param name="app">The <see cref="WebApplication"/> instance to use to resolve the db context.</param>
    public static void ApplyMigrations(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Database.Migrate();
    }
}
