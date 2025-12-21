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
        /// Replaces all existing banking institutions in the database with the provided list of banking institutions.
        /// </summary>
        /// <param name="bankingInstitutions">A list of banking institutions to be saved in the database, replacing the existing entries.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task ReplaceBankingInstitutions(List<BankingInstitution> bankingInstitutions)
        {
            await context.BankingInstitutions.ExecuteDeleteAsync();
            await context.BankingInstitutions.AddRangeAsync(bankingInstitutions);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Adds new authentication requests to the database or updates existing ones based on their third-party IDs.
        /// </summary>
        /// <param name="authenticationRequests">A list of authentication requests to add or update in the database.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task AddOrUpdateAuthenticationRequests(params List<AuthenticationRequest> authenticationRequests)
        {
            foreach (var authenticationRequest in authenticationRequests)
            {
                var existingRequest = context.AuthenticationRequests
                    .SingleOrDefault(req => req.ThirdPartyId == authenticationRequest.ThirdPartyId);

                if (existingRequest == null)
                {
                    await context.AuthenticationRequests.AddAsync(authenticationRequest);
                    await context.SaveChangesAsync();
                    return;
                }

                existingRequest.Status = authenticationRequest.Status;
                existingRequest.AuthenticationLink = authenticationRequest.AuthenticationLink;

                var associatedAccountIds = authenticationRequest.AssociatedAccounts
                    .Select(acc => acc.ThirdPartyId).Distinct();
                var accounts = context.BankAccounts
                    .Where(account => associatedAccountIds.Contains(account.ThirdPartyId)).ToList();

                authenticationRequest.AssociatedAccounts.ForEach(account =>
                {
                    var linkedAccount = accounts.SingleOrDefault(existingAccount =>
                        existingAccount.ThirdPartyId == account.ThirdPartyId);

                    linkedAccount?.UpdateProperties(account);

                    if (linkedAccount == null)
                    {
                        existingRequest.AssociatedAccounts.Add(account);
                    }
                });

                await context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Removes all authentication requests from the database that are not included in the provided collection
        /// of authentication requests.
        /// </summary>
        /// <param name="authenticationRequests">The authentication requests to compare the ones stored in the
        /// database to.</param>
        public async Task RemoveNotIncludedAuthenticationRequests(List<AuthenticationRequest> authenticationRequests)
        {
            var thirdPartyIds = authenticationRequests.Select(req => req.ThirdPartyId);

            await context.AuthenticationRequests
                .Where(req => !thirdPartyIds.Contains(req.ThirdPartyId))
                .ExecuteDeleteAsync();
        }
    }
}