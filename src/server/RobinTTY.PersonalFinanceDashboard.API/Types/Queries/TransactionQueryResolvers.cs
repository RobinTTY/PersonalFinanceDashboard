using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate.Types;
using RobinTTY.PersonalFinanceDashboard.API.Repositories;
using RobinTTY.PersonalFinanceDashboard.Core.Models;

namespace RobinTTY.PersonalFinanceDashboard.Api.Types.Queries;

[QueryType]
public class TransactionQueryResolvers
{
    /// <summary>
    /// Creates a new instance of <see cref="TransactionQueryResolvers"/>.
    /// </summary>
    public TransactionQueryResolvers()
    {
    }
    
    /// <summary>
    /// Look up transactions of an account.
    /// </summary>
    /// <param name="accountId">The id of the account to retrieve.</param>
    [UsePaging]
    public async Task<IEnumerable<Transaction>> GetTransactions(string accountId, TransactionRepository repository, CancellationToken cancellationToken)
    {
        return await repository.GetAll();
    }
}