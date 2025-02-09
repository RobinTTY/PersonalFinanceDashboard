using HotChocolate.Data;
using HotChocolate.Types;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Repositories;

namespace RobinTTY.PersonalFinanceDashboard.Api.Resolvers.Queries;

/// <summary>
/// <see cref="BankingInstitution"/> related query resolvers.
/// </summary>
[QueryType]
public class BankingInstitutionQueryResolvers
{
    /// <summary>
    /// Look up banking institutions by their id.
    /// </summary>
    /// <param name="repository">The injected repository to use for data retrieval.</param>
    /// <param name="institutionId">The id of the banking institution to retrieve.</param>
    [UseSingleOrDefault]
    [UseProjection]
    public async Task<IQueryable<BankingInstitution?>> GetBankingInstitution(BankingInstitutionRepository repository,
        Guid institutionId)
    {
        return await repository.GetBankingInstitution(institutionId);
    }

    /// <summary>
    /// Look up banking institutions.
    /// </summary>
    /// <param name="repository">The injected repository to use for data retrieval.</param>
    /// <param name="countryCode">Optional filter by country the institution operates in.</param>
    [UsePaging(MaxPageSize = 3000)]
    [UseProjection]
    public async Task<IQueryable<BankingInstitution>> GetBankingInstitutions(BankingInstitutionRepository repository,
        string? countryCode = null)
    {
        return await repository.GetBankingInstitutions(countryCode);
    }
}
