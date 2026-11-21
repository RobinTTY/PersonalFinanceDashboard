using System.Diagnostics;
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
    /// <param name="institutionRepository">The injected repository to look up the institution with.</param>
    /// <param name="institutionId">The id of the institution which the authentication request should be created for.</param>
    /// <param name="redirectUri">The URI to redirect to after the authentication is completed.</param>
    public async Task<AuthenticationRequest> CreateAuthenticationRequest(AuthenticationRequestRepository repository,
        BankingInstitutionRepository institutionRepository, Guid institutionId, string redirectUri)
    {
        if (!Uri.TryCreate(redirectUri, UriKind.Absolute, out var uri))
            throw MutationErrors.Create($"Redirect URI \"{redirectUri}\" is not a valid absolute URI.",
                ErrorCodes.InvalidRedirectUri);

        var institution = await institutionRepository.FindBankingInstitution(institutionId)
                          ?? throw MutationErrors.NotFound<BankingInstitution>(institutionId,
                              ErrorCodes.BankingInstitutionNotFound);

        var createResult = await repository.AddAuthenticationRequest(institution, uri);
        return createResult switch
        {
            CreateAuthenticationRequestResult.Created created => created.Request,
            CreateAuthenticationRequestResult.NotConnectable => throw MutationErrors.Create(
                $"{institution.Name} can't be connected to a bank data provider.",
                ErrorCodes.InstitutionNotConnectable),
            CreateAuthenticationRequestResult.ProviderFailed failed => throw MutationErrors.ThirdPartyRequestFailed(
                failed.Error, ErrorCodes.CreateAuthenticationRequestFailed),

            var result => throw new UnreachableException($"Unhandled result {result}.")
        };
    }

    /// <summary>
    /// Delete an existing authentication request.
    /// </summary>
    /// <param name="repository">The injected repository to use for data retrieval.</param>
    /// <param name="authenticationId">The id of the authentication request to delete.</param>
    public async Task<DeleteResponse> DeleteAuthenticationRequest(AuthenticationRequestRepository repository,
        Guid authenticationId)
    {
        var error = await repository.DeleteAuthenticationRequest(authenticationId);
        if (error is null)
            return new DeleteResponse { DeletedId = authenticationId };

        throw MutationErrors.ThirdPartyRequestFailed(error, ErrorCodes.DeleteAuthenticationRequestFailed);
    }

    /// <summary>
    /// Synchronizes authentication request data with third-party data providers.
    /// </summary>
    /// <param name="synchronizer">Runs the synchronization.</param>
    /// <returns>A boolean value indicating whether the synchronization was successful.</returns>
    public async Task<bool> SynchronizeAuthenticationRequestData(IDataSynchronizer synchronizer)
    {
        return await synchronizer.Synchronize(ThirdPartyDataType.AuthenticationRequests, forceThirdPartySync: true);
    }
}