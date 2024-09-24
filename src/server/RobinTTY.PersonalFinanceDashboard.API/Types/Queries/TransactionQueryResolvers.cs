using HotChocolate.Types;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Repositories;

namespace RobinTTY.PersonalFinanceDashboard.Api.Types.Queries;

/// <summary>
/// <see cref="Transaction"/> related query resolvers.
/// </summary>
[QueryType]
public class TransactionQueryResolvers
{
    /// <summary>
    /// Look up transactions.
    /// </summary>
    /// <param name="repository">The repository to use for data retrieval.</param>
    /// <param name="cancellationToken">Optional token to signal cancellation of the operation.</param>
    public async Task<IEnumerable<Transaction>> GetTransactions(TransactionRepository repository,
        CancellationToken cancellationToken)
    {
        return await repository.GetTransactions(cancellationToken);
    }

    /// <summary>
    /// Look up transactions of an account.
    /// </summary>
    /// <param name="accountId">The id of the account to retrieve.</param>
    /// <param name="repository">The repository to use for data retrieval.</param>
    /// <param name="cancellationToken">Optional token to signal cancellation of the operation.</param>
    [UsePaging]
    public async Task<IEnumerable<Transaction>> GetTransactionsByAccountId(string accountId, TransactionRepository repository, CancellationToken cancellationToken)
    {
        return await repository.GetTransactionsByAccountId(accountId);
    }
}