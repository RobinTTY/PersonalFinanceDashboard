using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate.Types;
using RobinTTY.PersonalFinanceDashboard.API.Repositories;
using RobinTTY.PersonalFinanceDashboard.Core.Models;

namespace RobinTTY.PersonalFinanceDashboard.API.Models.Queries;

[ExtendObjectType(OperationTypeNames.Query)]
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
    public async Task<IReadOnlyList<Transaction>> GetTransactions(string accountId, TransactionRepository repository, CancellationToken cancellationToken)
    {
        return await repository.GetTransactions();
    }
}