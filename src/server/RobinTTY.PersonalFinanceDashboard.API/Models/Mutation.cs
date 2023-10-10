using System.Linq;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.ThirdPartyDataProviders;
using System.Threading.Tasks;
using RobinTTY.NordigenApiClient.Models.Responses;
using RobinTTY.PersonalFinanceDashboard.API.Utility;
using Transaction = RobinTTY.PersonalFinanceDashboard.Core.Models.Transaction;

namespace RobinTTY.PersonalFinanceDashboard.API.Models;

/// <summary>
/// GraphQL root type for mutation operations.
/// </summary>
public class Mutation
{
    private readonly GoCardlessDataProvider _dataProvider;

    /// <summary>
    /// Creates a new instance of <see cref="Mutation"/>.
    /// </summary>
    /// <param name="dataProvider"></param>
    public Mutation(GoCardlessDataProvider dataProvider)
    {
        _dataProvider = dataProvider;
    }

    // TODO
    // https://relay.dev/docs/v1.5.0/graphql-server-specification/
    // 1. By convention, mutations are named as verbs (done)
    // 2. their inputs are the name with "Input" appended at the end
    // 3. they return an object that is the name with "Payload" appended

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

    /// <summary>
    /// Create a new transaction.
    /// </summary>
    public async Task<Transaction> CreateTransaction()
    {
        var transaction = MockDataAccessService.GetTransactions(1).First();
        return transaction;
    }
}
