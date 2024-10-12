using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Mappers;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Services;
using RobinTTY.PersonalFinanceDashboard.ThirdPartyDataProviders;
using RobinTTY.PersonalFinanceDashboard.ThirdPartyDataProviders.Models;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Repositories;

/// <summary>
/// Manages <see cref="BankingInstitution"/> data retrieval.
/// </summary>
public class BankingInstitutionRepository
{
    private readonly ILogger _logger;
    private readonly ApplicationDbContext _dbContext;
    private readonly GoCardlessDataProvider _dataProvider;
    private readonly BankingInstitutionMapper _bankingInstitutionMapper;
    private readonly ThirdPartyDataRetrievalMetadataService _dataRetrievalMetadataService;

    /// <summary>
    /// Creates a new instance of <see cref="BankingInstitutionRepository"/>.
    /// </summary>
    /// <param name="logger">Logger used for monitoring purposes.</param>
    /// <param name="dbContext">The <see cref="ApplicationDbContext"/> to use for data retrieval.</param>
    /// <param name="dataProvider">The data provider to use for data retrieval.</param>
    /// <param name="bankingInstitutionMapper">The mapper used to map ef entities to the domain model.</param>
    /// <param name="dataRetrievalMetadataService">Service used to determine if the database data is stale.</param>
    public BankingInstitutionRepository(
        ILogger<BankingInstitutionRepository> logger,
        ApplicationDbContext dbContext,
        GoCardlessDataProvider dataProvider,
        BankingInstitutionMapper bankingInstitutionMapper,
        ThirdPartyDataRetrievalMetadataService dataRetrievalMetadataService)
    {
        _logger = logger;
        _dbContext = dbContext;
        _dataProvider = dataProvider;
        _bankingInstitutionMapper = bankingInstitutionMapper;
        _dataRetrievalMetadataService = dataRetrievalMetadataService;
    }

    /// <summary>
    /// Gets the <see cref="BankingInstitution"/> matching the specified id.
    /// </summary>
    /// <param name="institutionId">The id of the <see cref="BankingInstitution"/> to retrieve.</param>
    /// <returns>The <see cref="BankingInstitution"/> if one ist matched otherwise <see langword="null"/>.</returns>
    public async Task<BankingInstitution?> GetBankingInstitution(string institutionId)
    {
        await RefreshBankingInstitutionsIfStale();
        
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
    /// <returns>A list of <see cref="BankingInstitution"/>s.</returns>
    public async Task<IEnumerable<BankingInstitution>> GetBankingInstitutions(string? countryCode)
    {
        await RefreshBankingInstitutionsIfStale();

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

    /// <summary>
    /// Adds a list of new <see cref="BankingInstitution"/>s.
    /// </summary>
    /// <param name="bankingInstitutions">The list of <see cref="BankingInstitution"/>s to add.</param>
    public async Task AddBankingInstitutions(IEnumerable<BankingInstitution> bankingInstitutions)
    {
        var institutionEntities =
            bankingInstitutions.Select(institution => _bankingInstitutionMapper.ModelToEntity(institution));

        await _dbContext.BankingInstitutions.AddRangeAsync(institutionEntities);
        await _dbContext.SaveChangesAsync();
    }
    
    /// <summary>
    /// Adds a new <see cref="BankingInstitution"/>.
    /// </summary>
    /// <param name="bankingInstitution">The <see cref="BankingInstitution"/> to add.</param>
    public async Task<BankingInstitution> AddBankingInstitution(BankingInstitution bankingInstitution)
    {
        var institutionEntities =_bankingInstitutionMapper.ModelToEntity(bankingInstitution);
        var result = await _dbContext.BankingInstitutions.AddAsync(institutionEntities);
        await _dbContext.SaveChangesAsync();

        return _bankingInstitutionMapper.EntityToModel(result.Entity);
    }

    /// <summary>
    /// Updates an existing <see cref="BankingInstitution"/>.
    /// </summary>
    /// <param name="bankingInstitution">The banking institution to update.</param>
    /// <returns>The updated <see cref="BankingInstitution"/>.</returns>
    public async Task<BankingInstitution> UpdateBankingInstitution(BankingInstitution bankingInstitution)
    {
        var institutionEntity = _bankingInstitutionMapper.ModelToEntity(bankingInstitution);
        var updatedEntity = _dbContext.BankingInstitutions.Update(institutionEntity);
        await _dbContext.SaveChangesAsync();

        return _bankingInstitutionMapper.EntityToModel(updatedEntity.Entity);
    }

    /// <summary>
    /// Deletes all existing <see cref="BankingInstitution"/>s.
    /// </summary>
    /// <returns>The number of deleted records.</returns>
    public async Task<int> DeleteBankingInstitutions()
    {
        return await _dbContext.BankingInstitutions.ExecuteDeleteAsync();
    }
    
    /// <summary>
    /// Deletes an existing <see cref="BankingInstitution"/>.
    /// </summary>
    /// <param name="institutionId">The id of the <see cref="BankingInstitution"/> to delete.</param>
    /// <returns>Boolean value indicating whether the operation was successful.</returns>
    public async Task<bool> DeleteBankingInstitution(string institutionId)
    {
        var result = await _dbContext.BankingInstitutions.Where(t => t.Id == institutionId).ExecuteDeleteAsync();
        return Convert.ToBoolean(result);
    }

    /// <summary>
    /// Refreshes the list of banking institutions if the data has gone stale.
    /// </summary>
    /// <exception cref="NotImplementedException">TODO</exception>
    private async Task RefreshBankingInstitutionsIfStale()
    {
        var dataIsStale = await _dataRetrievalMetadataService.DataIsStale(ThirdPartyDataType.BankingInstitutions);
        if (dataIsStale)
        {
            var response = await _dataProvider.GetBankingInstitutions();
            if (response.IsSuccessful)
            {
                await DeleteBankingInstitutions();
                await AddBankingInstitutions(response.Result);
                await _dataRetrievalMetadataService.ResetDataExpiry(ThirdPartyDataType.BankingInstitutions);

                _logger.LogInformation(
                    "Refreshed stale banking institution data. {updateRecords} records were updated.",
                    response.Result.Count());
            }
            else
            {
                // TODO: What to do in case of failure should depend on if we already have data
                // Log failure and continue, maybe also send a notification to frontend
                throw new NotImplementedException();
            }
        }
    }
}
