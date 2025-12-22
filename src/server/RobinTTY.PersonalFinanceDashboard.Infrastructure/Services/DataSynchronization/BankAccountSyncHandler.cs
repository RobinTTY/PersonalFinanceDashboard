using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Interfaces;
using RobinTTY.PersonalFinanceDashboard.ThirdPartyDataProviders;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Services.DataSynchronization;

public class BankAccountSyncHandler(
    ApplicationDbContext dbContext,
    GoCardlessDataProviderService dataProvider,
    ThirdPartyDataRetrievalMetadataService dataRetrievalMetadataService,
    ILogger<BankAccountSyncHandler> logger) : IDataSyncHandler
{
    /// <inheritdoc />
    public async Task<bool> SynchronizeData(bool forceThirdPartySync = false)
    {
        var dataIsStale = await dataRetrievalMetadataService.DataIsStale(ThirdPartyDataType.BankAccounts);
        if (dataIsStale || forceThirdPartySync)
        {
            var bankAccounts = await GetBankAccounts();
            if (bankAccounts.Count == 0)
            {
                return false;
            }

            await AddOrUpdateBankAccounts(bankAccounts);
            await dataRetrievalMetadataService.ResetDataExpiry(ThirdPartyDataType.BankAccounts);

            logger.LogInformation("Synced {Count} bank accounts", bankAccounts.Count);
        }

        return true;
    }

    private async Task<List<BankAccount>> GetBankAccounts()
    {
        var authRequests = dbContext.AuthenticationRequests
            .Include(authenticationRequest => authenticationRequest.AssociatedAccounts).ToList();

        var activeLinkedAccountIds = new List<Guid>();
        foreach (var authenticationRequest in authRequests)
        {
            if (authenticationRequest.Status == AuthenticationStatus.Active)
            {
                var ids = authenticationRequest.AssociatedAccounts.Select(acc => acc.ThirdPartyId);
                activeLinkedAccountIds.AddRange(ids);
            }
        }

        var bankAccounts = new List<BankAccount>();
        var responses = await dataProvider.GetBankAccounts(activeLinkedAccountIds);
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
                await dbContext.BankAccounts.AddAsync(updatedBankAccount);
                await dbContext.SaveChangesAsync();
                return;
            }

            existingBankAccount.UpdateProperties(updatedBankAccount);
            UpdateAssociatedAuthenticationRequests(updatedBankAccount, existingBankAccount);
            UpdateAssociatedInstitution(updatedBankAccount, existingBankAccount);

            await dbContext.SaveChangesAsync();
        }
    }

    private void UpdateAssociatedAuthenticationRequests(BankAccount updatedBankAccount,
        BankAccount existingBankAccount)
    {
        var associatedAuthenticationRequestIds = updatedBankAccount.AssociatedAuthenticationRequests
            .Select(req => req.ThirdPartyId);
        var authRequests = dbContext.AuthenticationRequests
            .Where(request => associatedAuthenticationRequestIds.Contains(request.ThirdPartyId)).ToList();
        
        updatedBankAccount.AssociatedAuthenticationRequests.ForEach(request =>
        {
            var linkedRequest = authRequests.SingleOrDefault(existingRequest =>
                existingRequest.ThirdPartyId == request.ThirdPartyId);
           
            if (linkedRequest == null)
            {
                existingBankAccount.AssociatedAuthenticationRequests.Add(request);
            }
        });
    }
    
    private void UpdateAssociatedInstitution(BankAccount updatedBankAccount,
        BankAccount existingBankAccount)
    {
        var associatedInstitutionId = updatedBankAccount.AssociatedInstitution?.ThirdPartyId;

        if (associatedInstitutionId != null)
        {
            var linkedInstitution = dbContext.BankingInstitutions
                .SingleOrDefault(institution => institution.ThirdPartyId == associatedInstitutionId);
        
            if (linkedInstitution == null)
            {
                existingBankAccount.AssociatedInstitution = linkedInstitution;
            }
        }
    }
}