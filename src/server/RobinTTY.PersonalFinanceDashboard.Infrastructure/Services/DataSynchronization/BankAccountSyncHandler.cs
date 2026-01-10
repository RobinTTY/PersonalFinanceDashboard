using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Services.DataSynchronization.Interfaces;
using RobinTTY.PersonalFinanceDashboard.ThirdPartyDataProviders;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Services.DataSynchronization;

public class BankAccountSyncHandler(
    ApplicationDbContext dbContext,
    GoCardlessDataProviderService dataProvider,
    ThirdPartyDataRetrievalMetadataService dataRetrievalMetadataService,
    ILogger<BankAccountSyncHandler> logger) : IBankAccountSyncHandler
{
    public async Task<bool> SynchronizeData(Guid? bankAccountId = null, bool forceThirdPartySync = false)
    {
        var dataIsStale = await dataRetrievalMetadataService.DataIsStale(ThirdPartyDataType.BankAccounts);
        if (dataIsStale || forceThirdPartySync)
        {
            if (bankAccountId.HasValue)
                bankAccountId = dbContext.BankAccounts
                    .SingleOrDefault(bankAccount => bankAccount.Id == bankAccountId)?.ThirdPartyId;
            
            var bankAccounts = await GetBankAccounts(bankAccountId);
            if (bankAccounts == null || bankAccounts.Count == 0)
            {
                return false;
            }

            await AddOrUpdateBankAccounts(bankAccounts);
            await dataRetrievalMetadataService.ResetDataExpiry(ThirdPartyDataType.BankAccounts);

            logger.LogInformation("Synced {Count} bank accounts", bankAccounts.Count);
        }

        return true;
    }

    private async Task<List<BankAccount>?> GetBankAccounts(Guid? bankAccountId)
    {
        var accountsToFetch = new List<Guid>();
        
        if (bankAccountId.HasValue)
        {
            accountsToFetch.Add(bankAccountId.Value);
        }
        else
        {
            var authRequests = dbContext.AuthenticationRequests
                .Include(authenticationRequest => authenticationRequest.AssociatedAccounts).ToList();

            foreach (var authenticationRequest in authRequests)
            {
                if (authenticationRequest.Status == AuthenticationStatus.Active)
                {
                    var ids = authenticationRequest.AssociatedAccounts.Select(acc => acc.ThirdPartyId);
                    accountsToFetch.AddRange(ids);
                }
            }
        }

        var bankAccounts = new List<BankAccount>();
        var responses = await dataProvider.GetBankAccounts(accountsToFetch);
        foreach (var response in responses)
        {
            if (response.IsSuccessful)
            {
                bankAccounts.Add(response.Result);
            }
        }

        return bankAccounts;
    }

    private async Task AddOrUpdateBankAccounts(List<BankAccount> bankAccounts)
    {
        foreach (var updatedBankAccount in bankAccounts)
        {
            var existingBankAccount = dbContext.BankAccounts
                .Include(account => account.AssociatedAuthenticationRequests)
                .Include(account => account.AssociatedInstitution)
                .SingleOrDefault(account => account.ThirdPartyId == updatedBankAccount.ThirdPartyId);

            if (existingBankAccount == null)
            {
                var insertEntity = BankAccount.CreateWithoutNavigationProperties(updatedBankAccount);
                var entry = await dbContext.BankAccounts.AddAsync(insertEntity);
                existingBankAccount = entry.Entity;
            }
            else
            {
                existingBankAccount.UpdateProperties(updatedBankAccount);
            }
            
            await UpdateAssociatedAuthenticationRequests(updatedBankAccount, existingBankAccount);
            await UpdateAssociatedInstitution(updatedBankAccount, existingBankAccount);
            await dbContext.SaveChangesAsync();
        }
    }

    private async Task UpdateAssociatedAuthenticationRequests(BankAccount updatedBankAccount,
        BankAccount existingBankAccount)
    {
        var associatedAuthenticationRequestIds = updatedBankAccount.AssociatedAuthenticationRequests
            .Select(req => req.ThirdPartyId);
        var authRequests = dbContext.AuthenticationRequests
            .Where(request => associatedAuthenticationRequestIds.Contains(request.ThirdPartyId)).ToList();
        
        foreach (var request in updatedBankAccount.AssociatedAuthenticationRequests)
        {
            var linkedRequest = authRequests.SingleOrDefault(existingRequest =>
                existingRequest.ThirdPartyId == request.ThirdPartyId);
           
            if (linkedRequest == null)
            {
                var entry = await dbContext.AuthenticationRequests.AddAsync(request);
                linkedRequest = entry.Entity;
                existingBankAccount.AssociatedAuthenticationRequests.Add(request);
            }

            var associationAlreadyExists = existingBankAccount.AssociatedAuthenticationRequests
                .All(req => req.ThirdPartyId != linkedRequest.ThirdPartyId);
            if (associationAlreadyExists)
            {
                existingBankAccount.AssociatedAuthenticationRequests.Add(linkedRequest);
            }
        }
    }
    
    private async Task UpdateAssociatedInstitution(BankAccount updatedBankAccount,
        BankAccount existingBankAccount)
    {
        var associatedInstitutionId = updatedBankAccount.AssociatedInstitution?.ThirdPartyId;

        if (associatedInstitutionId != null)
        {
            var trackedInstitution = dbContext.BankingInstitutions
                .SingleOrDefault(institution => institution.ThirdPartyId == associatedInstitutionId);
        
            if (trackedInstitution == null && updatedBankAccount.AssociatedInstitution != null)
            {
                var entry = await dbContext.BankingInstitutions.AddAsync(updatedBankAccount.AssociatedInstitution);
                trackedInstitution = entry.Entity;
            }
            
            var associationDoesNotExistYet =
                existingBankAccount.AssociatedInstitution?.ThirdPartyId != trackedInstitution?.ThirdPartyId;
            if (associationDoesNotExistYet)
            {
                existingBankAccount.AssociatedInstitution = trackedInstitution;
            }
        }
    }
}