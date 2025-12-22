using Microsoft.AspNetCore.Builder;
using RobinTTY.PersonalFinanceDashboard.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);
var environment = builder.Environment;

builder.ConfigureSerilog();

builder.Services
    .AddDatabase(environment)
    .AddConfiguredHttpClient()
    .AddApplicationConfiguration()
    .AddApplicationServices()
    .AddRepositories()
    .AddGraphQlServices()
    .AddThirdPartyDataSynchronizationServices();

var app = builder.Build();

app.ApplyMigrations();
app.UseSerilogRequestLogging();
app.UseCors();
app.UseGraphQl(args);
app.Run();
