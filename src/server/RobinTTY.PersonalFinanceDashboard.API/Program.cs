global using System;
global using System.Linq;
global using System.Threading;
global using System.Collections.Generic;
global using System.Threading.Tasks;
global using Serilog;
global using HotChocolate;
using Microsoft.AspNetCore.Builder;
using RobinTTY.PersonalFinanceDashboard.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureSerilog();

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
