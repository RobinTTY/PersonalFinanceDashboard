using System.Linq;
using HotChocolate.Types;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.Database.Mock;

namespace RobinTTY.PersonalFinanceDashboard.API.Models;

/// <summary>
/// GraphQL root type for query operations.
/// </summary>
public class Query
{
    [UsePaging]
    public IQueryable<Account> GetAccounts() => MockDataAccessService.GetAccounts(100);

    [UsePaging]
    public IQueryable<Transaction> GetTransactions() => MockDataAccessService.GetTransactions(100);
}
