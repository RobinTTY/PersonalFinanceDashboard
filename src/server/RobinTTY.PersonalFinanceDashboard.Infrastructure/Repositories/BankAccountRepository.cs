using Microsoft.EntityFrameworkCore;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Mappers;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Repositories;

public class BankAccountRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly BankAccountMapper _bankAccountMapper;

    /// <summary>
    /// TODO
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="bankAccountMapper"></param>
    public BankAccountRepository(ApplicationDbContext dbContext, BankAccountMapper bankAccountMapper)
    {
        _dbContext = dbContext;
        _bankAccountMapper = bankAccountMapper;
    }

    /// <summary>
    /// TODO
    /// </summary>
    /// <param name="accountId"></param>
    /// <returns></returns>
    public async Task<BankAccount?> GetBankAccount(Guid accountId)
    {
        var accountEntity =
            await _dbContext.BankAccounts.SingleOrDefaultAsync(account => account.Id == accountId);

        return accountEntity is not null ? _bankAccountMapper.EntityToModel(accountEntity) : null;
    }

    public async Task<List<BankAccount>> GetBankAccounts()
    {
        var accountEntities = await _dbContext.BankAccounts.ToListAsync();
        return accountEntities.Select(_bankAccountMapper.EntityToModel).ToList();
    }
}
