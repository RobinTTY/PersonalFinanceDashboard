﻿using HotChocolate.Types;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Repositories;

namespace RobinTTY.PersonalFinanceDashboard.Api.Types.Queries;

/// <summary>
/// <see cref="BankAccount"/> related query resolvers.
/// </summary>
[QueryType]
public sealed class AccountQueryResolvers
{
    // TODO: probably should be GetBankAccount and then separate route for investment accounts
    /// <summary>
    /// Look up an account by its id.
    /// </summary>
    /// <param name="accountId">The id of the account to lookup.</param>
    /// <param name="repository">The repository to use for data retrieval.</param>
    public async Task<BankAccount?> GetAccount(string accountId, AccountRepository repository)
    {
        return await repository.GetAccount(accountId);
    }

    /// <summary>
    /// Look up accounts by a list of ids.
    /// </summary>
    /// <param name="accountIds">The ids of the accounts to retrieve.</param>
    /// <param name="repository">The repository to use for data retrieval.</param>
    [UsePaging]
    public async Task<IEnumerable<BankAccount>> GetAccounts(IEnumerable<string> accountIds, AccountRepository repository)
    {
        return await repository.GetAccounts(accountIds);
    }
}