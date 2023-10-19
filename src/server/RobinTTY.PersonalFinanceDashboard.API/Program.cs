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

// TODO: Create a high level overview of the architecture that should apply
//      - There can be many data providers
//      - Data providers could be configured via the front-end
//      - For the beginning it is smart to start with one provider to keep complexity low
//      - Since the authentication/retrieval logic will differ from provider to provider, it will be difficult
//        to abstract this logic away into one unified interface, so maybe I will need to refine/scrap this idea later...

// HotChocolate GraphQL Setup
builder.Services
    .AddGraphQLServer()
    .AddQueryType()
    .AddMutationType<Mutation>()
    //TODO: .AddMutationConventions()
    .AddApiTypes()
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
