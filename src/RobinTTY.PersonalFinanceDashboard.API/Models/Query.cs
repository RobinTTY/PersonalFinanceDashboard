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
    private readonly GoCardlessDataProvider _dataProvider;

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
}
