global using System;
global using Autofac;
global using Serilog;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Autofac.Extensions.DependencyInjection;
using HotChocolate.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RobinTTY.PersonalFinanceDashboard.API;
using RobinTTY.PersonalFinanceDashboard.API.Utility;
using RobinTTY.PersonalFinanceDashboard.API.Models;

var builder = WebApplication.CreateBuilder(args);

// Setup Autofac
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Services.AddHttpClient();
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    containerBuilder.RegisterModule<ApplicationModule>();
});

// TODO: update to sensible policy
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

builder.Services.AddPooledDbContextFactory<ApplicationDbContext>(options => options.UseSqlite("Data Source=application.db"));
builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    //TODO: .AddMutationConventions()
    .AddSubscriptionType<Subscription>()
    .AddInMemorySubscriptions()
    .RegisterDbContext<ApplicationDbContext>(DbContextKind.Pooled);

var app = builder.Build();
app.UseCors();
app.UseWebSockets();
app.MapGraphQL();
app.Run();
