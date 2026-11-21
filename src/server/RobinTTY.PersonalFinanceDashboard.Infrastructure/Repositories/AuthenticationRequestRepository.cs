using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RobinTTY.PersonalFinanceDashboard.Core.DataProviders;
using RobinTTY.PersonalFinanceDashboard.Core.Extensions;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Services.DataSynchronization.Interfaces;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Repositories;

/// <summary>
/// Manages <see cref="AuthenticationRequest"/> data retrieval.
/// </summary>
public class AuthenticationRequestRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly BankDataProviderRegistry _providers;
    private readonly IAuthenticationRequestSyncHandler _authenticationRequestSyncHandler;
    private readonly IBankAccountSyncHandler _bankAccountSyncHandler;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<AuthenticationRequestRepository> _logger;

    /// <summary>
    /// Creates a new instance of <see cref="AuthenticationRequestRepository"/>.
    /// </summary>
    /// <param name="dbContext">The <see cref="ApplicationDbContext"/> to use for data retrieval.</param>
    /// <param name="providers">The data providers authentication requests are made at.</param>
    /// <param name="authenticationRequestSyncHandler">Handles the synchronization of authentication request data.</param>
    /// <param name="bankAccountSyncHandler">Handles the synchronization of the accounts of authentication requests.</param>
    /// <param name="timeProvider">Provides the current time.</param>
    /// <param name="logger">Logger used for monitoring purposes.</param>
    public AuthenticationRequestRepository(
        ApplicationDbContext dbContext,
        BankDataProviderRegistry providers,
        IAuthenticationRequestSyncHandler authenticationRequestSyncHandler,
        IBankAccountSyncHandler bankAccountSyncHandler,
        TimeProvider timeProvider,
        ILogger<AuthenticationRequestRepository> logger)
    {
        _dbContext = dbContext;
        _providers = providers;
        _authenticationRequestSyncHandler = authenticationRequestSyncHandler;
        _bankAccountSyncHandler = bankAccountSyncHandler;
        _timeProvider = timeProvider;
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
        await _authenticationRequestSyncHandler.SynchronizeData(authenticationId, true);
        await SynchronizeAccountsOfActiveRequest(authenticationId);
        return _dbContext.AuthenticationRequests.Where(authentication => authentication.Id == authenticationId);
    }

    /// <summary>
    /// Synchronizes the accounts of an active authentication request, e.g. right after the user granted access.
    /// Accounts of a reconnected bank thereby take the place of the earlier ones before they are returned.
    /// </summary>
    private async Task SynchronizeAccountsOfActiveRequest(Guid authenticationId)
    {
        var request = await _dbContext.AuthenticationRequests
            .Include(request => request.AssociatedAccounts)
            .SingleOrDefaultAsync(request => request.Id == authenticationId);
        if (request == null || !request.IsActive(_timeProvider.GetUtcNow().UtcDateTime))
            return;

        foreach (var accountId in request.AssociatedAccounts.Select(account => account.Id!.Value).ToList())
            await _bankAccountSyncHandler.SynchronizeData(accountId);
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
    /// <param name="institution">The institution to create a <see cref="AuthenticationRequest"/> for.</param>
    /// <param name="redirectUri">The URI of the page to redirect to after successful authentication.</param>
    /// <returns>The created <see cref="AuthenticationRequest"/>, or why it couldn't be created.</returns>
    public async Task<CreateAuthenticationRequestResult> AddAuthenticationRequest(BankingInstitution institution,
        Uri redirectUri)
    {
        // Institutions without a provider (e.g. manually created ones) can't be connected
        if (!_providers.TryGet(institution.Source, out var provider))
            return new CreateAuthenticationRequestResult.NotConnectable();

        var request = await provider.CreateAuthenticationRequest(institution.ExternalId!, redirectUri);

        if (!request.IsSuccessful)
        {
            _logger.LogError(
                "Creation of new authentication request failed. The data provider returned the following error: " +
                "\"{error}\" error details: \"{errorDetails}\"", request.Error.Summary, request.Error.Detail);
            return new CreateAuthenticationRequestResult.ProviderFailed(request.Error);
        }

        await _dbContext.AuthenticationRequests.AddAsync(request.Result);
        await _dbContext.SaveChangesAsync();

        return new CreateAuthenticationRequestResult.Created(request.Result);
    }

    /// <summary>
    /// Deletes an existing <see cref="AuthenticationRequest"/>.
    /// </summary>
    /// <param name="id">The id of the <see cref="AuthenticationRequest"/> to delete.</param>
    /// <returns>The error if the deletion failed, otherwise <see langword="null"/>.</returns>
    public async Task<ThirdPartyError?> DeleteAuthenticationRequest(Guid id)
    {
        var authRequest =
            await _dbContext.AuthenticationRequests.SingleOrDefaultAsync(authRequest => authRequest.Id == id);

        if (authRequest is null)
        {
            _logger.LogError(
                "Deletion of authentication request failed. Authentication request with id {id} does not exist.", id);

            return new ThirdPartyError("Authentication request not found.",
                $"Authentication request with id {id} does not exist.");
        }

        if (!_providers.TryGet(authRequest.Source, out var provider))
        {
            return new ThirdPartyError("Failed to delete authentication request.",
                $"No data provider is registered for {authRequest.Source}.");
        }

        var error = await provider.DeleteAuthenticationRequest(authRequest.ExternalId!);
        if (error is null)
        {
            _dbContext.AuthenticationRequests.Remove(authRequest);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Successfully deleted authentication request with id {id}", id);

            return null;
        }

        _logger.LogError(
            "Deletion of authentication request failed. The data provider returned the following error: " +
            "\"{error}\" error details: \"{errorDetails}\"", error.Summary, error.Detail);

        return new ThirdPartyError("Failed to delete authentication request.",
            $"The data provider returned the following error: {error.Message}");
    }
}