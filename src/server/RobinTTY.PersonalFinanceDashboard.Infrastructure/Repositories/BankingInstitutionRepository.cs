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

    // TODO: Refactor this into appropriate classes
    /// <summary>
    /// Gets all <see cref="BankingInstitution"/>s.
    /// </summary>
    /// <returns>A list of all <see cref="BankingInstitution"/>s.</returns>
    public async Task<IEnumerable<BankingInstitution>> GetBankingInstitutions(string? countryCode)
    {
        var dataRetrievalMetadata = new List<ThirdPartyDataRetrievalMetadata>
        {
            new()
            {
                DataType = ThirdPartyDataType.BankingInstitutions,
                DataSource = ThirdPartyDataSource.GoCardless,
                LastRetrievalTime = DateTime.MinValue,
                RetrievalInterval = TimeSpan.FromSeconds(30) // TODO: should be something like 1 day or more
            }
        };

        var institutionRetrievalMetadata =
            dataRetrievalMetadata.Single(metadata => metadata.DataType == ThirdPartyDataType.BankingInstitutions);

        var nextRetrievalTime = institutionRetrievalMetadata.LastRetrievalTime +
                                institutionRetrievalMetadata.RetrievalInterval;


        if (nextRetrievalTime < DateTime.Now)
        {
            var response = await _dataProvider.GetBankingInstitutions();
            if (response.IsSuccessful)
            {
                // Delete existing rows in table
                var affectedRows = await _dbContext.BankingInstitutions.ExecuteDeleteAsync();

                // Add the institutions returned in the API response
                var institutionEntities =
                    response.Result.Select(institution => _bankingInstitutionMapper.ModelToEntity(institution));
                await _dbContext.BankingInstitutions.AddRangeAsync(institutionEntities);

                // TODO: update LastRetrievalTime
                
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                // TODO: What to do in case of failure should depend on if we already have data
            }
        }

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
}

public enum ThirdPartyDataSource
{
    Undefined,
    GoCardless
}

public enum ThirdPartyDataType
{
    Undefined,
    BankingInstitutions
}

public class ThirdPartyDataRetrievalMetadata
{
    /// <summary>
    /// The type of data being retrieved. 
    /// </summary>
    public ThirdPartyDataType DataType { get; set; }
    /// <summary>
    /// The source of the data.
    /// </summary>
    public ThirdPartyDataSource DataSource { get; set; }
    /// <summary>
    /// The <see cref="DateTime"/> at which the data was retrieved last.
    /// </summary>
    public DateTime LastRetrievalTime { get; set; }
    /// <summary>
    /// The interval at which the data should be retrieved again from the data source.
    /// </summary>
    public TimeSpan RetrievalInterval { get; set; }
}
