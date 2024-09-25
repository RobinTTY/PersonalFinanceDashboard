using Microsoft.EntityFrameworkCore;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Mappers;
using RobinTTY.PersonalFinanceDashboard.ThirdPartyDataProviders;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Repositories;

/// <summary>
/// Manages <see cref="BankingInstitution"/> data retrieval.
/// </summary>
public class BankingInstitutionRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly GoCardlessDataProvider _dataProvider;
    private readonly BankingInstitutionMapper _bankingInstitutionMapper;

    /// <summary>
    /// Creates a new instance of <see cref="BankingInstitutionRepository"/>.
    /// </summary>
    /// <param name="dbContext">The <see cref="ApplicationDbContext"/> to use for data retrieval.</param>
    /// <param name="dataProvider">The data provider to use for data retrieval.</param>
    /// <param name="bankingInstitutionMapper">The mapper used to map ef entities to the domain model.</param>
    public BankingInstitutionRepository(ApplicationDbContext dbContext, GoCardlessDataProvider dataProvider,
        BankingInstitutionMapper bankingInstitutionMapper)
    {
        _dbContext = dbContext;
        _dataProvider = dataProvider;
        _bankingInstitutionMapper = bankingInstitutionMapper;
    }

    /// <summary>
    /// Gets the <see cref="BankingInstitution"/> matching the specified id.
    /// </summary>
    /// <param name="institutionId">The id of the <see cref="BankingInstitution"/> to retrieve.</param>
    /// <returns>The <see cref="BankingInstitution"/> if one ist matched otherwise <see langword="null"/>.</returns>
    public async Task<BankingInstitution?> GetBankingInstitution(string institutionId)
    {
        // var request = await _dataProvider.GetBankingInstitution(institutionId);
        var bankingInstitutionEntity = await _dbContext.BankingInstitutions
            .SingleOrDefaultAsync(institution => institution.Id == institutionId);

        return bankingInstitutionEntity == null
            ? null
            : _bankingInstitutionMapper.EntityToModel(bankingInstitutionEntity);
    }

    /// <summary>
    /// Gets all <see cref="BankingInstitution"/>s.
    /// </summary>
    /// <returns>A list of all <see cref="BankingInstitution"/>s.</returns>
    public async Task<IEnumerable<BankingInstitution>> GetBankingInstitutions(string? countryCode)
    {
        // var request = await _dataProvider.GetBankingInstitutions(countryCode);
        var bankingInstitutionEntities = await _dbContext.BankingInstitutions.ToListAsync();
        var bankingInstitutionModels = bankingInstitutionEntities
            .Select(institution => _bankingInstitutionMapper.EntityToModel(institution))
            .ToList();

        return bankingInstitutionModels;
    }
}
