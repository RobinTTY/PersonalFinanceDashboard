using Microsoft.EntityFrameworkCore;
using RobinTTY.PersonalFinanceDashboard.Core.Models;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Extensions;

/// <summary>
/// Extensions on the <see cref="ApplicationDbContext"/> type.
/// </summary>
public static class ApplicationDbContextExtensions
{
    /// <summary>
    /// Adds the provided authentication request to the database if it doesn't exist yet.
    /// If it already exists, the existing entity will be updated.
    /// Does not execute a <see cref="DbContext.SaveChanges()"/>.
    /// </summary>
    /// <param name="context">The database context to use.</param>
    /// <param name="authRequest">The authentication request to persist in the database.</param>
    /// <returns>The type of operation that was performed.</returns>
    public static async Task<OperationType> AddOrUpdateAuthenticationRequest(this ApplicationDbContext context,
        AuthenticationRequest authRequest)
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

    // TODO: this can be made generic?!
    // accept models that implement interface which updates all properties
    // accept dbset that should be updated
    public static async Task<OperationType> AddOrUpdateBankingInstitution(this ApplicationDbContext context,
        BankingInstitution bankingInstitution)
    {
        var existingBankingInstitution = context.BankingInstitutions
            .SingleOrDefault(institution => institution.ThirdPartyId == bankingInstitution.ThirdPartyId);

        if (existingBankingInstitution != null)
        {
            existingBankingInstitution.Name = bankingInstitution.Name;
            existingBankingInstitution.Bic = bankingInstitution.Bic;
            existingBankingInstitution.LogoUri = bankingInstitution.LogoUri;
            existingBankingInstitution.Countries = bankingInstitution.Countries;
            return OperationType.Update;
        }

        await context.BankingInstitutions.AddAsync(bankingInstitution);
        return OperationType.Insert;
    }

    public static async Task<OperationType> AddOrUpdateBankAccount(this ApplicationDbContext context,
        BankAccount bankAccount)
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

    public enum OperationType
    {
        Undefined,
        Insert,
        Update
    }
}