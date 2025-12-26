using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Services.DataSynchronization.Interfaces;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Repositories;

/// <summary>
/// Manages <see cref="Transaction"/> data retrieval.
/// </summary>
public class TransactionRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ITransactionSyncHandler _transactionSyncHandler;
    private readonly ILogger<TransactionRepository> _logger;

    /// <summary>
    /// Creates a new instance of <see cref="TransactionRepository"/>.
    /// </summary>
    /// <param name="dbContext">The <see cref="ApplicationDbContext"/> to use for data retrieval.</param>
    /// <param name="transactionSyncHandler">Handles the synchronization of third party data.</param>
    /// <param name="logger">Logger used for monitoring purposes.</param>
    public TransactionRepository(ApplicationDbContext dbContext, ITransactionSyncHandler transactionSyncHandler,
        ILogger<TransactionRepository> logger)
    {
        _dbContext = dbContext;
        _transactionSyncHandler = transactionSyncHandler;
        _logger = logger;
    }

    /// <summary>
    /// Gets all <see cref="Transaction"/>s.
    /// </summary>
    /// <returns>A list of all <see cref="Transaction"/>s.</returns>
    public async Task<IQueryable<Transaction>> GetTransactions(CancellationToken cancellationToken)
    {
        await _transactionSyncHandler.SynchronizeData();
        return _dbContext.Transactions;
    }

    /// <summary>
    /// Gets all transactions associated with an account.
    /// </summary>
    /// <param name="accountId">The account id the transactions are associated with.</param>
    /// <returns>A list of matched <see cref="Transaction"/>s.</returns>
    public async Task<IQueryable<Transaction>> GetTransactionsByAccountId(Guid accountId)
    {
        await _transactionSyncHandler.SynchronizeData(accountId);
        
        return _dbContext.Transactions
            .Where(transaction => transaction.AccountId == accountId);
    }

    /// <summary>
    /// Adds a new <see cref="Transaction"/>.
    /// </summary>
    /// <param name="transaction">The <see cref="Transaction"/> to add.</param>
    /// <returns>The <see cref="Transaction"/> to add.</returns>
    // TODO: This should use a TransactionRequest class not Transaction itself
    public async Task<Transaction> AddTransaction(Transaction transaction)
    {
        await _dbContext.Transactions.AddAsync(transaction);
        await _dbContext.SaveChangesAsync();

        return transaction;
    }

    /// <summary>
    /// Updates an existing <see cref="Transaction"/>.
    /// </summary>
    /// <param name="transaction">The <see cref="Transaction"/> to update.</param>
    /// <returns>The updated <see cref="Transaction"/>.</returns>
    public async Task<Transaction> UpdateTransaction(Transaction transaction)
    {
        var updateEntry = _dbContext.Transactions.Update(transaction);
        await _dbContext.SaveChangesAsync();

        return updateEntry.Entity;
    }

    /// <summary>
    /// Deletes an existing <see cref="Transaction"/>.
    /// </summary>
    /// <param name="transactionId">The id of the <see cref="Transaction"/> to delete.</param>
    /// <returns>Boolean value indicating whether the operation was successful.</returns>
    public async Task<bool> DeleteTransaction(Guid transactionId)
    {
        var result = await _dbContext.Transactions.Where(t => t.Id == transactionId).ExecuteDeleteAsync();
        return Convert.ToBoolean(result);
    }
}