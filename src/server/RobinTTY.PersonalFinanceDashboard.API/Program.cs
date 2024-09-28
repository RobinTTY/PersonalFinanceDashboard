global using System;
global using System.Linq;
global using System.Threading;
global using System.Collections.Generic;
global using System.Threading.Tasks;
global using Serilog;
global using HotChocolate;

using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RobinTTY.PersonalFinanceDashboard.API.Utility;
using RobinTTY.PersonalFinanceDashboard.ThirdPartyDataProviders;
using RobinTTY.NordigenApiClient.Models;
using RobinTTY.PersonalFinanceDashboard.Infrastructure;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Mappers;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Repositories;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);
var appConfig = AppConfigurationManager.AppConfiguration;

// Configure logging
builder.Host.UseSerilog((context, loggerConfig) =>
{
    loggerConfig.ReadFrom.Configuration(context.Configuration);
});

// HTTP setup
builder.Services
    .AddHttpClient()
    .AddCors(options =>
    {
        // TODO: update to sensible policy
        options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
    });

// DB setup
// TODO: The filepath shouldn't be hardcoded
builder.Services.AddDbContextPool<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=../RobinTTY.PersonalFinanceDashboard.Infrastructure/application.db"));

// Mappers
builder.Services.AddSingleton<TransactionMapper>();
builder.Services.AddSingleton<BankingInstitutionMapper>();

// TODO: automatic registration of repositories via codegen?
// Repositories
builder.Services
    .AddScoped<AccountRepository>()
    .AddScoped<AuthenticationRequestRepository>()
    .AddScoped<BankingInstitutionRepository>()
    .AddScoped<TransactionRepository>()
    .AddScoped<ThirdPartyDataRetrievalMetadataRepository>();
    
// Services
builder.Services
    .AddScoped<ThirdPartyDataRetrievalMetadataService>();

// Others
builder.Services
    .AddSingleton(new NordigenClientCredentials(appConfig.NordigenApi!.SecretId, appConfig.NordigenApi.SecretKey))
    .AddSingleton<GoCardlessDataProvider>();

// TODO: Create a high level overview of the architecture that should apply
//      - There can be many data providers
//      - Data providers could be configured via the front-end
//      - For the beginning it is smart to start with one provider to keep complexity low
//      - Since the authentication/retrieval logic will differ from provider to provider, it will be difficult
//        to abstract this logic away into one unified interface, so maybe I will need to refine/scrap this idea later...

// HotChocolate GraphQL setup
builder.Services
    // TODO: Configure cost analyzer at some point (enforces maximum query costs)
    .AddGraphQLServer(disableCostAnalyzer: true)
    // TODO: Document what the different extensions methods do
    .AddTypes()
    // AddQueryConventions: https://www.youtube.com/watch?v=yoW2Mt6C0Cg
    .AddQueryConventions()
    .AddMutationConventions();

var app = builder.Build();

// Apply database migrations at startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

app.UseSerilogRequestLogging();
app.UseCors();
app.MapGraphQL();
app.Run();
