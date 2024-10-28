using Microsoft.AspNetCore.Builder;
using RobinTTY.PersonalFinanceDashboard.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.ConfigureSerilog();

builder.Services
    .AddDatabase()
    .AddConfiguredHttpClient()
    .AddApplicationConfiguration()
    .AddApplicationServices()
    .AddEntityMappers()
    .AddRepositories()
    .AddGraphQlServices();

var app = builder.Build();

app.ApplyMigrations();
app.UseSerilogRequestLogging();
app.UseCors();
app.UseGraphQl(args);
app.Run();
