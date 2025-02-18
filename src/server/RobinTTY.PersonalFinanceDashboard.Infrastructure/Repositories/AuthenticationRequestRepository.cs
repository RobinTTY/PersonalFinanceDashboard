using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RobinTTY.NordigenApiClient.Models.Responses;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Extensions;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Services;
using RobinTTY.PersonalFinanceDashboard.ThirdPartyDataProviders;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Repositories;

/// <summary>
/// Manages <see cref="AuthenticationRequest"/> data retrieval.
/// </summary>
public class AuthenticationRequestRepository
{
    private readonly ILogger<AuthenticationRequestRepository> _logger;
    private readonly ApplicationDbContext _dbContext;
    private readonly GoCardlessDataProviderService _dataProviderService;
    private readonly ThirdPartyDataRetrievalMetadataService _dataRetrievalMetadataService;

    /// <summary>
    /// Creates a new instance of <see cref="AuthenticationRequestRepository"/>.
    /// </summary>
    /// <param name="logger">Logger used for monitoring purposes.</param>
    /// <param name="dbContext">The <see cref="ApplicationDbContext"/> to use for data retrieval.</param>
    /// <param name="dataProviderService">The data provider to use for data retrieval.</param>
    /// <param name="dataRetrievalMetadataService">Service used to determine if the database data is stale.</param>
    public AuthenticationRequestRepository(
        ILogger<AuthenticationRequestRepository> logger,
        ApplicationDbContext dbContext,
        GoCardlessDataProviderService dataProviderService,
        ThirdPartyDataRetrievalMetadataService dataRetrievalMetadataService)
    {
        _logger = logger;
        _dbContext = dbContext;
        _dataProviderService = dataProviderService;
        _dataRetrievalMetadataService = dataRetrievalMetadataService;
    }

    /// <summary>
    /// Gets the <see cref="AuthenticationRequest"/> matching the specified id.
    /// </summary>
    /// <param name="authenticationId">The id of the <see cref="AuthenticationRequest"/> to retrieve.</param>
    /// <returns>The <see cref="AuthenticationRequest"/> if one ist matched otherwise <see langword="null"/>.</returns>
    public async Task<IQueryable<AuthenticationRequest?>> GetAuthenticationRequest(Guid authenticationId)
    {
        await RefreshAuthenticationRequestsIfStale();

        return _dbContext.AuthenticationRequests.Where(authentication => authentication.Id == authenticationId);
    }

    /// <summary>
    /// Gets a list of <see cref="AuthenticationRequest"/>s.
    /// </summary>
    /// <returns>A list of <see cref="AuthenticationRequest"/>s.</returns>
    public async Task<IQueryable<AuthenticationRequest>> GetAuthenticationRequests()
    {
        await RefreshAuthenticationRequestsIfStale();

        return _dbContext.AuthenticationRequests;
    }

    /// <summary>
    /// Adds a new <see cref="AuthenticationRequest"/>.
    /// </summary>
    /// <param name="institutionId">The id of the institution to create a <see cref="AuthenticationRequest"/> for.</param>
    /// <param name="redirectUri">The URI of the page to redirect to after successful authentication.</param>
    /// <returns>The added <see cref="AuthenticationRequest"/>.</returns>
    public async Task<AuthenticationRequest> AddAuthenticationRequest(string institutionId, string redirectUri)
    {
        // TODO: URI validation (here or in resolver?)
        var request = await _dataProviderService.CreateAuthenticationRequest(institutionId, new Uri(redirectUri));
        return request.Result!;
    }

    /// <summary>
    /// Deletes an existing <see cref="AuthenticationRequest"/>.
    /// </summary>
    /// <param name="authenticationId">The id of the <see cref="AuthenticationRequest"/> to delete.</param>
    /// <returns>TODO</returns>
    public async Task<BasicResponse> DeleteAuthenticationRequest(string authenticationId)
    {
        var request = await _dataProviderService.DeleteAuthenticationRequest(authenticationId);
        return request.Result!;
    }

    /// <summary>
    /// Adds a list of new <see cref="AuthenticationRequest"/>s that were retrieved by a third party data retrieval service.
    /// </summary>
    /// <param name="authenticationRequests">The list of <see cref="AuthenticationRequest"/>s to add.</param>
    private async Task AddOrUpdateExternalAuthenticationRequests(
        IEnumerable<AuthenticationRequest> authenticationRequests)
    {
        foreach (var authenticationRequest in authenticationRequests)
        {
            // We don't want to add the associated accounts here because they might already be in the db
            var associatedAccounts = authenticationRequest.AssociatedAccounts.ToList();
            authenticationRequest.AssociatedAccounts.Clear();
            _dbContext.InsertOrUpdate(authenticationRequest);
            await _dbContext.SaveChangesAsync();

            // Add the links to the associated accounts and add any that aren't already in the db
            foreach (var associatedAccount in associatedAccounts)
            {
                var matchingAccount = await _dbContext.BankAccounts
                    .SingleOrDefaultAsync(account => account.Id == associatedAccount.Id);

                if (matchingAccount is null)
                {
                    var result = await _dbContext.BankAccounts.AddAsync(associatedAccount);
                    await _dbContext.SaveChangesAsync();
                    matchingAccount = result.Entity;
                }

                authenticationRequest.AssociatedAccounts.Add(matchingAccount);
            }
        }

        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Deletes all existing <see cref="AuthenticationRequest"/>s.
    /// </summary>
    /// <returns>The number of deleted records.</returns>
    private async Task<int> DeleteAuthenticationRequests()
    {
        return await _dbContext.AuthenticationRequests.ExecuteDeleteAsync();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <exception cref="NotImplementedException">TODO</exception>
    // TODO: This is basically the same logic as other repositories with external data
    // Can this be generalized enough even with small differences in the way the database is updated?
    // e.g. full deletion of current data and reinsert vs upsert...
    private async Task RefreshAuthenticationRequestsIfStale()
    {
        var dataIsStale = await _dataRetrievalMetadataService.DataIsStale(ThirdPartyDataType.AuthenticationRequests);
        if (dataIsStale)
        {
            var response = await _dataProviderService.GetAuthenticationRequests(100);
            if (response.IsSuccessful)
            {
                await AddOrUpdateExternalAuthenticationRequests(response.Result);
                await _dataRetrievalMetadataService.ResetDataExpiry(ThirdPartyDataType.AuthenticationRequests);

                _logger.LogInformation(
                    "Refreshed stale Authentication request data. {updateRecords} records were updated.",
                    response.Result.Count());
            }
            else
            {
                // TODO: What to do in case of failure should depend on if we already have data
                // Log failure and continue, maybe also send a notification to frontend
                throw new NotImplementedException();
            }
        }
    }
}
