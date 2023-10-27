using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.ThirdPartyDataProviders;

namespace RobinTTY.PersonalFinanceDashboard.API.Repositories;

/// <summary>
/// Manages <see cref="BankingInstitution"/> data retrieval.
/// </summary>
public class BankingInstitutionRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly GoCardlessDataProvider _dataProvider;

    /// <summary>
    /// Creates a new instance of <see cref="BankingInstitutionRepository"/>.
    /// </summary>
    /// <param name="dbContext">The <see cref="ApplicationDbContext"/> to use for data retrieval.</param>
    /// <param name="dataProvider">The data provider to use for data retrieval.</param>
    public BankingInstitutionRepository(ApplicationDbContext dbContext, GoCardlessDataProvider dataProvider)
    {
        _dbContext = dbContext;
        _dataProvider = dataProvider;
    }
    
    /// <summary>
    /// Gets the <see cref="BankingInstitution"/> matching the specified id.
    /// </summary>
    /// <param name="institutionId">The id of the <see cref="BankingInstitution"/> to retrieve.</param>
    /// <returns>The <see cref="BankingInstitution"/> if one ist matched otherwise <see langword="null"/>.</returns>
    public async Task<BankingInstitution?> Get(string institutionId)
    {
        var request = await _dataProvider.GetBankingInstitution(institutionId);
        return request.Result!;
    }

    /// <summary>
    /// Gets all <see cref="BankingInstitution"/>s.
    /// </summary>
    /// <returns>A list of all <see cref="BankingInstitution"/>s.</returns>
    public async Task<IEnumerable<BankingInstitution>> GetAll(string? countryCode)
    {
        var request = await _dataProvider.GetBankingInstitutions(countryCode);
        return request.Result!;
    }
}