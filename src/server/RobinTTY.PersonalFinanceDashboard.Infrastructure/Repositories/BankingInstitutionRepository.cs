using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Services;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Services.DataSynchronization;
using RobinTTY.PersonalFinanceDashboard.ThirdPartyDataProviders;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Repositories;

/// <summary>
/// Manages <see cref="BankingInstitution"/> data retrieval.
/// </summary>
public class BankingInstitutionRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly BankingInstitutionSyncHandler _bankingInstitutionSyncHandler;
    private readonly ILogger<BankingInstitutionRepository> _logger;

    /// <summary>
    /// Creates a new instance of <see cref="BankingInstitutionRepository"/>.
    /// </summary>
    /// <param name="dbContext">The <see cref="ApplicationDbContext"/> to use for data retrieval.</param>
    /// <param name="bankingInstitutionSyncHandler">Handles the synchronization of third party data.</param>
    /// <param name="logger">Logger used for monitoring purposes.</param>
    public BankingInstitutionRepository(
        ApplicationDbContext dbContext,
        BankingInstitutionSyncHandler bankingInstitutionSyncHandler,
        ILogger<BankingInstitutionRepository> logger)
    {
        _dbContext = dbContext;
        _bankingInstitutionSyncHandler = bankingInstitutionSyncHandler;
        _logger = logger;
    }

    /// <summary>
    /// Gets the <see cref="BankingInstitution"/> matching the specified id.
    /// </summary>
    /// <param name="institutionId">The id of the <see cref="BankingInstitution"/> to retrieve.</param>
    /// <returns>The <see cref="BankingInstitution"/> if one ist matched otherwise <see langword="null"/>.</returns>
    public async Task<IQueryable<BankingInstitution?>> GetBankingInstitution(Guid institutionId)
    {
        await _bankingInstitutionSyncHandler.SynchronizeData();

        return _dbContext.BankingInstitutions.Where(institution => institution.Id == institutionId);
    }

    /// <summary>
    /// Gets all <see cref="BankingInstitution"/>s.
    /// </summary>
    /// <returns>A list of <see cref="BankingInstitution"/>s.</returns>
    public async Task<IQueryable<BankingInstitution>> GetBankingInstitutions(string? countryCode)
    {
        await _bankingInstitutionSyncHandler.SynchronizeData();

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
    /// Deletes an existing <see cref="BankingInstitution"/>.
    /// </summary>
    /// <param name="institutionId">The id of the <see cref="BankingInstitution"/> to delete.</param>
    /// <returns>Boolean value indicating whether the operation was successful.</returns>
    public async Task<bool> DeleteBankingInstitution(Guid institutionId)
    {
        var result = await _dbContext.BankingInstitutions.Where(t => t.Id == institutionId).ExecuteDeleteAsync();
        return Convert.ToBoolean(result);
    }
}
