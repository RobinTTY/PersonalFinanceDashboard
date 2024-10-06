﻿using Microsoft.EntityFrameworkCore;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Mappers;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Repositories;

/// <summary>
/// Manages <see cref="BankAccount"/> data retrieval.
/// </summary>
public class BankAccountRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly BankAccountMapper _bankAccountMapper;

    /// <summary>
    /// Creates a new instance of <see cref="BankAccountRepository"/>.
    /// </summary>
    /// <param name="dbContext">The <see cref="ApplicationDbContext"/> to use for data retrieval.</param>
    /// <param name="bankAccountMapper">The mapper used to map ef entities to the domain model.</param>
    public BankAccountRepository(ApplicationDbContext dbContext, BankAccountMapper bankAccountMapper)
    {
        _dbContext = dbContext;
        _bankAccountMapper = bankAccountMapper;
    }

    /// <summary>
    /// Gets the <see cref="BankAccount"/> matching the specified id.
    /// </summary>
    /// <param name="accountId">The id of the <see cref="BankAccount"/> to retrieve.</param>
    /// <returns>The <see cref="BankAccount"/> if one ist matched otherwise <see langword="null"/>.</returns>
    public async Task<BankAccount?> GetBankAccount(Guid accountId)
    {
        var accountEntity =
            await _dbContext.BankAccounts.SingleOrDefaultAsync(account => account.Id == accountId);

        return accountEntity is not null ? _bankAccountMapper.EntityToModel(accountEntity) : null;
    }

    /// <summary>
    /// Gets a list of <see cref="BankAccount"/>s.
    /// </summary>
    /// <returns>A list of <see cref="BankAccount"/>s.</returns>
    public async Task<List<BankAccount>> GetBankAccounts()
    {
        var accountEntities = await _dbContext.BankAccounts.ToListAsync();
        return accountEntities.Select(_bankAccountMapper.EntityToModel).ToList();
    }
}