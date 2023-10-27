using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.ThirdPartyDataProviders;

namespace RobinTTY.PersonalFinanceDashboard.API.Repositories;

/// <summary>
/// Manages <see cref="Account"/> data retrieval.
/// </summary>
public class AccountRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly GoCardlessDataProvider _dataProvider;

    public AccountRepository(ApplicationDbContext dbContext, GoCardlessDataProvider dataProvider)
    {
        _dbContext = dbContext;
        _dataProvider = dataProvider;
    }

    public async Task<BankAccount?> Get(string accountId)
    {
        var account = await _dataProvider.GetBankAccount(accountId);
        return account.Result;
    }

    public async Task<IEnumerable<BankAccount>> Get(IEnumerable<string> accountIds)
    {
        var accounts = await _dataProvider.GetBankAccounts(accountIds);
        return accounts.Select(query => query.Result)!;
    }
}