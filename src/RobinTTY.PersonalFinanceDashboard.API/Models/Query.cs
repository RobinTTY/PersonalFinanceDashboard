using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.Database.Mock;

namespace RobinTTY.PersonalFinanceDashboard.API.Models;

/// <summary>
/// GraphQL root type for query operations.
/// </summary>
public class Query
{
    public IEnumerable<Account> GetAccounts() => MockDataAccessService.GetAccounts(100);

    public IEnumerable<Transaction> GetTransactions() => MockDataAccessService.GetTransactions(100);
}
