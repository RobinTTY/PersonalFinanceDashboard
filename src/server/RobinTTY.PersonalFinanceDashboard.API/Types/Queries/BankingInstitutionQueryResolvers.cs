using HotChocolate.Types;
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
    public async Task<BankingInstitution> GetBankingInstitution(string institutionId)
    {
        var request = await _dataProvider.GetBankingInstitution(institutionId);
        return request.Result!;
    }

    /// <summary>
    /// Look up banking institutions.
    /// </summary>
    /// <param name="countryCode">Optional filter by country the institution operates in.</param>
    [UsePaging(MaxPageSize = 3000)]
    public async Task<IQueryable<BankingInstitution>> GetBankingInstitutions(string? countryCode = null) {
        var request = await _dataProvider.GetBankingInstitutions(countryCode);
        return request.Result!;
    }
}