using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RobinTTY.NordigenApiClient;
using RobinTTY.NordigenApiClient.Models;
using RobinTTY.PersonalFinanceDashboard.Api.Utility;
using RobinTTY.PersonalFinanceDashboard.Infrastructure;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Repositories;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Services;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Services.DataSynchronization;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Services.DataSynchronization.Interfaces;
using RobinTTY.PersonalFinanceDashboard.ThirdPartyDataProviders;

namespace RobinTTY.PersonalFinanceDashboard.Api.Extensions;

/// <summary>
/// Provides extensions for the <see cref="IServiceCollection"/> interface.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <param name="services">The service collection to which to add the services.</param>
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Registers all repositories in the service collection.
        /// </summary>
        /// <returns>A reference to the <see cref="IServiceCollection"/> after the operation has completed.</returns>
        public IServiceCollection AddRepositories()
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
        /// <returns>A reference to the <see cref="IServiceCollection"/> after the operation has completed.</returns>
        public IServiceCollection AddGraphQlServices()
        {
            var requestExecutorBuilder = services
                .AddGraphQLServer()
                // Adds all GraphQL query and mutation types using the code generator (looks for attributes)
                .AddResolvers()
                // Enables returning FieldResult type errors
                .AddQueryConventions()
                .AddMutationConventions()
                // Enables efficient querying of the underlying db via projections
                .AddProjections()
                // Enables filtering of query results (e.g. filtering based on a certain property)
                .AddFiltering()
                .ModifyOptions(options => { options.StripLeadingIFromInterface = true; })
                // TODO: Configure cost analyzer at some point (enforces maximum query costs)
                .ModifyCostOptions(options => { options.EnforceCostLimits = false; });

            return requestExecutorBuilder.Services;
        }

        /// <summary>
        /// Adds all necessary miscellaneous services for this application.
        /// </summary>
        /// <returns>A reference to the <see cref="IServiceCollection"/> after the operation has completed.</returns>
        public IServiceCollection AddApplicationServices()
        {
            return services
                .AddSingleton(TimeProvider.System)
                .AddSingleton<NordigenClient>()
                .AddSingleton<GoCardlessDataProviderService>();
        }

        /// <summary>
        /// Registers services required for synchronizing third-party data in the service collection.
        /// </summary>
        /// <returns>A reference to the <see cref="IServiceCollection"/> after the operation has completed.</returns>
        public IServiceCollection AddThirdPartyDataSynchronizationServices()
        {
            return services
                .AddScoped<IBankingInstitutionSyncHandler, BankingInstitutionSyncHandler>()
                .AddScoped<IAuthenticationRequestSyncHandler, AuthenticationRequestSyncHandler>()
                .AddScoped<IBankAccountSyncHandler, BankAccountSyncHandler>()
                .AddScoped<ITransactionSyncHandler, TransactionSyncHandler>()
                .AddScoped<ThirdPartyDataRetrievalMetadataService>();
        }

        /// <summary>
        /// Adds all necessary configuration data for this application. Missing required settings stop the
        /// application on startup.
        /// </summary>
        /// <returns>A reference to the <see cref="IServiceCollection"/> after the operation has completed.</returns>
        public IServiceCollection AddApplicationConfiguration()
        {
            services.AddOptions<NordigenApiClientConfiguration>()
                .BindConfiguration(NordigenApiClientConfiguration.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddOptions<DatabaseConfiguration>()
                .BindConfiguration(DatabaseConfiguration.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            return services.AddSingleton(provider =>
            {
                var config = provider.GetRequiredService<IOptions<NordigenApiClientConfiguration>>().Value;
                return new NordigenClientCredentials(config.SecretId, config.SecretKey);
            });
        }

        /// <summary>
        /// Adds a configured <see cref="IHttpClientFactory"/> to the service collection.
        /// </summary>
        /// <param name="configuration">The configuration of the application.</param>
        /// <returns>A reference to the <see cref="IServiceCollection"/> after the operation has completed.</returns>
        public IServiceCollection AddConfiguredHttpClient(IConfiguration configuration)
        {
            var appConfig = configuration.GetSection(AppConfiguration.SectionName).Get<AppConfiguration>();

            return services
                .AddHttpClient()
                .AddCors(options =>
                {
                    var allowedOrigins = appConfig?.AllowedCorsOrigins is { Length: > 0 } origins
                        ? origins
                        : ["http://localhost:5173"];
                    options.AddDefaultPolicy(policy =>
                        policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod());
                });
        }

        /// <summary>
        /// Adds the database the application uses to the service collection.
        /// </summary>
        /// <param name="environment">The environment options of the application.</param>
        /// <returns>A reference to the <see cref="IServiceCollection"/> after the operation has completed.</returns>
        public IServiceCollection AddDatabase(IWebHostEnvironment environment)
        {
            return services.AddDbContextPool<ApplicationDbContext>((provider, options) =>
            {
                var config = provider.GetRequiredService<IOptions<DatabaseConfiguration>>().Value;
                options.UseSqlite(config.ConnectionString);

                if (environment.IsDevelopment())
                    options.EnableDetailedErrors();
            });
        }
    }
}