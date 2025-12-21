using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RobinTTY.NordigenApiClient.Models.Responses;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Services.DataSynchronization;
using RobinTTY.PersonalFinanceDashboard.ThirdPartyDataProviders;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Repositories;

/// <summary>
/// Manages <see cref="AuthenticationRequest"/> data retrieval.
/// </summary>
public class AuthenticationRequestRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly GoCardlessDataProviderService _dataProviderService;
    private readonly AuthenticationRequestSyncHandler _authenticationRequestSyncHandler;
    private readonly ILogger<AuthenticationRequestRepository> _logger;


    /// <summary>
    /// Creates a new instance of <see cref="AuthenticationRequestRepository"/>.
    /// </summary>
    /// <param name="dbContext">The <see cref="ApplicationDbContext"/> to use for data retrieval.</param>
    /// <param name="dataProviderService">The data provider to use for data retrieval.</param>
    /// <param name="authenticationRequestSyncHandler"></param>
    /// <param name="logger">Logger used for monitoring purposes.</param>
    public AuthenticationRequestRepository(
        ApplicationDbContext dbContext,
        GoCardlessDataProviderService dataProviderService,
        AuthenticationRequestSyncHandler authenticationRequestSyncHandler,
        ILogger<AuthenticationRequestRepository> logger)
    {
        _dbContext = dbContext;
        _dataProviderService = dataProviderService;
        _authenticationRequestSyncHandler = authenticationRequestSyncHandler;
        _logger = logger;
    }

    /// <summary>
    /// Gets the <see cref="AuthenticationRequest"/> matching the specified id.
    /// </summary>
    /// <param name="authenticationId">The id of the <see cref="AuthenticationRequest"/> to retrieve.</param>
    /// <returns>The <see cref="AuthenticationRequest"/> if one ist matched otherwise <see langword="null"/>.</returns>
    public async Task<IQueryable<AuthenticationRequest?>> GetAuthenticationRequest(Guid authenticationId)
    {
        // TODO: Maybe let the API consumer choose whether to force the third party data sync
        await _authenticationRequestSyncHandler.SynchronizeData(true);

        return _dbContext.AuthenticationRequests.Where(authentication => authentication.Id == authenticationId);
    }

    /// <summary>
    /// Gets a list of <see cref="AuthenticationRequest"/>s.
    /// </summary>
    /// <returns>A list of <see cref="AuthenticationRequest"/>s.</returns>
    public async Task<IQueryable<AuthenticationRequest>> GetAuthenticationRequests()
    {
        await _authenticationRequestSyncHandler.SynchronizeData();

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
}
