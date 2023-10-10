global using System;
global using Serilog;

using Microsoft.AspNetCore.Builder;
using HotChocolate.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RobinTTY.PersonalFinanceDashboard.API;
using RobinTTY.PersonalFinanceDashboard.API.Models;
using RobinTTY.PersonalFinanceDashboard.API.Utility;
using RobinTTY.PersonalFinanceDashboard.ThirdPartyDataProviders;
using RobinTTY.NordigenApiClient.Models;

var builder = WebApplication.CreateBuilder(args);
var appConfig = AppConfigurationManager.AppConfiguration;
builder.Services.AddSingleton(_ => new NordigenClientCredentials(appConfig.NordigenApi!.SecretId, appConfig.NordigenApi.SecretKey));
builder.Services.AddSingleton<GoCardlessDataProvider>();
builder.Services
    .AddHttpClient()
    .AddCors(options =>
    {
        // TODO: update to sensible policy
        options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
    })
    .AddPooledDbContextFactory<ApplicationDbContext>(options => options.UseSqlite("Data Source=application.db"))
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    //TODO: .AddMutationConventions()
    .RegisterDbContext<ApplicationDbContext>(DbContextKind.Pooled);

var app = builder.Build();

// Apply database migrations at startup
//using (var scope = app.Services.CreateScope())
//{
//    var contextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
//    using var context = contextFactory.CreateDbContext();
//    context.Database.Migrate();
//}

app.UseCors();
app.UseWebSockets();
app.MapGraphQL();
app.Run();
