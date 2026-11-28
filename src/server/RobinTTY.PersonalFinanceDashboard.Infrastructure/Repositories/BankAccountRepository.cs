using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RobinTTY.PersonalFinanceDashboard.Core.Extensions;
using RobinTTY.PersonalFinanceDashboard.Core.Models;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Repositories;

/// <summary>
/// Manages <see cref="BankAccount"/> data retrieval.
/// </summary>
public class BankAccountRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<BankAccountRepository> _logger;
    private readonly TimeProvider _timeProvider;

    /// <summary>
    /// Creates a new instance of <see cref="BankAccountRepository"/>.
    /// </summary>
    /// <param name="dbContext">The <see cref="ApplicationDbContext"/> to use for data retrieval.</param>
    /// <param name="logger">Logger used for monitoring purposes.</param>
    /// <param name="timeProvider">Provides the current time.</param>
    public BankAccountRepository(
        ApplicationDbContext dbContext,
        ILogger<BankAccountRepository> logger,
        TimeProvider timeProvider)
    {
        _dbContext = dbContext;
        _logger = logger;
        _timeProvider = timeProvider;
    }

    /// <summary>
    /// Gets the <see cref="BankAccount"/> matching the specified id.
    /// </summary>
    /// <param name="accountId">The id of the <see cref="BankAccount"/> to retrieve.</param>
    /// <returns>The <see cref="BankAccount"/> if one ist matched otherwise <see langword="null"/>.</returns>
    public IQueryable<BankAccount?> GetBankAccount(Guid accountId) =>
        _dbContext.BankAccounts.Where(account => account.Id == accountId);

    /// <summary>
    /// Gets a list of <see cref="BankAccount"/>s.
    /// </summary>
    /// <returns>A list of <see cref="BankAccount"/>s.</returns>
    public IQueryable<BankAccount> GetBankAccounts() =>
        _dbContext.BankAccounts
            .OrderBy(account => account.Name ?? account.ProviderName)
            .ThenBy(account => account.Id);

    /// <summary>
    /// Adds a new <see cref="BankAccount"/>.
    /// </summary>
    /// <param name="bankAccount">The <see cref="BankAccount"/>s to add.</param>
    /// <param name="cancellationToken">A token to signal cancellation.</param>
    public async Task<BankAccount> AddBankAccount(BankAccount bankAccount,
        CancellationToken cancellationToken = default)
    {
        var entityEntry = _dbContext.BankAccounts.Add(bankAccount);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return entityEntry.Entity;
    }

    /// <summary>
    /// Updates the name and description of an existing <see cref="BankAccount"/>.
    /// </summary>
    /// <param name="bankAccountId">The id of the <see cref="BankAccount"/> to update.</param>
    /// <param name="name">The name of the account. If empty, the provider's name is used again.</param>
    /// <param name="description">A description of this account. If empty, the provider's description is used again.</param>
    /// <param name="cancellationToken">A token to signal cancellation.</param>
    /// <returns>The updated <see cref="BankAccount"/>, or <see langword="null"/> if it doesn't exist.</returns>
    public async Task<BankAccount?> UpdateBankAccount(Guid bankAccountId, string name, string? description,
        CancellationToken cancellationToken = default)
    {
        var bankAccount = await _dbContext.BankAccounts.SingleOrDefaultAsync(account => account.Id == bankAccountId,
            cancellationToken);
        if (bankAccount == null)
            return null;

        bankAccount.Name = string.IsNullOrWhiteSpace(name) ? null : name;
        bankAccount.Description = string.IsNullOrWhiteSpace(description) ? null : description;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return bankAccount;
    }

    /// <summary>
    /// Sets whether a <see cref="BankAccount"/> is included in analytics such as graphs and account views.
    /// </summary>
    /// <param name="bankAccountId">The id of the <see cref="BankAccount"/> to update.</param>
    /// <param name="includeInAnalytics">Whether the account should be included in analytics.</param>
    /// <param name="cancellationToken">A token to signal cancellation.</param>
    /// <returns>The updated <see cref="BankAccount"/> if one is matched, otherwise <see langword="null"/>.</returns>
    public async Task<BankAccount?> SetIncludeInAnalytics(Guid bankAccountId, bool includeInAnalytics,
        CancellationToken cancellationToken = default)
    {
        var bankAccount = await _dbContext.BankAccounts.SingleOrDefaultAsync(account => account.Id == bankAccountId,
            cancellationToken);
        if (bankAccount == null)
            return null;

        bankAccount.IncludeInAnalytics = includeInAnalytics;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return bankAccount;
    }

    /// <summary>
    /// Deletes an existing <see cref="BankAccount"/> and its transactions. Accounts that are still synchronized
    /// aren't deleted, since the synchronization would recreate them.
    /// </summary>
    /// <param name="bankAccountId">The id of the <see cref="BankAccount"/> to delete.</param>
    /// <param name="cancellationToken">A token to signal cancellation.</param>
    /// <returns>Whether the account was deleted.</returns>
    public async Task<DeleteResult> DeleteBankAccount(Guid bankAccountId,
        CancellationToken cancellationToken = default)
    {
        var bankAccount = await _dbContext.BankAccounts
            .Include(account => account.AssociatedAuthenticationRequests)
            .SingleOrDefaultAsync(account => account.Id == bankAccountId, cancellationToken);
        if (bankAccount == null)
            return DeleteResult.NotFound;
        if (bankAccount.IsSynchronized(_timeProvider.GetUtcNow().UtcDateTime))
            return DeleteResult.Synchronized;

        await _dbContext.BankAccounts.Where(account => account.Id == bankAccountId)
            .ExecuteDeleteAsync(cancellationToken);
        return DeleteResult.Deleted;
    }
}
