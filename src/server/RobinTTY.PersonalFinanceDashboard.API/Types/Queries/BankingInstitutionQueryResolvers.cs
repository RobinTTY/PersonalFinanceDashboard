using HotChocolate.Types;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Repositories;

namespace RobinTTY.PersonalFinanceDashboard.Api.Types.Queries;

/// <summary>
/// <see cref="BankingInstitution"/> related query resolvers.
/// </summary>
[QueryType]
public class BankingInstitutionQueryResolvers
{
    /// <summary>
    /// Look up banking institutions by their id.
    /// </summary>
    /// <param name="institutionId">The id of the banking institution to retrieve.</param>
    /// <param name="repository">The repository to use for data retrieval.</param>
    public async Task<BankingInstitution?> GetBankingInstitution(BankingInstitutionRepository repository, string institutionId)
    {
        return await repository.GetBankingInstitution(institutionId);
    }

    /// <summary>
    /// Look up banking institutions.
    /// </summary>
    /// <param name="countryCode">Optional filter by country the institution operates in.</param>
    /// <param name="repository">The repository to use for data retrieval.</param>
    [UsePaging(MaxPageSize = 3000)]
    public async Task<IEnumerable<BankingInstitution>> GetBankingInstitutions(BankingInstitutionRepository repository,
        string? countryCode = null)
    {
        return await repository.GetBankingInstitutions(countryCode);
    }
}
