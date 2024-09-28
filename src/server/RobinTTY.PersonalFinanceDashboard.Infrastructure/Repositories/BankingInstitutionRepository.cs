using Microsoft.EntityFrameworkCore;
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
    private readonly ApplicationDbContext _dbContext;
    private readonly GoCardlessDataProvider _dataProvider;
    private readonly BankingInstitutionMapper _bankingInstitutionMapper;
    private readonly ThirdPartyDataRetrievalMetadataService _dataRetrievalMetadataService;

    /// <summary>
    /// Creates a new instance of <see cref="BankingInstitutionRepository"/>.
    /// </summary>
    /// <param name="dbContext">The <see cref="ApplicationDbContext"/> to use for data retrieval.</param>
    /// <param name="dataProvider">The data provider to use for data retrieval.</param>
    /// <param name="bankingInstitutionMapper">The mapper used to map ef entities to the domain model.</param>
    /// <param name="dataRetrievalMetadataService">TODO</param>
    public BankingInstitutionRepository(ApplicationDbContext dbContext, GoCardlessDataProvider dataProvider,
        BankingInstitutionMapper bankingInstitutionMapper,
        ThirdPartyDataRetrievalMetadataService dataRetrievalMetadataService)
    {
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
    /// TODO
    /// </summary>
    /// <param name="bankingInstitutions"></param>
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

    /// <summary>
    /// TODO
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
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
                
                // TODO: Replace with logger
                Console.WriteLine("Banking institutions have been refreshed.");
            }
            else
            {
                // TODO: What to do in case of failure should depend on if we already have data
                throw new NotImplementedException();
            }
        }
    }
}
