using HotChocolate.Types;
using RobinTTY.PersonalFinanceDashboard.API.Repositories;
using RobinTTY.PersonalFinanceDashboard.Core.Models;

namespace RobinTTY.PersonalFinanceDashboard.Api.Types.Queries;

[QueryType]
public class AuthenticationRequestQueryResolvers
{
    /// <summary>
    /// Look up authentication requests by their id.
    /// </summary>
    /// <param name="authenticationId">The id of the authentication request to retrieve.</param>
    public async Task<AuthenticationRequest?> GetAuthenticationRequest(string authenticationId, AuthenticationRequestRepository repository)
    {
        return await repository.Get(authenticationId);
    }

    /// <summary>
    /// Look up authentication requests.
    /// </summary>
    public async Task<IEnumerable<AuthenticationRequest>> GetAuthenticationRequests(AuthenticationRequestRepository repository)
    {
        return await repository.GetAll();
    }
}