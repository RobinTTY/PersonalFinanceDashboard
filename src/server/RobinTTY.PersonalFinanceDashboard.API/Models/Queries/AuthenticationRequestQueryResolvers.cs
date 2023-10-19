using System.Linq;
using System.Threading.Tasks;
using HotChocolate.Types;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.ThirdPartyDataProviders;

namespace RobinTTY.PersonalFinanceDashboard.API.Models.Queries;

[ExtendObjectType(OperationTypeNames.Query)]
public class AuthenticationRequestQueryResolvers
{
    private readonly GoCardlessDataProvider _dataProvider;
    
    public AuthenticationRequestQueryResolvers(GoCardlessDataProvider dataProvider)
    {
        _dataProvider = dataProvider;
    }
    
    /// <summary>
    /// Look up authentication requests by their id.
    /// </summary>
    /// <param name="authenticationId">The id of the authentication request to retrieve.</param>
    public async Task<AuthenticationRequest?> GetAuthenticationRequest(string authenticationId)
    {
        var requests = await _dataProvider.GetAuthenticationRequest(authenticationId);
        return requests.Result!;
    }

    /// <summary>
    /// Look up authentication requests.
    /// </summary>
    public async Task<IQueryable<AuthenticationRequest>> GetAuthenticationRequests()
    {
        // TODO: Limit?
        var requests = await _dataProvider.GetAuthenticationRequests(100);
        return requests.Result!.AsQueryable();
    }
}