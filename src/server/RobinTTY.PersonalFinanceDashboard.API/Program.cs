using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using RobinTTY.PersonalFinanceDashboard.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);
var environment = builder.Environment;

builder.ConfigureSerilog();

builder.Services
    .AddDatabase(environment)
    .AddConfiguredHttpClient(builder.Configuration)
    .AddApplicationConfiguration()
    .AddApplicationServices()
    .AddRepositories()
    .AddGraphQlServices()
    .AddThirdPartyDataSynchronizationServices();

var app = builder.Build();

app.ApplyMigrations();
app.UseSerilogRequestLogging();
app.UseCors();
app.UseGraphQl();

// Runs the server, or a GraphQL command such as: dotnet run -- schema export --output schema.graphql
// https://chillicream.com/docs/hotchocolate/v14/server/command-line
app.RunWithGraphQLCommands(args);