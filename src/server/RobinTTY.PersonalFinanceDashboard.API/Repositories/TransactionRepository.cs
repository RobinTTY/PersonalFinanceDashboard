using System.Linq;
using Microsoft.EntityFrameworkCore;
using RobinTTY.PersonalFinanceDashboard.API.EfModels;
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

    public async Task<IEnumerable<Transaction>> GetAll()
    {
        // TODO
        //var transactions = await _dataProvider.GetTransactions(accountId);
        //return transactions.Result!.AsQueryable();

        // var mockedTransactions = MockDataAccessService.GetTransactions(100);
        // return await Task.FromResult(mockedTransactions);
        var transactions = await _dbContext.Transactions.ToListAsync();
        // TODO: tag mapping
        var transformed = transactions.Select(t => new Transaction(t.Id, t.ValueDate, t.Payer!, t.Payee!, t.Amount, t.Currency, t.Category, new List<Tag>(), t.Notes)).ToList();
        return transformed;
    }

    public async Task<Transaction> Add(Transaction transaction)
    {
        // TODO: Automate mapping
        var efTransaction = new EfTransaction(transaction.Id, transaction.ValueDate, transaction.Payer!, transaction.Payee!, transaction.Amount, transaction.Currency, transaction.Category, transaction.Notes);
        _dbContext.Transactions.Add(efTransaction);
        await _dbContext.SaveChangesAsync();

        transaction.Id = efTransaction.Id;
        return transaction;
    }

    public async Task<EfTransaction> Update(EfTransaction transaction)
    {
        _dbContext.Transactions.Update(transaction);
        await _dbContext.SaveChangesAsync();
        return transaction;
    }

    public async Task<bool> Delete(string transactionId)
    {
        var result = await _dbContext.Transactions.Where(t => t.Id == transactionId).ExecuteDeleteAsync();
        return Convert.ToBoolean(result);
    }
}
