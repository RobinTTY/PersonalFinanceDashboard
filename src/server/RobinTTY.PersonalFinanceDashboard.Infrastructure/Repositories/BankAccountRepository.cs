using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Extensions;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Services;
using RobinTTY.PersonalFinanceDashboard.ThirdPartyDataProviders;

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
    public async Task<IQueryable<BankAccount?>> GetBankAccount(Guid accountId)
    {
        return _dbContext.BankAccounts.Where(account => account.Id == accountId);
    }

    /// <summary>
    /// Gets a list of <see cref="BankAccount"/>s.
    /// </summary>
    /// <returns>A list of <see cref="BankAccount"/>s.</returns>
    public async Task<IQueryable<BankAccount>> GetBankAccounts()
    {
        return _dbContext.BankAccounts;
    }

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
    /// Updates an existing <see cref="BankAccount"/>.
    /// </summary>
    /// <param name="bankAccount">The bank account to update.</param>
    /// <returns>The updated <see cref="BankAccount"/>.</returns>
    public async Task<BankAccount> UpdateBankAccount(BankAccount bankAccount)
    {
        var entityEntry = _dbContext.BankAccounts.Update(bankAccount);
        await _dbContext.SaveChangesAsync();

        return entityEntry.Entity;
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