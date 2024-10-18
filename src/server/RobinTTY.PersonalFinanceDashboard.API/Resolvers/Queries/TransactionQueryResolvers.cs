using HotChocolate.Types;
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
    /// <param name="cancellationToken">Optional token to signal cancellation of the operation.</param>
    public async Task<IEnumerable<Transaction>> GetTransactions(TransactionRepository repository,
        CancellationToken cancellationToken)
    {
        return await repository.GetTransactions(cancellationToken);
    }

    /// <summary>
    /// Look up transactions of an account.
    /// </summary>
    /// <param name="repository">The injected repository to use for data retrieval.</param>
    /// <param name="accountId">The id of the account to retrieve.</param>
    /// <param name="cancellationToken">Optional token to signal cancellation of the operation.</param>
    [UsePaging]
    public async Task<IEnumerable<Transaction>> GetTransactionsByAccountId(TransactionRepository repository,
        string accountId, CancellationToken cancellationToken)
    {
        return await repository.GetTransactionsByAccountId(accountId);
    }
}
