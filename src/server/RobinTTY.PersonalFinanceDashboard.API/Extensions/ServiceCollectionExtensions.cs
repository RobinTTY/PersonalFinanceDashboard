using System.Net.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RobinTTY.NordigenApiClient.Models;
using RobinTTY.PersonalFinanceDashboard.API.Utility;
using RobinTTY.PersonalFinanceDashboard.Infrastructure;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Repositories;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Services;
using RobinTTY.PersonalFinanceDashboard.ThirdPartyDataProviders;

namespace RobinTTY.PersonalFinanceDashboard.Api.Extensions;

/// <summary>
/// Provides extensions for the <see cref="IServiceCollection"/> interface.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all repositories in the service collection.
    /// </summary>
    /// <param name="services">The service collection to which to add the services.</param>
    /// <returns>A reference to the <see cref="IServiceCollection"/> after the operation has completed.</returns>
    // TODO: automatic registration of repositories via codegen?
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        return services
            .AddScoped<BankAccountRepository>()
            .AddScoped<AuthenticationRequestRepository>()
            .AddScoped<BankingInstitutionRepository>()
            .AddScoped<TransactionRepository>()
            .AddScoped<ThirdPartyDataRetrievalMetadataRepository>();
    }

    /// <summary>
    /// Registers all necessary HotChocolate (GraphQL) features in the service collection.
    /// </summary>
    /// <param name="services">The service collection to which to add the services.</param>
    /// <returns>A reference to the <see cref="IServiceCollection"/> after the operation has completed.</returns>
    public static IServiceCollection AddGraphQlServices(this IServiceCollection services)
    {
        var requestExecutorBuilder = services
            // TODO: Configure cost analyzer at some point (enforces maximum query costs)
            .AddGraphQLServer(disableCostAnalyzer: true)
            // Adds all GraphQL query and mutation types using the code generator (looks for attributes)
            .AddResolvers()
            // TODO: Document what the different extensions methods do
            // AddQueryConventions: https://www.youtube.com/watch?v=yoW2Mt6C0Cg
            .AddQueryConventions()
            .AddMutationConventions()
            // Enables efficient querying of the underlying db via projections
            .AddProjections()
            .ModifyOptions(options =>
            {
                options.StripLeadingIFromInterface = true;
            });

        return requestExecutorBuilder.Services;
    }

    /// <summary>
    /// Adds all necessary miscellaneous services for this application.
    /// </summary>
    /// <param name="services">The service collection to which to add the services.</param>
    /// <returns>A reference to the <see cref="IServiceCollection"/> after the operation has completed.</returns>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        return services
            .AddSingleton<GoCardlessDataProviderService>()
            .AddScoped<ThirdPartyDataRetrievalMetadataService>();
    }

    /// <summary>
    /// Adds all necessary configuration data for this application.
    /// </summary>
    /// <param name="services">The service collection to which to add the services.</param>
    /// <returns>A reference to the <see cref="IServiceCollection"/> after the operation has completed.</returns>
    public static IServiceCollection AddApplicationConfiguration(this IServiceCollection services)
    {
        var appConfig = AppConfigurationManager.AppConfiguration;
        return services.AddSingleton(new NordigenClientCredentials(appConfig.NordigenApi!.SecretId,
            appConfig.NordigenApi.SecretKey));
    }

    /// <summary>
    /// Adds a configured <see cref="IHttpClientFactory"/> to the service collection. 
    /// </summary>
    /// <param name="services">The service collection to which to add the services.</param>
    /// <returns>A reference to the <see cref="IServiceCollection"/> after the operation has completed.</returns>
    public static IServiceCollection AddConfiguredHttpClient(this IServiceCollection services)
    {
        return services
            .AddHttpClient()
            .AddCors(options =>
            {
                // TODO: update to sensible policy
                options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            });
    }

    /// <summary>
    /// Adds the database the application uses to the service collection.
    /// </summary>
    /// <param name="services">The service collection to which to add the services.</param>
    /// <returns>A reference to the <see cref="IServiceCollection"/> after the operation has completed.</returns>
    public static IServiceCollection AddDatabase(this IServiceCollection services)
    {
        // TODO: The filepath shouldn't be hardcoded => configuration
        return services.AddDbContextPool<ApplicationDbContext>(options =>
            options.UseSqlite("Data Source=../RobinTTY.PersonalFinanceDashboard.Infrastructure/application.db"));
    }
}
