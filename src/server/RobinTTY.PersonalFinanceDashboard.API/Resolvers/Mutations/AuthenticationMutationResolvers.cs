using HotChocolate.Types;
using RobinTTY.NordigenApiClient.Models.Responses;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Repositories;

namespace RobinTTY.PersonalFinanceDashboard.Api.Resolvers.Mutations;

/// <summary>
/// <see cref="AuthenticationRequest"/> related mutation resolvers.
/// </summary>
[MutationType]
public class AuthenticationMutationResolvers
{
    /// <summary>
    /// Create a new authentication request for an institution.
    /// </summary>
    /// <param name="repository">The injected repository to use for data retrieval.</param>
    /// <param name="institutionId">The id of the institution which the authentication request should be created for.</param>
    /// <param name="redirectUri">The URI to redirect to after the authentication is completed.</param>
    // TODO: Exceptions
    // [Error<Exception>]
    public async Task<AuthenticationRequest> CreateAuthenticationRequest(AuthenticationRequestRepository repository, string institutionId, string redirectUri)
    {
        return await repository.AddAuthenticationRequest(institutionId, redirectUri);
    }

    /// <summary>
    /// Delete an existing authentication request.
    /// </summary>
    /// <param name="repository">The injected repository to use for data retrieval.</param>
    /// <param name="authenticationId">The id of the authentication request to delete.</param>
    public async Task<BasicResponse> DeleteAuthenticationRequest(AuthenticationRequestRepository repository, string authenticationId)
    {
        return await repository.DeleteAuthenticationRequest(authenticationId);
    }
}