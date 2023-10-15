using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate.Types;
using RobinTTY.PersonalFinanceDashboard.API.Repositories;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.ThirdPartyDataProviders;

namespace RobinTTY.PersonalFinanceDashboard.API.Models;

// TODO: This implementation is very barebones right now and needs error handling etc.
/// <summary>
/// GraphQL root type for query operations.
/// </summary>
public class Query
{
    private readonly GoCardlessDataProvider _dataProvider;

    /// <summary>
    /// Create a new instance of <see cref="Query"/>.
    /// </summary>
    /// <param name="dataProvider">The <see cref="GoCardlessDataProvider"/> to inject.</param>
    public Query(GoCardlessDataProvider dataProvider)
    {
        _dataProvider = dataProvider;
    }

    // TODO: Create a high level overview of the architecture that should apply here
    //      - There can be many data providers
    //      - Data providers could be configured via the front-end
    //      - For the beginning it is smart to start with one provider to keep complexity low
    //      - Since the authentication/retrieval logic will differ from provider to provider, it will be difficult
    //        to abstract this logic away into one unified interface, so maybe I will need to refine/scrap this idea later...

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

    /// <summary>
    /// Look up transactions of an account.
    /// </summary>
    /// <param name="accountId">The id of the account to retrieve.</param>
    [UsePaging]
    public async Task<IReadOnlyList<Transaction>> GetTransactions(string accountId, TransactionRepository repository, CancellationToken cancellationToken)
    {
        return await repository.GetTransactions();
    }

    /// <summary>
    /// Look up banking institutions by their id.
    /// </summary>
    /// <param name="institutionId">The id of the banking institution to retrieve.</param>
    public async Task<BankingInstitution> GetBankingInstitution(string institutionId)
    {
        var request = await _dataProvider.GetBankingInstitution(institutionId);
        return request.Result!;
    }

    /// <summary>
    /// Look up banking institutions.
    /// </summary>
    /// <param name="countryCode">Optional filter by country the institution operates in.</param>
    [UsePaging(MaxPageSize = 3000)]
    public async Task<IQueryable<BankingInstitution>> GetBankingInstitutions(string? countryCode = null) {
        var request = await _dataProvider.GetBankingInstitutions(countryCode);
        return request.Result!;
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
