using HotChocolate.Types;
using RobinTTY.PersonalFinanceDashboard.API.Repositories;
using RobinTTY.PersonalFinanceDashboard.Core.Models;

namespace RobinTTY.PersonalFinanceDashboard.Api.Types.Queries;

/// <summary>
/// <see cref="Transaction"/> related query resolvers.
/// </summary>
[QueryType]
public class TransactionQueryResolvers
{
    /// <summary>
    /// Look up transactions of an account.
    /// </summary>
    /// <param name="accountId">The id of the account to retrieve.</param>
    /// <param name="repository">The repository to use for data retrieval.</param>
    [UsePaging]
    public async Task<IEnumerable<Transaction>> GetTransactions(string accountId, TransactionRepository repository, CancellationToken cancellationToken)
    {
        return await repository.GetAll();
    }
}