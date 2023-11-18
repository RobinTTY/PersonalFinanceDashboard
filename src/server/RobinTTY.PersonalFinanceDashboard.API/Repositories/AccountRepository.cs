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

    /// <summary>
    /// Creates a new instance of <see cref="AccountRepository"/>.
    /// </summary>
    /// <param name="dbContext">The <see cref="ApplicationDbContext"/> to use for data retrieval.</param>
    /// <param name="dataProvider">The data provider to use for data retrieval.</param>
    public AccountRepository(ApplicationDbContext dbContext, GoCardlessDataProvider dataProvider)
    {
        _dbContext = dbContext;
        _dataProvider = dataProvider;
    }

    /// <summary>
    /// Gets the <see cref="BankAccount"/> matching the specified id.
    /// </summary>
    /// <param name="accountId">The id of the account to retrieve.</param>
    /// <returns>The <see cref="BankAccount"/> if one ist matched otherwise <see langword="null"/>.</returns>
    public async Task<BankAccount?> Get(string accountId)
    {
        var account = await _dataProvider.GetBankAccount(accountId);
        return account.Result;
    }

    /// <summary>
    /// Gets the <see cref="BankAccount"/>s matching the specified ids.
    /// </summary>
    /// <param name="accountIds">The ids of the accounts to retrieve.</param>
    /// <returns>A list of the matched <see cref="BankAccount"/>s.</returns>
    public async Task<IEnumerable<BankAccount>> Get(IEnumerable<string> accountIds)
    {
        var accounts = await _dataProvider.GetBankAccounts(accountIds);
        return accounts.Select(query => query.Result)!;
    }
}