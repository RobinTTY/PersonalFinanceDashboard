using System.Collections.Generic;
using System.Threading.Tasks;
using RobinTTY.PersonalFinanceDashboard.API.Utility;
using RobinTTY.PersonalFinanceDashboard.Core.Models;

namespace RobinTTY.PersonalFinanceDashboard.API.Repositories;

/// <summary>
/// Manages <see cref="Transaction"/> data retrieval.
/// </summary>
public class TransactionRepository
{
    private readonly ApplicationDbContext _dbContext;

    /// <summary>
    /// Creates a new instance of <see cref="TransactionRepository"/>.
    /// </summary>
    /// <param name="dbContext">The <see cref="ApplicationDbContext"/> to use for data retrieval.</param>
    public TransactionRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Transaction>> GetTransactions()
    {
        // TODO
        //var transactions = await _dataProvider.GetTransactions(accountId);
        //return transactions.Result!.AsQueryable();

        var mockedTransactions = MockDataAccessService.GetTransactions(100);
        return await Task.FromResult(mockedTransactions);
        //_dbContext.Transactions.
    }
}
