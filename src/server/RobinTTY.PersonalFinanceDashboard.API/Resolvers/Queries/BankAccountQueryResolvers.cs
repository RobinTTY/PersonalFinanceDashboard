using HotChocolate.Data;
using HotChocolate.Types;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Repositories;

namespace RobinTTY.PersonalFinanceDashboard.Api.Resolvers.Queries;

/// <summary>
/// <see cref="BankAccount"/> related query resolvers.
/// </summary>
[QueryType]
public sealed class BankAccountQueryResolvers
{
    /// <summary>
    /// Look up an account by its id.
    /// </summary>
    /// <param name="repository">The repository to use for data retrieval.</param>
    /// <param name="accountId">The id of the account to lookup.</param>
    [UseSingleOrDefault]
    [UseProjection]
    public async Task<IQueryable<BankAccount?>> GetBankAccount(BankAccountRepository repository, Guid accountId)
    {
        return await repository.GetBankAccount(accountId);
    }

    /// <summary>
    /// Look up accounts.
    /// </summary>
    /// <param name="repository">The injected repository to use for data retrieval.</param>
    [UsePaging]
    [UseProjection]
    public async Task<IQueryable<BankAccount>> GetBankAccounts(BankAccountRepository repository)
    {
        return await repository.GetBankAccounts();
    }
}