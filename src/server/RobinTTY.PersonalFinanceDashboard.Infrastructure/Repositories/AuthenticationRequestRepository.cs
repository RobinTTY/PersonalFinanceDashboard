using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RobinTTY.PersonalFinanceDashboard.Core.DataProviders;
using RobinTTY.PersonalFinanceDashboard.Core.Models;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Repositories;

/// <summary>
/// Manages <see cref="AuthenticationRequest"/> data retrieval.
/// </summary>
public class AuthenticationRequestRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly BankDataProviderRegistry _providers;
    private readonly ILogger<AuthenticationRequestRepository> _logger;

    /// <summary>
    /// Creates a new instance of <see cref="AuthenticationRequestRepository"/>.
    /// </summary>
    /// <param name="dbContext">The <see cref="ApplicationDbContext"/> to use for data retrieval.</param>
    /// <param name="providers">The data providers authentication requests are made at.</param>
    /// <param name="logger">Logger used for monitoring purposes.</param>
    public AuthenticationRequestRepository(
        ApplicationDbContext dbContext,
        BankDataProviderRegistry providers,
        ILogger<AuthenticationRequestRepository> logger)
    {
        _dbContext = dbContext;
        _providers = providers;
        _logger = logger;
    }

    /// <summary>
    /// Gets the <see cref="AuthenticationRequest"/> matching the specified id.
    /// </summary>
    /// <param name="authenticationId">The id of the <see cref="AuthenticationRequest"/> to retrieve.</param>
    /// <returns>The <see cref="AuthenticationRequest"/> if one ist matched otherwise <see langword="null"/>.</returns>
    public IQueryable<AuthenticationRequest?> GetAuthenticationRequest(Guid authenticationId) =>
        _dbContext.AuthenticationRequests.Where(authentication => authentication.Id == authenticationId);

    /// <summary>
    /// Gets a list of <see cref="AuthenticationRequest"/>s.
    /// </summary>
    /// <returns>A list of <see cref="AuthenticationRequest"/>s.</returns>
    public IQueryable<AuthenticationRequest> GetAuthenticationRequests() => _dbContext.AuthenticationRequests;

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

        request.Result.Institution = institution;
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