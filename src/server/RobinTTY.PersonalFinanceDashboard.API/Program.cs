using RobinTTY.PersonalFinanceDashboard.Api.Extensions;
using RobinTTY.PersonalFinanceDashboard.Api.Utility;
using RobinTTY.PersonalFinanceDashboard.ThirdPartyDataProviders;

var builder = WebApplication.CreateBuilder(args);
var environment = builder.Environment;

builder.ConfigureSerilog();

builder.Services
    .AddDatabase(environment)
    .AddConfiguredHttpClient(builder.Configuration)
    .AddApplicationConfiguration()
    .AddApplicationServices()
    .AddGoCardless(AppConfiguration.GoCardlessSectionName)
    .AddRepositories()
    .AddGraphQlServices(environment)
    .AddThirdPartyDataSynchronizationServices();

var app = builder.Build();

// GraphQL commands (see below) don't use the database
if (args is not ["schema", ..])
    app.ApplyMigrations();
app.UseSerilogRequestLogging();
app.UseCors();
app.UseGraphQl();
app.MapHealthChecks("/health");

// Runs the server, or a GraphQL command such as: dotnet run -- schema export --output schema.graphql
// https://chillicream.com/docs/hotchocolate/v14/server/command-line
app.RunWithGraphQLCommands(args);
