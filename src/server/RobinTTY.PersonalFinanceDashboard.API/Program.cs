global using System;
global using Serilog;

using HotChocolate;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RobinTTY.PersonalFinanceDashboard.API;
using RobinTTY.PersonalFinanceDashboard.API.Models;
using RobinTTY.PersonalFinanceDashboard.API.Utility;
using RobinTTY.PersonalFinanceDashboard.ThirdPartyDataProviders;
using RobinTTY.NordigenApiClient.Models;
using RobinTTY.PersonalFinanceDashboard.API.Repositories;

var builder = WebApplication.CreateBuilder(args);
var appConfig = AppConfigurationManager.AppConfiguration;

// HTTP Setup
builder.Services
    .AddHttpClient()
    .AddCors(options =>
    {
        // TODO: update to sensible policy
        options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
    });

// DB Setup
builder.Services.AddDbContextPool<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=application.db"));

// General Services
builder.Services
    .AddScoped<TransactionRepository>()
    .AddSingleton(_ => new NordigenClientCredentials(appConfig.NordigenApi!.SecretId, appConfig.NordigenApi.SecretKey))
    .AddSingleton<GoCardlessDataProvider>();

// HotChocolate GraphQL Setup
builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    //TODO: .AddMutationConventions()
    .RegisterService<TransactionRepository>(ServiceKind.Resolver);

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
