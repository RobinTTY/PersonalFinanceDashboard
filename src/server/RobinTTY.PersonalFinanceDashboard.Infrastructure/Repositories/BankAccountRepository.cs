using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RobinTTY.PersonalFinanceDashboard.Core.Models;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Repositories;

/// <summary>
/// Manages <see cref="BankAccount"/> data retrieval.
/// </summary>
public class BankAccountRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<BankAccountRepository> _logger;

    /// <summary>
    /// Creates a new instance of <see cref="BankAccountRepository"/>.
    /// </summary>
    /// <param name="dbContext">The <see cref="ApplicationDbContext"/> to use for data retrieval.</param>
    /// <param name="logger">Logger used for monitoring purposes.</param>
    public BankAccountRepository(
        ApplicationDbContext dbContext,
        ILogger<BankAccountRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
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
        _dbContext.BankAccounts.OrderBy(account => account.Name).ThenBy(account => account.Id);

    /// <summary>
    /// Adds a new <see cref="BankAccount"/>.
    /// </summary>
    /// <param name="bankAccount">The <see cref="BankAccount"/>s to add.</param>
    public async Task<BankAccount> AddBankAccount(BankAccount bankAccount)
    {
        var entityEntry = _dbContext.BankAccounts.Add(bankAccount);
        await _dbContext.SaveChangesAsync();

        return entityEntry.Entity;
    }

    /// <summary>
    /// Updates the name and description of an existing <see cref="BankAccount"/>.
    /// </summary>
    /// <param name="bankAccountId">The id of the <see cref="BankAccount"/> to update.</param>
    /// <param name="name">The name of the account.</param>
    /// <param name="description">A description of this account.</param>
    /// <returns>The updated <see cref="BankAccount"/>, or <see langword="null"/> if it doesn't exist.</returns>
    public async Task<BankAccount?> UpdateBankAccount(Guid bankAccountId, string name, string? description)
    {
        var bankAccount = await _dbContext.BankAccounts.SingleOrDefaultAsync(account => account.Id == bankAccountId);
        if (bankAccount == null)
            return null;

        bankAccount.Name = name;
        bankAccount.Description = description;
        await _dbContext.SaveChangesAsync();

        return bankAccount;
    }

    /// <summary>
    /// Sets whether a <see cref="BankAccount"/> is included in analytics such as graphs and account views.
    /// </summary>
    /// <param name="bankAccountId">The id of the <see cref="BankAccount"/> to update.</param>
    /// <param name="includeInAnalytics">Whether the account should be included in analytics.</param>
    /// <returns>The updated <see cref="BankAccount"/> if one is matched, otherwise <see langword="null"/>.</returns>
    public async Task<BankAccount?> SetIncludeInAnalytics(Guid bankAccountId, bool includeInAnalytics)
    {
        var bankAccount = await _dbContext.BankAccounts.SingleOrDefaultAsync(account => account.Id == bankAccountId);
        if (bankAccount == null)
            return null;

        bankAccount.IncludeInAnalytics = includeInAnalytics;
        await _dbContext.SaveChangesAsync();

        return bankAccount;
    }

    /// <summary>
    /// Deletes an existing <see cref="BankAccount"/>.
    /// </summary>
    /// <param name="bankAccountId">The id of the <see cref="BankAccount"/> to delete.</param>
    /// <returns>Boolean value indicating whether the operation was successful.</returns>
    public async Task<bool> DeleteBankAccount(Guid bankAccountId)
    {
        var result = await _dbContext.BankAccounts.Where(t => t.Id == bankAccountId).ExecuteDeleteAsync();
        return Convert.ToBoolean(result);
    }
}
