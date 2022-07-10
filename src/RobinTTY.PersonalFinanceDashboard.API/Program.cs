global using System;
global using Autofac;
global using Serilog;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Autofac.Extensions.DependencyInjection;
using RobinTTY.PersonalFinanceDashboard.API.Utility;

var builder = WebApplication.CreateBuilder(args);

// Setup Autofac
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    containerBuilder.RegisterModule<ApplicationModule>();
});

// Add services


var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
