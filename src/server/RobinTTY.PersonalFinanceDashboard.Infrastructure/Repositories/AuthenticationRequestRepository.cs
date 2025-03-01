using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RobinTTY.NordigenApiClient.Models.Responses;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Extensions;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Services;
using RobinTTY.PersonalFinanceDashboard.ThirdPartyDataProviders;
using BankAccount = RobinTTY.PersonalFinanceDashboard.Core.Models.BankAccount;

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

        if (request.IsSuccessful)
        {
            await _dbContext.AuthenticationRequests.AddAsync(request.Result);
            await _dbContext.SaveChangesAsync();
            return request.Result;
        }

        _logger.LogError(
            "Creation of new authentication request failed. The data provider returned the following error: " +
            "\"{error}\" error details: \"{errorDetails}\"", request.Error.Summary, request.Error.Detail);

        //TODO: return error
        throw new NotImplementedException();
    }

    /// <summary>
    /// Deletes an existing <see cref="AuthenticationRequest"/>.
    /// </summary>
    /// <param name="id">The id of the <see cref="AuthenticationRequest"/> to delete.</param>
    /// <returns>The deleted <see cref="AuthenticationRequest"/>.</returns>
    public async Task<BasicResponse> DeleteAuthenticationRequest(Guid id)
    {
        var authRequest =
            await _dbContext.AuthenticationRequests.SingleOrDefaultAsync(authRequest => authRequest.Id == id);

        if (authRequest is null)
        {
            _logger.LogError(
                "Deletion of authentication request failed. Authentication request with id {id} does not exist.", id);
            throw new NotImplementedException();
        }

        var request = await _dataProviderService.DeleteAuthenticationRequest(authRequest.ThirdPartyId);
        if (request.IsSuccessful)
        {
            _dbContext.AuthenticationRequests.Remove(authRequest);
            await _dbContext.SaveChangesAsync();
            return request.Result;
        }

        _logger.LogError(
            "Deletion of authentication request failed. The data provider returned the following error: " +
            "\"{error}\" error details: \"{errorDetails}\"", request.Error.Summary, request.Error.Detail);
        throw new NotImplementedException();
    }

    /// <summary>
    /// Refreshes the list of authentication requests if the data has gone stale.
    /// </summary>
    private async Task RefreshAuthenticationRequestsIfStale()
    {
        var dataIsStale = await _dataRetrievalMetadataService.DataIsStale(ThirdPartyDataType.AuthenticationRequests);
        if (dataIsStale)
        {
            // TODO: Remove limit, use paging
            var response = await _dataProviderService.GetAuthenticationRequests(100);
            if (response.IsSuccessful)
            {
                var requests = response.Result.ToList();
                await SyncAuthenticationRequestEntities(requests);
                await _dataRetrievalMetadataService.ResetDataExpiry(ThirdPartyDataType.AuthenticationRequests);

                _logger.LogInformation(
                    "Refreshed stale authentication request data. {updateRecords} records were updated.",
                    response.Result.Count());
            }
            else
            {
                _logger.LogError(
                    "Refreshing stale authentication requests failed. Error summary: \"{message}\" Error details: \"{details}\"",
                    response.Error.Summary, response.Error.Detail);

                // TODO: What to do in case of failure should depend on if we already have data
                // Log failure and continue, maybe also send a notification to frontend, maybe through SignalR endpoint
            }
        }
    }

    /// <summary>
    /// Syncs the authentication requests the database contains with the external data provider.
    /// </summary>
    /// <param name="authenticationRequests">The list of <see cref="AuthenticationRequest"/>s to add.</param>
    private async Task SyncAuthenticationRequestEntities(List<AuthenticationRequest> authenticationRequests)
    {
        await AddOrUpdateAuthRequests(authenticationRequests);
        await DeleteOutdatedAuthenticationRequests(authenticationRequests);
    }

    /// <summary>
    /// Adds or updates authentication requests based on the information retrieved from the third party data provider.
    /// </summary>
    /// <param name="authRequests">The authentication requests retrieved from the third party data provider.</param>
    private async Task AddOrUpdateAuthRequests(List<AuthenticationRequest> authRequests)
    {
        foreach (var authenticationRequest in authRequests)
        {
            // We don't want to add the associated accounts here because they might already be in the db
            var associatedAccounts = authenticationRequest.AssociatedAccounts.ToList();
            authenticationRequest.AssociatedAccounts.Clear();

            await _dbContext.AddOrUpdateAuthenticationRequest(authenticationRequest);
            await LinkAssociatedAccountsToAuthRequests(authenticationRequest, associatedAccounts);
            await _dbContext.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Adds relationships between an authentication request and the associated accounts.
    /// </summary>
    /// <param name="authenticationRequest">The authentication request to which to link the accounts.</param>
    /// <param name="associatedAccounts">The associated accounts to link.</param>
    private async Task LinkAssociatedAccountsToAuthRequests(AuthenticationRequest authenticationRequest,
        List<BankAccount> associatedAccounts)
    {
        foreach (var associatedAccount in associatedAccounts)
        {
            var matchingAccount = await _dbContext.BankAccounts
                .SingleOrDefaultAsync(account => account.Id == associatedAccount.Id);

            if (matchingAccount is null)
            {
                var result = await _dbContext.BankAccounts.AddAsync(associatedAccount);
                matchingAccount = result.Entity;
            }

            authenticationRequest.AssociatedAccounts.Add(matchingAccount);
        }
    }

    /// <summary>
    /// Removes authentication requests which are no longer tracked by the third party data provider from the db.
    /// </summary>
    /// <param name="authRequests">The authentication requests as retrieved from the third party data provider.</param>
    private async Task DeleteOutdatedAuthenticationRequests(List<AuthenticationRequest> authRequests)
    {
        var updatedAuthReqIds = authRequests.Select(req => req.ThirdPartyId).ToList();
        await _dbContext.AuthenticationRequests
            .Where(dbReq => updatedAuthReqIds.All(updatedReqId => updatedReqId != dbReq.ThirdPartyId))
            .ExecuteDeleteAsync();
    }
}
