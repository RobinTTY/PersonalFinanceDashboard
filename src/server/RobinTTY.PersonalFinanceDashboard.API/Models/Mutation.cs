using System.Linq;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.ThirdPartyDataProviders;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Subscriptions;
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
    /// TODO
    /// </summary>
    /// <param name="institutionId"></param>
    /// <param name="redirectUri"></param>
    /// <returns></returns>
    public async Task<AuthenticationRequest> CreateAuthenticationRequest(string institutionId, string redirectUri)
    {
        var request = await _dataProvider.CreateAuthenticationRequest(institutionId, new Uri(redirectUri));
        return request.Result!;
    }

    public async Task<BasicResponse> DeleteAuthenticationRequest(string authenticationId)
    {
        var request = await _dataProvider.DeleteAuthenticationRequest(authenticationId);
        return request.Result!;
    }

    public async Task<Transaction> CreateTransaction([Service] ITopicEventSender topicEventSender)
    {
        var transaction = MockDataAccessService.GetTransactions(1).First();
        await topicEventSender.SendAsync(nameof(Subscription.TransactionCreated), transaction);
        return transaction;
    }
}
