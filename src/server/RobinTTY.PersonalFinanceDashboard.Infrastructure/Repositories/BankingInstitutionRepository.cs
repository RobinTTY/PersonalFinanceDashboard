using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Services;
using RobinTTY.PersonalFinanceDashboard.ThirdPartyDataProviders;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Repositories;

/// <summary>
/// Manages <see cref="BankingInstitution"/> data retrieval.
/// </summary>
public class BankingInstitutionRepository
{
    private readonly ILogger<BankingInstitutionRepository> _logger;
    private readonly ApplicationDbContext _dbContext;
    private readonly GoCardlessDataProviderService _dataProviderService;
    private readonly ThirdPartyDataRetrievalMetadataService _dataRetrievalMetadataService;

    /// <summary>
    /// Creates a new instance of <see cref="BankingInstitutionRepository"/>.
    /// </summary>
    /// <param name="logger">Logger used for monitoring purposes.</param>
    /// <param name="dbContext">The <see cref="ApplicationDbContext"/> to use for data retrieval.</param>
    /// <param name="dataProviderService">The data provider to use for data retrieval.</param>
    /// <param name="dataRetrievalMetadataService">Service used to determine if the database data is stale.</param>
    public BankingInstitutionRepository(
        ILogger<BankingInstitutionRepository> logger,
        ApplicationDbContext dbContext,
        GoCardlessDataProviderService dataProviderService,
        ThirdPartyDataRetrievalMetadataService dataRetrievalMetadataService)
    {
        _logger = logger;
        _dbContext = dbContext;
        _dataProviderService = dataProviderService;
        _dataRetrievalMetadataService = dataRetrievalMetadataService;
    }

    /// <summary>
    /// Gets the <see cref="BankingInstitution"/> matching the specified id.
    /// </summary>
    /// <param name="institutionId">The id of the <see cref="BankingInstitution"/> to retrieve.</param>
    /// <returns>The <see cref="BankingInstitution"/> if one ist matched otherwise <see langword="null"/>.</returns>
    public async Task<IQueryable<BankingInstitution?>> GetBankingInstitution(Guid institutionId)
    {
        await RefreshBankingInstitutionsIfStale();

        return _dbContext.BankingInstitutions.Where(institution => institution.Id == institutionId);
    }

    /// <summary>
    /// Gets all <see cref="BankingInstitution"/>s.
    /// </summary>
    /// <returns>A list of <see cref="BankingInstitution"/>s.</returns>
    public async Task<IQueryable<BankingInstitution>> GetBankingInstitutions(string? countryCode)
    {
        await RefreshBankingInstitutionsIfStale();

        return countryCode == null
            ? _dbContext.BankingInstitutions
            : _dbContext.BankingInstitutions.Where(institution => institution.Countries.Contains(countryCode));
    }

    /// <summary>
    /// Adds a list of new <see cref="BankingInstitution"/>s.
    /// </summary>
    /// <param name="bankingInstitutions">The list of <see cref="BankingInstitution"/>s to add.</param>
    /// <returns>The number of records that were added.</returns>
    public async Task<int> AddBankingInstitutions(IEnumerable<BankingInstitution> bankingInstitutions)
    {
        await _dbContext.BankingInstitutions.AddRangeAsync(bankingInstitutions);
        return await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Adds a new <see cref="BankingInstitution"/>.
    /// </summary>
    /// <param name="bankingInstitution">The <see cref="BankingInstitution"/> to add.</param>
    /// <returns>The added <see cref="BankingInstitution"/>.</returns>
    public async Task<BankingInstitution> AddBankingInstitution(BankingInstitution bankingInstitution)
    {
        var result = await _dbContext.BankingInstitutions.AddAsync(bankingInstitution);
        await _dbContext.SaveChangesAsync();

        return result.Entity;
    }

    /// <summary>
    /// Updates an existing <see cref="BankingInstitution"/>.
    /// </summary>
    /// <param name="bankingInstitution">The banking institution to update.</param>
    /// <returns>The updated <see cref="BankingInstitution"/>.</returns>
    public async Task<BankingInstitution> UpdateBankingInstitution(BankingInstitution bankingInstitution)
    {
        var updatedEntity = _dbContext.BankingInstitutions.Update(bankingInstitution);
        await _dbContext.SaveChangesAsync();

        return updatedEntity.Entity;
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
    public async Task<bool> DeleteBankingInstitution(Guid institutionId)
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
            var response = await _dataProviderService.GetBankingInstitutions();
            if (response.IsSuccessful)
            {
                // TODO: This needs to be updated to account for user generated banking institutions
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
