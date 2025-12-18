using Microsoft.EntityFrameworkCore;
using RobinTTY.PersonalFinanceDashboard.Core.Models;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Extensions;

/// <summary>
/// Extensions on the <see cref="ApplicationDbContext"/> type.
/// </summary>
public static class ApplicationDbContextExtensions
{
    /// <param name="context">The database context to use.</param>
    extension(ApplicationDbContext context)
    {
        /// <summary>
        /// Adds the provided authentication request to the database if it doesn't exist yet.
        /// If it already exists, the existing entity will be updated.
        /// Does not execute a <see cref="DbContext.SaveChanges()"/>.
        /// </summary>
        /// <param name="authRequest">The authentication request to persist in the database.</param>
        /// <returns>The type of operation that was performed.</returns>
        public async Task<OperationType> AddOrUpdateAuthenticationRequest(AuthenticationRequest authRequest)
        {
            var existingAuthRequest = context.AuthenticationRequests
                .SingleOrDefault(authReq => authReq.ThirdPartyId == authRequest.ThirdPartyId);

            if (existingAuthRequest != null)
            {
                existingAuthRequest.Status = authRequest.Status;
                existingAuthRequest.ThirdPartyId = authRequest.ThirdPartyId;
                existingAuthRequest.AuthenticationLink = authRequest.AuthenticationLink;
                return OperationType.Update;
            }

            await context.AuthenticationRequests.AddAsync(authRequest);
            return OperationType.Insert;
        }

        public async Task ReplaceBankingInstitutions(List<BankingInstitution> bankingInstitutions)
        {
            await context.BankingInstitutions.ExecuteDeleteAsync();
            await context.BankingInstitutions.AddRangeAsync(bankingInstitutions);
        }

        public async Task<OperationType> AddOrUpdateBankAccount(BankAccount bankAccount)
        {
            var existingBankAccount = context.BankAccounts
                .SingleOrDefault(account => account.ThirdPartyId == bankAccount.ThirdPartyId);

            if (existingBankAccount != null)
            {
                existingBankAccount.Name = bankAccount.Name;
                existingBankAccount.Iban = bankAccount.Iban;
                existingBankAccount.Bic = bankAccount.Bic;
                existingBankAccount.Bban = bankAccount.Bban;
                existingBankAccount.Balance = bankAccount.Balance;
                existingBankAccount.Currency = bankAccount.Currency;
                existingBankAccount.OwnerName = bankAccount.OwnerName;
                existingBankAccount.AccountType = bankAccount.AccountType;
                existingBankAccount.Description = bankAccount.Description;
                existingBankAccount.AssociatedInstitution = bankAccount.AssociatedInstitution;
            
                return OperationType.Update;
            }

            await context.BankAccounts.AddAsync(bankAccount);
            return OperationType.Insert;
        }
    }

    // TODO: For now only support institutions from third party data provider, no custom institutions, replace whole list on sync to speed up the process
    // accept models that implement interface which updates all properties
    // accept dbset that should be updated

    public enum OperationType
    {
        Undefined,
        Insert,
        Update
    }
}