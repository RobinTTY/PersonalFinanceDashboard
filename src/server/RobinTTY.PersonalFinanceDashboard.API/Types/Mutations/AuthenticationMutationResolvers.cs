using HotChocolate.Types;
using RobinTTY.NordigenApiClient.Models.Responses;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Repositories;

namespace RobinTTY.PersonalFinanceDashboard.Api.Types.Mutations;

/// <summary>
/// <see cref="AuthenticationRequest"/> related mutation resolvers.
/// </summary>
[MutationType]
public class AuthenticationMutationResolvers
{
    /// <summary>
    /// Create a new authentication request for an institution.
    /// </summary>
    /// <param name="institutionId">The id of the institution which the authentication request should be created for.</param>
    /// <param name="redirectUri">The URI to redirect to after the authentication is completed.</param>
    /// <param name="repository">The repository to use for data retrieval.</param>
    // TODO: Exceptions
    // [Error<Exception>]
    public async Task<AuthenticationRequest> CreateAuthenticationRequest(string institutionId, string redirectUri, AuthenticationRequestRepository repository)
    {
        return await repository.Add(institutionId, redirectUri);
    }

    /// <summary>
    /// Delete an existing authentication request.
    /// </summary>
    /// <param name="authenticationId">The id of the authentication request to delete.</param>
    /// <param name="repository">The repository to use for data retrieval.</param>
    public async Task<BasicResponse> DeleteAuthenticationRequest(string authenticationId, AuthenticationRequestRepository repository)
    {
        return await repository.Delete(authenticationId);
    }
}