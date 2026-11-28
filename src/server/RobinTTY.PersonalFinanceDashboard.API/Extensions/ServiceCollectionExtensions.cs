using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RobinTTY.PersonalFinanceDashboard.Api.Resolvers.Subscriptions;
using RobinTTY.PersonalFinanceDashboard.Api.Utility;
using RobinTTY.PersonalFinanceDashboard.Core.DataProviders;
using RobinTTY.PersonalFinanceDashboard.Infrastructure;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Repositories;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Services;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Services.DataSynchronization;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Services.DataSynchronization.Interfaces;

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
                // Notifies clients of synchronized data
                .AddInMemorySubscriptions()
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
                .AddSingleton<BankDataProviderRegistry>();
        }

        /// <summary>
        /// Registers services required for synchronizing third-party data in the service collection.
        /// </summary>
        /// <returns>A reference to the <see cref="IServiceCollection"/> after the operation has completed.</returns>
        public IServiceCollection AddThirdPartyDataSynchronizationServices()
        {
            services.AddHostedService<DataSynchronizationBackgroundService>();

            return services
                .AddSingleton<DataSynchronizationLock>()
                .AddSingleton<DataSynchronizationTrigger>()
                .AddSingleton<IDataSynchronizationNotifier, SubscriptionDataSynchronizationNotifier>()
                .AddScoped<IDataSynchronizer, DataSynchronizer>()
                .AddScoped<IBankingInstitutionSyncHandler, BankingInstitutionSyncHandler>()
                .AddScoped<IAuthenticationRequestSyncHandler, AuthenticationRequestSyncHandler>()
                .AddScoped<IBankAccountSyncHandler, BankAccountSyncHandler>()
                .AddScoped<ITransactionSyncHandler, TransactionSyncHandler>()
                .AddScoped<ThirdPartyDataRetrievalMetadataService>()
                .AddScoped<BankAccountSyncStatusService>();
        }

        /// <summary>
        /// Adds all necessary configuration data for this application. Missing required settings stop the
        /// application on startup.
        /// </summary>
        /// <returns>A reference to the <see cref="IServiceCollection"/> after the operation has completed.</returns>
        public IServiceCollection AddApplicationConfiguration()
        {
            services.AddOptions<DatabaseConfiguration>()
                .BindConfiguration(DatabaseConfiguration.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();
            services.AddOptions<DataRetrievalConfiguration>()
                .BindConfiguration(AppConfiguration.DataRetrievalSectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            return services;
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
        /// Adds the database the application uses to the service collection, and a health check that it can be
        /// reached.
        /// </summary>
        /// <param name="environment">The environment options of the application.</param>
        /// <returns>A reference to the <see cref="IServiceCollection"/> after the operation has completed.</returns>
        public IServiceCollection AddDatabase(IWebHostEnvironment environment)
        {
            services.AddHealthChecks().AddDbContextCheck<ApplicationDbContext>();

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
