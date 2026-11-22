using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Repositories;

namespace RobinTTY.PersonalFinanceDashboard.Api.Resolvers.Queries;

/// <summary>
/// <see cref="Transaction"/> related query resolvers.
/// </summary>
[QueryType]
public class TransactionQueryResolvers
{
    /// <summary>
    /// Look up transactions.
    /// </summary>
    /// <param name="repository">The injected repository to use for data retrieval.</param>
    [UsePaging]
    [UseProjection]
    public IQueryable<Transaction> GetTransactions(TransactionRepository repository) =>
        repository.GetTransactions();

    /// <summary>
    /// Look up transactions of an account.
    /// </summary>
    /// <param name="repository">The injected repository to use for data retrieval.</param>
    /// <param name="accountId">The id of the account to retrieve.</param>
    [UsePaging]
    [UseProjection]
    public IQueryable<Transaction> GetTransactionsByAccountId(TransactionRepository repository, Guid accountId) =>
        repository.GetTransactionsByAccountId(accountId);
}
