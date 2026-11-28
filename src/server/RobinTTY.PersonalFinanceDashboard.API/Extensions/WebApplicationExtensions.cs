using HotChocolate.AspNetCore;
using Microsoft.EntityFrameworkCore;
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
    public static void UseGraphQl(this WebApplication app)
    {
        var isDevelopment = app.Environment.IsDevelopment();

        // Subscriptions are delivered over WebSockets
        app.UseWebSockets();
        app.MapGraphQL().WithOptions(new GraphQLServerOptions
        {
            Tool = { Enable = isDevelopment },
            EnableSchemaRequests = isDevelopment
        });
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
