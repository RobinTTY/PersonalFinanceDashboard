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
        public async Task ReplaceBankingInstitutions(List<BankingInstitution> bankingInstitutions)
        {
            await context.BankingInstitutions.ExecuteDeleteAsync();
            await context.BankingInstitutions.AddRangeAsync(bankingInstitutions);
            await context.SaveChangesAsync();
        }

        public async Task AddOrUpdateAuthenticationRequests(params List<AuthenticationRequest> authenticationRequests)
        {
            foreach (var authenticationRequest in authenticationRequests)
            {
                var existingRequest = context.AuthenticationRequests
                    .Include(req => req.AssociatedAccounts)
                    .SingleOrDefault(req => req.ThirdPartyId == authenticationRequest.ThirdPartyId);

                if (existingRequest == null)
                {
                    await context.AuthenticationRequests.AddAsync(authenticationRequest);
                    await context.SaveChangesAsync();
                    return;
                }

                existingRequest.Status = authenticationRequest.Status;
                existingRequest.AuthenticationLink = authenticationRequest.AuthenticationLink;

                authenticationRequest.AssociatedAccounts.ForEach(account =>
                {
                    var linkedAccount = existingRequest.AssociatedAccounts
                        .SingleOrDefault(existingAccount => existingAccount.ThirdPartyId == account.ThirdPartyId);

                    linkedAccount?.UpdateProperties(account);

                    if (linkedAccount == null)
                    {
                        existingRequest.AssociatedAccounts.Add(account);
                    }
                });

                await context.SaveChangesAsync();
            }
        }
    }
}