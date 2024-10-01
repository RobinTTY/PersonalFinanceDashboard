using Microsoft.EntityFrameworkCore;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Entities;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Mappers;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Repositories;

/// <summary>
/// Manages <see cref="Transaction"/> data retrieval.
/// </summary>
public class TransactionRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly TransactionMapper _transactionMapper;

    /// <summary>
    /// Creates a new instance of <see cref="TransactionRepository"/>.
    /// </summary>
    /// <param name="dbContext">The <see cref="ApplicationDbContext"/> to use for data retrieval.</param>
    /// <param name="transactionMapper">The mapper used to map ef entities to the domain model.</param>
    public TransactionRepository(ApplicationDbContext dbContext, TransactionMapper transactionMapper)
    {
        _dbContext = dbContext;
        _transactionMapper = transactionMapper;
    }

    /// <summary>
    /// Gets all <see cref="Transaction"/>s.
    /// </summary>
    /// <returns>A list of all <see cref="Transaction"/>s.</returns>
    public async Task<IEnumerable<Transaction>> GetTransactions(CancellationToken cancellationToken)
    {
        // TODO: How should navigation properties be included programatically?
        var transactionEntities = await _dbContext.Transactions
            .Include(t => t.Tags)
            .ToListAsync(cancellationToken);
        var transactionModels = transactionEntities
            .Select(t => _transactionMapper.EntityToModel(t))
            .ToList();

        return transactionModels;
    }

    /// <summary>
    /// Gets all transactions associated with an account.
    /// </summary>
    /// <param name="accountId">The account id the transactions are associated with.</param>
    /// <returns>A list of matched <see cref="Transaction"/>s.</returns>
    public async Task<IEnumerable<Transaction>> GetTransactionsByAccountId(string accountId)
    {
        var transactionEntities = await _dbContext.Transactions
            .Where(transaction => transaction.AccountId == accountId)
            .ToListAsync();
        var transactionModels = transactionEntities
            .Select(t => _transactionMapper.EntityToModel(t))
            .ToList();

        return transactionModels;
    }

    /// <summary>
    /// Adds a new <see cref="Transaction"/>.
    /// </summary>
    /// <param name="transaction">The <see cref="Transaction"/> to add.</param>
    /// <returns>The <see cref="Transaction"/> to add.</returns>
    // TODO: This should use a TransactionRequest class not Transaction itself
    public async Task<Transaction> AddTransaction(Transaction transaction)
    {
        var transactionEntity = _transactionMapper.ModelToEntity(transaction);
        await _dbContext.Transactions.AddAsync(transactionEntity);
        await _dbContext.SaveChangesAsync();

        transaction.Id = transactionEntity.Id;
        return transaction;
    }

    /// <summary>
    /// Updates an existing <see cref="Transaction"/>.
    /// </summary>
    /// <param name="transactionDto">The <see cref="Transaction"/> to update.</param>
    /// <returns>The updated <see cref="Transaction"/>.</returns>
    /// TODO: return Transaction not TransactionEntity
    public async Task<TransactionEntity> UpdateTransaction(TransactionEntity transactionDto)
    {
        var updateEntry = _dbContext.Transactions.Update(transactionDto);
        await _dbContext.SaveChangesAsync();
        return updateEntry.Entity;
    }

    /// <summary>
    /// Deletes an existing <see cref="Transaction"/>.
    /// </summary>
    /// <param name="transactionId">The id of the <see cref="Transaction"/> to delete.</param>
    /// <returns>Boolean value indicating whether the operation was successful.</returns>
    public async Task<bool> DeleteTransaction(string transactionId)
    {
        var result = await _dbContext.Transactions.Where(t => t.Id == transactionId).ExecuteDeleteAsync();
        return Convert.ToBoolean(result);
    }
}
