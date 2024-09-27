using Microsoft.EntityFrameworkCore;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Entities;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Mappers;
using RobinTTY.PersonalFinanceDashboard.ThirdPartyDataProviders;
using RobinTTY.PersonalFinanceDashboard.ThirdPartyDataProviders.Models;

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

    // TODO: Refactor this into appropriate classes
    /// <summary>
    /// Gets all <see cref="BankingInstitution"/>s.
    /// </summary>
    /// <returns>A list of all <see cref="BankingInstitution"/>s.</returns>
    public async Task<IEnumerable<BankingInstitution>> GetBankingInstitutions(string? countryCode)
    {
        var dataRetrievalMetadata = new List<ThirdPartyDataRetrievalMetadataEntity>
        {
            new()
            {
                DataType = ThirdPartyDataType.BankingInstitutions,
                DataSource = ThirdPartyDataSource.GoCardless,
                LastRetrievalTime = DateTime.MinValue,
                RetrievalInterval = TimeSpan.FromSeconds(30) // TODO: should be something like 1 day or more
            }
        };

        var bankingInstitutionEntities = countryCode == null
            ? await _dbContext.BankingInstitutions
                .ToListAsync()
            : await _dbContext.BankingInstitutions
                .Where(institution => institution.Countries.Contains(countryCode))
                .ToListAsync();

        var bankingInstitutionModels = bankingInstitutionEntities
            .Select(institution => _bankingInstitutionMapper.EntityToModel(institution))
            .ToList();

        return bankingInstitutionModels;
    }

    public async Task AddBankingInstitutions(IEnumerable<BankingInstitution> bankingInstitutions)
    {
        var institutionEntities =
            bankingInstitutions.Select(institution => _bankingInstitutionMapper.ModelToEntity(institution));
        
        await _dbContext.BankingInstitutions.AddRangeAsync(institutionEntities);
        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// TODO
    /// </summary>
    /// <returns></returns>
    public async Task<int> DeleteBankingInstitutions()
    {
        return await _dbContext.BankingInstitutions.ExecuteDeleteAsync();
    }
}
