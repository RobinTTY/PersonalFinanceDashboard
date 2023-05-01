global using System;
global using System.Linq;
global using System.Collections.Generic;
global using Autofac;
global using Serilog;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RobinTTY.PersonalFinanceDashboard.API.Resolvers;
using RobinTTY.PersonalFinanceDashboard.API.Utility;

var builder = WebApplication.CreateBuilder(args);

// Setup Autofac
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    containerBuilder.RegisterModule<ApplicationModule>();
});

// Add configuration parameters
builder.Configuration.AddConfiguration(AppConfigurationManager.GetApplicationConfiguration());

builder.Services.AddGraphQLServer()
    .AddQueryType<Query>();

var app = builder.Build();

// Sets up "/graphql" endpoint
app.UseRouting().UseEndpoints(endpointBuilder => endpointBuilder.MapGraphQL());

app.MapGet("/", () => "Hello World!");

app.Run();

public record Book(string Title, Author Author);
public record Author(string Name);
