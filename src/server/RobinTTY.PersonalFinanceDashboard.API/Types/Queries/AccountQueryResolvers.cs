using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotChocolate.Types;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.ThirdPartyDataProviders;

namespace RobinTTY.PersonalFinanceDashboard.Api.Types.Queries;

[QueryType]
public sealed class AccountQueryResolvers
{
    private readonly GoCardlessDataProvider _dataProvider;

    public AccountQueryResolvers(GoCardlessDataProvider dataProvider)
    {
        _dataProvider = dataProvider;
    }

    // TODO: probably should be GetBankAccount and then separate route for investment accounts
    /// <summary>
    /// Look up an account by its id.
    /// </summary>
    /// <param name="accountId">The id of the account to lookup.</param>
    public async Task<BankAccount?> GetAccount(string accountId)
    {
        var account = await _dataProvider.GetBankAccount(accountId);
        return account.Result;
    }

    /// <summary>
    /// Look up accounts by a list of ids.
    /// </summary>
    /// <param name="accountIds">The ids of the accounts to retrieve.</param>
    [UsePaging]
    public async Task<IQueryable<BankAccount>> GetAccounts(IEnumerable<string> accountIds)
    {
        var accounts = await _dataProvider.GetBankAccounts(accountIds);
        return accounts.Select(query => query.Result).AsQueryable()!;
    }
}
