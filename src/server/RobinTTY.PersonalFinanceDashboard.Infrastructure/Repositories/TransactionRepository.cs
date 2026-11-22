using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RobinTTY.PersonalFinanceDashboard.Core.Models;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Repositories;

/// <summary>
/// Manages <see cref="Transaction"/> data retrieval.
/// </summary>
public class TransactionRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<TransactionRepository> _logger;

    /// <summary>
    /// Creates a new instance of <see cref="TransactionRepository"/>.
    /// </summary>
    /// <param name="dbContext">The <see cref="ApplicationDbContext"/> to use for data retrieval.</param>
    /// <param name="logger">Logger used for monitoring purposes.</param>
    public TransactionRepository(ApplicationDbContext dbContext, ILogger<TransactionRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// Gets all <see cref="Transaction"/>s, the newest first.
    /// </summary>
    /// <returns>A list of all <see cref="Transaction"/>s.</returns>
    public IQueryable<Transaction> GetTransactions() =>
        _dbContext.Transactions
            .OrderByDescending(transaction => transaction.ValueDate)
            // Same-day transactions need a fixed order to be paged
            .ThenBy(transaction => transaction.Id);

    /// <summary>
    /// Gets all transactions associated with an account, the newest first.
    /// </summary>
    /// <param name="accountId">The account id the transactions are associated with.</param>
    /// <returns>A list of matched <see cref="Transaction"/>s.</returns>
    public IQueryable<Transaction> GetTransactionsByAccountId(Guid accountId) =>
        GetTransactions()
            .Where(transaction => transaction.BankAccount != null && transaction.BankAccount.Id == accountId);

    /// <summary>
    /// Adds a new <see cref="Transaction"/> to the specified <see cref="BankAccount"/>.
    /// </summary>
    /// <param name="transaction">The <see cref="Transaction"/> to add.</param>
    /// <param name="bankAccountId">The id of the <see cref="BankAccount"/> the transaction belongs to.</param>
    /// <param name="cancellationToken">A token to signal cancellation.</param>
    /// <returns>The added <see cref="Transaction"/>, or <see langword="null"/> if the bank account doesn't exist.</returns>
    public async Task<Transaction?> AddTransaction(Transaction transaction, Guid bankAccountId,
        CancellationToken cancellationToken = default)
    {
        var bankAccount = await _dbContext.BankAccounts.SingleOrDefaultAsync(account => account.Id == bankAccountId,
            cancellationToken);
        if (bankAccount == null)
            return null;

        transaction.BankAccount = bankAccount;
        await _dbContext.Transactions.AddAsync(transaction, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return transaction;
    }

    /// <summary>
    /// Updates the user-entered properties of an existing <see cref="Transaction"/>.
    /// </summary>
    /// <param name="transactionId">The id of the <see cref="Transaction"/> to update.</param>
    /// <param name="category">The category this transaction belongs to, blank to remove it.</param>
    /// <param name="notes">User created notes for this transaction, blank to remove them.</param>
    /// <param name="cancellationToken">A token to signal cancellation.</param>
    /// <returns>The updated <see cref="Transaction"/>, or <see langword="null"/> if it doesn't exist.</returns>
    public async Task<Transaction?> UpdateTransaction(Guid transactionId, string? category, string? notes,
        CancellationToken cancellationToken = default)
    {
        var transaction =
            await _dbContext.Transactions.SingleOrDefaultAsync(t => t.Id == transactionId, cancellationToken);
        if (transaction == null)
            return null;

        transaction.Category = string.IsNullOrWhiteSpace(category) ? null : category;
        transaction.Notes = string.IsNullOrWhiteSpace(notes) ? null : notes;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return transaction;
    }

    /// <summary>
    /// Deletes an existing <see cref="Transaction"/>.
    /// </summary>
    /// <param name="transactionId">The id of the <see cref="Transaction"/> to delete.</param>
    /// <param name="cancellationToken">A token to signal cancellation.</param>
    /// <returns>Boolean value indicating whether the operation was successful.</returns>
    public async Task<bool> DeleteTransaction(Guid transactionId, CancellationToken cancellationToken = default)
    {
        var result = await _dbContext.Transactions.Where(t => t.Id == transactionId)
            .ExecuteDeleteAsync(cancellationToken);
        return Convert.ToBoolean(result);
    }
}
