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
            .Where(transaction => transaction.BankAccount != null && transaction.BankAccount.Id == accountId)
            .OrderByDescending(transaction => transaction.ValueDate);
    }

    /// <summary>
    /// Adds a new <see cref="Transaction"/> to the specified <see cref="BankAccount"/>.
    /// </summary>
    /// <param name="transaction">The <see cref="Transaction"/> to add.</param>
    /// <param name="bankAccountId">The id of the <see cref="BankAccount"/> the transaction belongs to.</param>
    /// <returns>The added <see cref="Transaction"/>, or <see langword="null"/> if the bank account doesn't exist.</returns>
    public async Task<Transaction?> AddTransaction(Transaction transaction, Guid bankAccountId)
    {
        var bankAccount = await _dbContext.BankAccounts.SingleOrDefaultAsync(account => account.Id == bankAccountId);
        if (bankAccount == null)
            return null;

        transaction.BankAccount = bankAccount;
        await _dbContext.Transactions.AddAsync(transaction);
        await _dbContext.SaveChangesAsync();

        return transaction;
    }

    /// <summary>
    /// Updates the user-entered properties of an existing <see cref="Transaction"/>.
    /// </summary>
    /// <param name="transactionId">The id of the <see cref="Transaction"/> to update.</param>
    /// <param name="category">The category this transaction belongs to.</param>
    /// <param name="notes">User created notes for this transaction.</param>
    /// <returns>The updated <see cref="Transaction"/>, or <see langword="null"/> if it doesn't exist.</returns>
    public async Task<Transaction?> UpdateTransaction(Guid transactionId, string category, string notes)
    {
        var transaction = await _dbContext.Transactions.SingleOrDefaultAsync(t => t.Id == transactionId);
        if (transaction == null)
            return null;

        transaction.Category = category;
        transaction.Notes = notes;
        await _dbContext.SaveChangesAsync();

        return transaction;
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