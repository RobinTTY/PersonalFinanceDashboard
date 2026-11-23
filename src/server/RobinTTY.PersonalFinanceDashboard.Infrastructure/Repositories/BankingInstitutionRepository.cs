using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RobinTTY.PersonalFinanceDashboard.Core.Models;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Repositories;

/// <summary>
/// Manages <see cref="BankingInstitution"/> data retrieval.
/// </summary>
public class BankingInstitutionRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<BankingInstitutionRepository> _logger;

    /// <summary>
    /// Creates a new instance of <see cref="BankingInstitutionRepository"/>.
    /// </summary>
    /// <param name="dbContext">The <see cref="ApplicationDbContext"/> to use for data retrieval.</param>
    /// <param name="logger">Logger used for monitoring purposes.</param>
    public BankingInstitutionRepository(
        ApplicationDbContext dbContext,
        ILogger<BankingInstitutionRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// Gets the <see cref="BankingInstitution"/> matching the specified id.
    /// </summary>
    /// <param name="institutionId">The id of the <see cref="BankingInstitution"/> to retrieve.</param>
    /// <returns>The <see cref="BankingInstitution"/> if one ist matched otherwise <see langword="null"/>.</returns>
    public IQueryable<BankingInstitution?> GetBankingInstitution(Guid institutionId) =>
        _dbContext.BankingInstitutions.Where(institution => institution.Id == institutionId);

    /// <summary>
    /// Loads the <see cref="BankingInstitution"/> matching the specified id.
    /// </summary>
    /// <param name="institutionId">The id of the <see cref="BankingInstitution"/> to retrieve.</param>
    /// <param name="cancellationToken">A token to signal cancellation.</param>
    /// <returns>The <see cref="BankingInstitution"/> if one is matched, otherwise <see langword="null"/>.</returns>
    public async Task<BankingInstitution?> FindBankingInstitution(Guid institutionId,
        CancellationToken cancellationToken = default) =>
        await _dbContext.BankingInstitutions.SingleOrDefaultAsync(institution => institution.Id == institutionId,
            cancellationToken);

    /// <summary>
    /// Gets all <see cref="BankingInstitution"/>s.
    /// </summary>
    /// <returns>A list of <see cref="BankingInstitution"/>s.</returns>
    public IQueryable<BankingInstitution> GetBankingInstitutions(string? countryCode)
    {
        var query = countryCode == null
            ? _dbContext.BankingInstitutions
            : _dbContext.BankingInstitutions.Where(institution => institution.Countries.Contains(countryCode));

        return query.OrderBy(institution => institution.Name).ThenBy(institution => institution.Id);
    }

    /// <summary>
    /// Adds a new <see cref="BankingInstitution"/>.
    /// </summary>
    /// <param name="bankingInstitution">The <see cref="BankingInstitution"/> to add.</param>
    /// <param name="cancellationToken">A token to signal cancellation.</param>
    /// <returns>The added <see cref="BankingInstitution"/>.</returns>
    public async Task<BankingInstitution> AddBankingInstitution(BankingInstitution bankingInstitution,
        CancellationToken cancellationToken = default)
    {
        var result = await _dbContext.BankingInstitutions.AddAsync(bankingInstitution, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return result.Entity;
    }

    /// <summary>
    /// Updates an existing <see cref="BankingInstitution"/>.
    /// </summary>
    /// <param name="institutionId">The id of the <see cref="BankingInstitution"/> to update.</param>
    /// <param name="name">The name of the institution.</param>
    /// <param name="bic">The Business Identifier Code (BIC) of the institution.</param>
    /// <param name="logoUri">A <see cref="Uri"/> for the logo of the institution.</param>
    /// <param name="countries">The countries the institution operates in.</param>
    /// <param name="cancellationToken">A token to signal cancellation.</param>
    /// <returns>The updated <see cref="BankingInstitution"/>, or <see langword="null"/> if it doesn't exist.</returns>
    public async Task<BankingInstitution?> UpdateBankingInstitution(Guid institutionId, string name, string bic,
        Uri logoUri, List<string> countries, CancellationToken cancellationToken = default)
    {
        var institution = await _dbContext.BankingInstitutions
            .SingleOrDefaultAsync(institution => institution.Id == institutionId, cancellationToken);
        if (institution == null)
            return null;

        institution.Name = name;
        institution.Bic = bic;
        institution.LogoUri = logoUri;
        institution.Countries = countries;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return institution;
    }

    /// <summary>
    /// Deletes an existing <see cref="BankingInstitution"/>.
    /// </summary>
    /// <param name="institutionId">The id of the <see cref="BankingInstitution"/> to delete.</param>
    /// <param name="cancellationToken">A token to signal cancellation.</param>
    /// <returns>Boolean value indicating whether the operation was successful.</returns>
    public async Task<bool> DeleteBankingInstitution(Guid institutionId, CancellationToken cancellationToken = default)
    {
        var deleted = await _dbContext.BankingInstitutions.Where(t => t.Id == institutionId)
            .ExecuteDeleteAsync(cancellationToken);
        return deleted > 0;
    }
}
