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

    /// <summary>
    /// Gets all <see cref="Transaction"/>s.
    /// </summary>
    /// <returns>A list of all <see cref="Transaction"/>s.</returns>
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

    /// <summary>
    /// Adds a new <see cref="Transaction"/>.
    /// </summary>
    /// <param name="transaction">The <see cref="Transaction"/> to add.</param>
    /// <returns>The added <see cref="Transaction"/>.</returns>
    public async Task<Transaction> Add(Transaction transaction)
    {
        // TODO: Automate mapping
        var efTransaction = new EfTransaction(transaction.Id, transaction.ValueDate, transaction.Payer!, transaction.Payee!, transaction.Amount, transaction.Currency, transaction.Category, transaction.Notes);
        _dbContext.Transactions.Add(efTransaction);
        await _dbContext.SaveChangesAsync();

        transaction.Id = efTransaction.Id;
        return transaction;
    }

    /// <summary>
    /// Updates an existing <see cref="Transaction"/>.
    /// </summary>
    /// <param name="transaction">The <see cref="Transaction"/> to update.</param>
    /// <returns>The updated <see cref="Transaction"/>.</returns>
    /// TODO: return Transaction not EfTransaction
    public async Task<EfTransaction> Update(EfTransaction transaction)
    {
        _dbContext.Transactions.Update(transaction);
        await _dbContext.SaveChangesAsync();
        return transaction;
    }

    /// <summary>
    /// Deletes an existing <see cref="Transaction"/>.
    /// </summary>
    /// <param name="transactionId">The id of the transaction to delete.</param>
    /// <returns>TODO</returns>
    public async Task<bool> Delete(string transactionId)
    {
        var result = await _dbContext.Transactions.Where(t => t.Id == transactionId).ExecuteDeleteAsync();
        return Convert.ToBoolean(result);
    }
}
