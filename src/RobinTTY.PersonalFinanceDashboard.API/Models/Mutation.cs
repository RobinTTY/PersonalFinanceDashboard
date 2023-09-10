using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.ThirdPartyDataProviders;
using System.Threading.Tasks;

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
}
