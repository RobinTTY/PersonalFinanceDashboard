using System.Threading.Tasks;
using HotChocolate.Types;
using RobinTTY.NordigenApiClient.Models.Responses;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.ThirdPartyDataProviders;

namespace RobinTTY.PersonalFinanceDashboard.API.Models.Mutations;

[ExtendObjectType(OperationTypeNames.Mutation)]
public class AuthenticationMutationResolvers
{
    private readonly GoCardlessDataProvider _dataProvider;

    public AuthenticationMutationResolvers(GoCardlessDataProvider dataProvider)
    {
        _dataProvider = dataProvider;
    }
    
    /// <summary>
    /// Create a new authentication request for an institution.
    /// </summary>
    /// <param name="institutionId">The id of the institution which the authentication request should be created for.</param>
    /// <param name="redirectUri">The URI to redirect to after the authentication is completed.</param>
    public async Task<AuthenticationRequest> CreateAuthenticationRequest(string institutionId, string redirectUri)
    {
        var request = await _dataProvider.CreateAuthenticationRequest(institutionId, new Uri(redirectUri));
        return request.Result!;
    }

    /// <summary>
    /// Delete an existing authentication request.
    /// </summary>
    /// <param name="authenticationId">The id of the authentication request to delete.</param>
    public async Task<BasicResponse> DeleteAuthenticationRequest(string authenticationId)
    {
        var request = await _dataProvider.DeleteAuthenticationRequest(authenticationId);
        return request.Result!;
    }
}