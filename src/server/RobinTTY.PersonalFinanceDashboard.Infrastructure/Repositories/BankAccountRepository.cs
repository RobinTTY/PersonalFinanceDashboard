using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Extensions;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Services;
using RobinTTY.PersonalFinanceDashboard.ThirdPartyDataProviders;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Repositories;

/// <summary>
/// Manages <see cref="BankAccount"/> data retrieval.
/// </summary>
public class BankAccountRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly GoCardlessDataProviderService _dataProviderService;
    private readonly ThirdPartyDataRetrievalMetadataService _dataRetrievalMetadataService;
    private readonly ILogger<BankAccountRepository> _logger;

    /// <summary>
    /// Creates a new instance of <see cref="BankAccountRepository"/>.
    /// </summary>
    /// <param name="dbContext">The <see cref="ApplicationDbContext"/> to use for data retrieval.</param>
    /// <param name="dataProviderService">The data provider to use for data retrieval.</param>
    /// <param name="dataRetrievalMetadataService">Service used to determine if the database data is stale.</param>
    /// <param name="logger">Logger used for monitoring purposes.</param>
    public BankAccountRepository(
        ApplicationDbContext dbContext,
        GoCardlessDataProviderService dataProviderService,
        ThirdPartyDataRetrievalMetadataService dataRetrievalMetadataService,
        ILogger<BankAccountRepository> logger)
    {
        _dbContext = dbContext;
        _dataProviderService = dataProviderService;
        _dataRetrievalMetadataService = dataRetrievalMetadataService;
        _logger = logger;
    }

    /// <summary>
    /// Gets the <see cref="BankAccount"/> matching the specified id.
    /// </summary>
    /// <param name="accountId">The id of the <see cref="BankAccount"/> to retrieve.</param>
    /// <returns>The <see cref="BankAccount"/> if one ist matched otherwise <see langword="null"/>.</returns>
    public async Task<IQueryable<BankAccount?>> GetBankAccount(Guid accountId)
    {
        await RefreshBankAccountsIfStale();

        return _dbContext.BankAccounts.Where(account => account.Id == accountId);
    }

    /// <summary>
    /// Gets a list of <see cref="BankAccount"/>s.
    /// </summary>
    /// <returns>A list of <see cref="BankAccount"/>s.</returns>
    public async Task<IQueryable<BankAccount>> GetBankAccounts()
    {
        await RefreshBankAccountsIfStale();

        return _dbContext.BankAccounts;
    }

    /// <summary>
    /// Adds a new <see cref="BankAccount"/>.
    /// </summary>
    /// <param name="bankAccount">The <see cref="BankAccount"/>s to add.</param>
    public async Task<BankAccount> AddBankAccount(BankAccount bankAccount)
    {
        var entityEntry = _dbContext.BankAccounts.Add(bankAccount);
        await _dbContext.SaveChangesAsync();

        return entityEntry.Entity;
    }

    /// <summary>
    /// Updates an existing <see cref="BankAccount"/>.
    /// </summary>
    /// <param name="bankAccount">The bank account to update.</param>
    /// <returns>The updated <see cref="BankAccount"/>.</returns>
    public async Task<BankAccount> UpdateBankAccount(BankAccount bankAccount)
    {
        var entityEntry = _dbContext.BankAccounts.Update(bankAccount);
        await _dbContext.SaveChangesAsync();

        return entityEntry.Entity;
    }

    /// <summary>
    /// Deletes an existing <see cref="BankAccount"/>.
    /// </summary>
    /// <param name="bankAccountId">The id of the <see cref="BankAccount"/> to delete.</param>
    /// <returns>Boolean value indicating whether the operation was successful.</returns>
    public async Task<bool> DeleteBankAccount(Guid bankAccountId)
    {
        var result = await _dbContext.BankAccounts.Where(t => t.Id == bankAccountId).ExecuteDeleteAsync();
        return Convert.ToBoolean(result);
    }

    /// <summary>
    /// Refreshes the list of bank accounts if the data has gone stale or if they haven't been fully populated yet.
    /// </summary>
    private async Task RefreshBankAccountsIfStale()
    {
        var dataIsStale = await _dataRetrievalMetadataService.DataIsStale(ThirdPartyDataType.BankAccounts);
        List<BankAccount> accountsToUpdate = [];

        if (dataIsStale)
        {
            var allAccounts = await _dbContext.BankAccounts.ToListAsync();
            accountsToUpdate.AddRange(allAccounts);
        }
        else
        {
            var nonPopulatedAccounts =
                await _dbContext.BankAccounts.Where(account => account.Iban == null).ToListAsync();
            accountsToUpdate.AddRange(nonPopulatedAccounts);
        }

        if (accountsToUpdate.Count > 0)
        {
            var accountIdsToUpdate = accountsToUpdate
                .Select(bankAccount => bankAccount.ThirdPartyId).ToList();
            var responses = await _dataProviderService.GetBankAccounts(accountIdsToUpdate);
            var responsesSuccessful = responses.All(resp => resp.IsSuccessful);

            if (responsesSuccessful)
            {
                await SyncBankAccounts(accountsToUpdate);
                await _dataRetrievalMetadataService.ResetDataExpiry(ThirdPartyDataType.BankAccounts);

                _logger.LogInformation(
                    "Refreshed stale bank account data. {updateRecords} records were updated.",
                    accountsToUpdate.Count);
            }
            else
            {
                var error = responses
                    .Where(response => !response.IsSuccessful)
                    .Select(response => response.Error)
                    .FirstOrDefault();

                _logger.LogError(
                    "Refreshing stale banking institutions failed. Error summary: \"{message}\" Error details: \"{details}\"",
                    error?.Summary, error?.Detail);

                // TODO: What to do in case of failure should depend on if we already have data
                // Log failure and continue, maybe also send a notification to frontend, maybe through SignalR endpoint
            }
        }
    }

    /// <summary>
    /// Syncs the bank accounts the database contains with the external data provider.
    /// </summary>
    /// <param name="bankAccounts">The list of <see cref="BankAccount"/>s to add.</param>
    private async Task SyncBankAccounts(List<BankAccount> bankAccounts)
    {
        await AddOrUpdateBankAccounts(bankAccounts);
        await DeleteOutdatedBankingInstitutions(bankAccounts);
    }

    /// <summary>
    /// Adds or updates bank accounts based on the information retrieved from the third party data provider.
    /// </summary>
    /// <param name="bankAccounts">The bank accounts retrieved from the third party data provider.</param>
    private async Task AddOrUpdateBankAccounts(List<BankAccount> bankAccounts)
    {
        foreach (var bankAccount in bankAccounts)
        {
            var associatedInstitution = bankAccount.AssociatedInstitution;
            var associatedAuthenticationRequests = bankAccount.AssociatedAuthenticationRequests.ToList();
            bankAccount.AssociatedInstitution = null;
            bankAccount.AssociatedAuthenticationRequests.Clear();

            if (associatedAuthenticationRequests.All(request => request.Status != AuthenticationStatus.Active))
                continue;
            
            await _dbContext.AddOrUpdateBankAccount(bankAccount);
            await LinkAssociatedInstitutionToBankAccount(bankAccount, associatedInstitution);
            await LinkAssociatedAuthenticationRequestsToBankAccount(bankAccount, associatedAuthenticationRequests);
        }

        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Adds relationships between a bank account and the associated authentication requests.
    /// </summary>
    /// <param name="bankAccount">The bank account to which to link the authentication requests.</param>
    /// <param name="associatedAuthenticationRequests">The associated accounts to link.</param>
    private async Task LinkAssociatedAuthenticationRequestsToBankAccount(BankAccount bankAccount,
        List<AuthenticationRequest> associatedAuthenticationRequests)
    {
        foreach (var authenticationRequest in associatedAuthenticationRequests)
        {
            var matchingAuthenticationRequest = await _dbContext.AuthenticationRequests
                .SingleOrDefaultAsync(account => account.ThirdPartyId == authenticationRequest.ThirdPartyId);

            if (matchingAuthenticationRequest is null)
            {
                var result = await _dbContext.AuthenticationRequests.AddAsync(authenticationRequest);
                matchingAuthenticationRequest = result.Entity;
            }

            bankAccount.AssociatedAuthenticationRequests.Add(matchingAuthenticationRequest);
        }
    }

    private async Task LinkAssociatedInstitutionToBankAccount(BankAccount bankAccount,
        BankingInstitution? bankingInstitution)
    {
        if (bankingInstitution is null) return;

        var matchingInstitution = await _dbContext.BankingInstitutions
            .SingleOrDefaultAsync(institution => institution.ThirdPartyId == bankingInstitution.ThirdPartyId);

        if (matchingInstitution is null)
        {
            var result = await _dbContext.BankingInstitutions.AddAsync(bankingInstitution);
            matchingInstitution = result.Entity;
        }

        bankAccount.AssociatedInstitution = matchingInstitution;
    }

    /// <summary>
    /// Removes bank accounts which are no longer tracked by the third party data provider from the db.
    /// </summary>
    /// <param name="bankAccounts">The bank accounts as retrieved from the third party data provider.</param>
    private async Task DeleteOutdatedBankingInstitutions(List<BankAccount> bankAccounts)
    {
        var updatedBankAccountIds = bankAccounts.Select(account => account.ThirdPartyId).ToList();
        await _dbContext.BankAccounts
            .Where(dbAccount =>
                updatedBankAccountIds.All(updatedInstitutionId =>
                    updatedInstitutionId != dbAccount.ThirdPartyId))
            .ExecuteDeleteAsync();
    }
}