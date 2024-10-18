using HotChocolate.Types;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Repositories;

namespace RobinTTY.PersonalFinanceDashboard.Api.Resolvers.Mutations;

/// <summary>
/// <see cref="BankAccount"/> related mutation resolvers.
/// </summary>
[MutationType]
public class BankAccountMutations
{
    /// <summary>
    /// Create a new banking account.
    /// </summary>
    /// <param name="repository">The injected repository to use for data retrieval.</param>
    /// <param name="bankAccount">The banking account to create.</param>
    public async Task<BankAccount> CreateBankAccount(BankAccountRepository repository, BankAccount bankAccount)
    {
        return await repository.AddBankAccount(bankAccount);
    }

    /// <summary>
    /// Update an existing banking account.
    /// </summary>
    /// <param name="repository">The injected repository to use for data retrieval.</param>
    /// <param name="bankAccount">The banking account to update.</param>
    /// <returns></returns>
    public async Task<BankAccount> UpdateBankAccount(BankAccountRepository repository, BankAccount bankAccount)
    {
        return await repository.UpdateBankAccount(bankAccount);
    }
    
    /// <summary>
    /// Delete an existing bank account.
    /// </summary>
    /// <param name="repository">The injected repository to use for data retrieval.</param>
    /// <param name="bankAccountId">The id of the bank account to delete.</param>
    public async Task<bool> DeleteBankAccount(BankAccountRepository repository,
        Guid bankAccountId)
    {
        return await repository.DeleteBankAccount(bankAccountId);
    }
}
