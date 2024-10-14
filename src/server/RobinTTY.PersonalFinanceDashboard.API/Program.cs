using Microsoft.AspNetCore.Builder;
using RobinTTY.PersonalFinanceDashboard.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.ConfigureSerilog();

builder.Services.AddDatabase();
builder.Services.AddConfiguredHttpClient();
builder.Services.AddApplicationConfiguration();
builder.Services.AddApplicationServices();
builder.Services.AddEntityMappers();
builder.Services.AddRepositories();
builder.Services.AddGraphQlServices();

var app = builder.Build();

app.ApplyMigrations();
app.UseSerilogRequestLogging();
app.UseCors();
app.UseGraphQl(args);
app.Run();
