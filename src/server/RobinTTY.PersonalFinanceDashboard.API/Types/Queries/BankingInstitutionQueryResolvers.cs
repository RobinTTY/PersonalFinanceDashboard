using HotChocolate.Types;
using RobinTTY.PersonalFinanceDashboard.API.Repositories;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.ThirdPartyDataProviders;

namespace RobinTTY.PersonalFinanceDashboard.Api.Types.Queries;

/// <summary>
/// <see cref="BankingInstitution"/> related query resolvers.
/// </summary>
[QueryType]
public class BankingInstitutionQueryResolvers
{
    private readonly GoCardlessDataProvider _dataProvider;

    /// <summary>
    /// TODO: Create repository
    /// </summary>
    /// <param name="dataProvider"></param>
    public BankingInstitutionQueryResolvers(GoCardlessDataProvider dataProvider)
    {
        _dataProvider = dataProvider;
    }

    /// <summary>
    /// Look up banking institutions by their id.
    /// </summary>
    /// <param name="institutionId">The id of the banking institution to retrieve.</param>
    public async Task<BankingInstitution?> GetBankingInstitution(string institutionId,
        BankingInstitutionRepository repository)
    {
        return await repository.Get(institutionId);
    }

    /// <summary>
    /// Look up banking institutions.
    /// </summary>
    /// <param name="countryCode">Optional filter by country the institution operates in.</param>
    [UsePaging(MaxPageSize = 3000)]
    public async Task<IEnumerable<BankingInstitution>> GetBankingInstitutions(BankingInstitutionRepository repository,
        string? countryCode = null)
    {
        return await repository.GetAll(countryCode);
    }
}