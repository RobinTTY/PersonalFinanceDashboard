using HotChocolate.Types;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Repositories;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Services;

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
    /// <param name="repository">The injected repository to use for data retrieval.</param>
    /// <param name="institutionId">The id of the banking institution to retrieve.</param>
    public async Task<BankingInstitution?> GetBankingInstitution(BankingInstitutionRepository repository,
        string institutionId)
    {
        return await repository.GetBankingInstitution(institutionId);
    }

    /// <summary>
    /// Look up banking institutions.
    /// </summary>
    /// <param name="retrievalService">The injected repository to use for data retrieval.</param>
    /// <param name="countryCode">Optional filter by country the institution operates in.</param>
    [UsePaging(MaxPageSize = 3000)]
    public async Task<IEnumerable<BankingInstitution>> GetBankingInstitutions(ThirdPartyDataRetrievalService retrievalService,
        string? countryCode = null)
    {
        // TODO: It isn't great that we need to inject different services here.
        // The wrong service could be injected. Maybe there is a better solution.
        return await retrievalService.GetBankingInstitutions(countryCode);
    }
}
