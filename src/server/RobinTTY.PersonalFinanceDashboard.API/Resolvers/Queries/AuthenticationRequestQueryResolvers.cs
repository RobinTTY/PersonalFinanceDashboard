using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Repositories;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Services.DataSynchronization.Interfaces;

namespace RobinTTY.PersonalFinanceDashboard.Api.Resolvers.Queries;

/// <summary>
/// <see cref="AuthenticationRequest"/> related query resolvers.
/// </summary>
[QueryType]
public class AuthenticationRequestQueryResolvers
{
    /// <summary>
    /// Look up authentication requests by their id.
    /// </summary>
    /// <param name="repository">The injected repository to use for data retrieval.</param>
    /// <param name="synchronizer">Retrieves the authentication request from its data provider.</param>
    /// <param name="authenticationId">The id of the authentication request to retrieve.</param>
    /// <param name="refresh">
    /// Whether to retrieve the authentication request from its data provider first, e.g. while waiting for the user
    /// to grant access. Once access was granted, its accounts are retrieved too.
    /// </param>
    /// <param name="cancellationToken">A token to signal cancellation.</param>
    [UseSingleOrDefault]
    [UseProjection]
    public async Task<IQueryable<AuthenticationRequest?>> GetAuthenticationRequest(
        AuthenticationRequestRepository repository, IDataSynchronizer synchronizer, Guid authenticationId,
        CancellationToken cancellationToken, bool refresh = false)
    {
        if (refresh)
            await synchronizer.RefreshAuthenticationRequest(authenticationId, cancellationToken);

        return repository.GetAuthenticationRequest(authenticationId);
    }

    /// <summary>
    /// Look up authentication requests.
    /// </summary>
    /// <param name="repository">The injected repository to use for data retrieval.</param>
    [UseProjection]
    public IQueryable<AuthenticationRequest> GetAuthenticationRequests(AuthenticationRequestRepository repository) =>
        repository.GetAuthenticationRequests();
}
