using HotChocolate.Types;
using RobinTTY.PersonalFinanceDashboard.Api.Models;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Repositories;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Services.DataSynchronization.Interfaces;

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
    public async Task<AuthenticationRequest> CreateAuthenticationRequest(AuthenticationRequestRepository repository,
        string institutionId, string redirectUri)
    {
        if (!Uri.TryCreate(redirectUri, UriKind.Absolute, out var uri))
            throw MutationErrors.Create($"Redirect URI \"{redirectUri}\" is not a valid absolute URI.",
                ErrorCodes.InvalidRedirectUri);

        var result = await repository.AddAuthenticationRequest(institutionId, uri);
        if (result.IsSuccessful)
            return result.Result;

        throw MutationErrors.ThirdPartyRequestFailed(result.Error, "Failed to create authentication request.",
            ErrorCodes.CreateAuthenticationRequestFailed);
    }

    /// <summary>
    /// Delete an existing authentication request.
    /// </summary>
    /// <param name="repository">The injected repository to use for data retrieval.</param>
    /// <param name="authenticationId">The id of the authentication request to delete.</param>
    public async Task<DeleteResponse> DeleteAuthenticationRequest(AuthenticationRequestRepository repository,
        Guid authenticationId)
    {
        var deleteResult = await repository.DeleteAuthenticationRequest(authenticationId);

        if (deleteResult.IsSuccessful)
        {
            return new DeleteResponse
            {
                DeletedId = authenticationId
            };
        }

        throw MutationErrors.ThirdPartyRequestFailed(deleteResult.Error, "Failed to delete authentication request.",
            ErrorCodes.DeleteAuthenticationRequestFailed);
    }

    /// <summary>
    /// Synchronizes authentication request data with third-party data providers.
    /// </summary>
    /// <param name="syncHandler">The handler responsible for managing the data synchronization process.</param>
    /// <returns>A boolean value indicating whether the synchronization was successful.</returns>
    public async Task<bool> SynchronizeAuthenticationRequestData(IAuthenticationRequestSyncHandler syncHandler)
    {
        return await syncHandler.SynchronizeData(forceThirdPartySync: true);
    }
}