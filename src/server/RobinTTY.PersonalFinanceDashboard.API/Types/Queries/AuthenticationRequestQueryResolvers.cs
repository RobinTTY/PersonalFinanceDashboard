using HotChocolate.Types;
using RobinTTY.PersonalFinanceDashboard.API.Repositories;
using RobinTTY.PersonalFinanceDashboard.Core.Models;

namespace RobinTTY.PersonalFinanceDashboard.Api.Types.Queries;

/// <summary>
/// <see cref="AuthenticationRequest"/> related query resolvers.
/// </summary>
[QueryType]
public class AuthenticationRequestQueryResolvers
{
    /// <summary>
    /// Look up authentication requests by their id.
    /// </summary>
    /// <param name="authenticationId">The id of the authentication request to retrieve.</param>
    /// <param name="repository">The repository to use for data retrieval.</param>
    public async Task<AuthenticationRequest> GetAuthenticationRequest(string authenticationId, AuthenticationRequestRepository repository)
    {
        return await repository.Get(authenticationId);
    }

    /// <summary>
    /// Look up authentication requests.
    /// </summary>
    /// <param name="repository">The repository to use for data retrieval.</param>
    public async Task<IEnumerable<AuthenticationRequest>> GetAuthenticationRequests(AuthenticationRequestRepository repository)
    {
        return await repository.GetAll();
    }
}