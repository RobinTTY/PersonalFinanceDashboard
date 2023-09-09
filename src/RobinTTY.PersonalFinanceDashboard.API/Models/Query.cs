using System.Linq;
using System.Threading.Tasks;
using HotChocolate.Types;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.Database.Mock;
using RobinTTY.PersonalFinanceDashboard.ThirdPartyDataProviders;

namespace RobinTTY.PersonalFinanceDashboard.API.Models;

/// <summary>
/// GraphQL root type for query operations.
/// </summary>
public class Query
{
    readonly GoCardlessDataProvider _dataProvider;

    public Query(GoCardlessDataProvider dataProvider)
    {
        _dataProvider = dataProvider;
    }

    [UsePaging]
    public IQueryable<Account> GetAccounts() => MockDataAccessService.GetAccounts(100);

    [UsePaging]
    public IQueryable<Transaction> GetTransactions() => MockDataAccessService.GetTransactions(100);

    // TODO: Investigate how to handle the return type (result/error) properly with paging
    [UsePaging]
    public IQueryable<BankingInstitution> GetBankingInstitutions() => _dataProvider.GetBankingInstitutions().GetAwaiter().GetResult().Result!;

    /// <summary>
    /// TODO
    /// </summary>
    /// <returns></returns>
    public async Task<IQueryable<AuthenticationRequest>> GetAuthenticationRequests()
    {
        // TODO: Limit?
        var requests = await _dataProvider.GetAuthenticationRequests(100);
        return requests.Result!.AsQueryable();
    }

    /// <summary>
    /// TODO
    /// </summary>
    /// <param name="institutionId"></param>
    /// <param name="redirectUri"></param>
    /// <returns></returns>
    public async Task<AuthenticationRequest> GetNewAuthenticationRequest(string institutionId, string redirectUri)
    {
        var request = await _dataProvider.GetNewAuthenticationRequest(institutionId, new Uri(redirectUri));
        return request.Result!;
    }
}
