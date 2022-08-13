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

public class Query
{
    private readonly List<Book> _books = new()
    {
        new Book("C# in Depth: Fourth Edition", new Author("Jon Skeet")),
        new Book("Learning GraphQL: Declarative Data Fetching for Modern Web Apps", new Author("Eve Porcello and Alex Banks")),
        new Book("Code: The Hidden Language of Computer Hardware and Software", new Author("Charles Petzold"))
    };

    public List<Book> GetBooks() => _books;
    public Book? GetBook(string title) => _books.FirstOrDefault(x => x.Title == title);

    public Author? GetAuthor(string name) => _books.FirstOrDefault(x => x.Author.Name == name)?.Author;
}