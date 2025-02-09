using HotChocolate.Types;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Repositories;

namespace RobinTTY.PersonalFinanceDashboard.Api.Resolvers.Mutations;

/// <summary>
/// <see cref="BankingInstitution"/> related mutation resolvers.
/// </summary>
[MutationType]
public class BankingInstitutionMutationResolvers
{
    /// <summary>
    /// Create a new banking institution.
    /// </summary>
    /// <param name="repository">The injected repository to use for data retrieval.</param>
    /// <param name="bankingInstitution">The banking institution to create.</param>
    public async Task<BankingInstitution> CreateBankingInstitution(BankingInstitutionRepository repository,
        BankingInstitution bankingInstitution)
    {
        return await repository.AddBankingInstitution(bankingInstitution);
    }

    /// <summary>
    /// Update an existing banking institution.
    /// </summary>
    /// <param name="repository">The injected repository to use for data retrieval.</param>
    /// <param name="bankingInstitution">The banking institution to update.</param>
    public async Task<BankingInstitution> UpdateBankingInstitution(BankingInstitutionRepository repository,
        BankingInstitution bankingInstitution)
    {
        return await repository.UpdateBankingInstitution(bankingInstitution);
    }

    // TODO: Return something more than bool? Generalized response across all resolvers?
    /// <summary>
    /// Delete an existing banking institution.
    /// </summary>
    /// <param name="repository">The injected repository to use for data retrieval.</param>
    /// <param name="bankingInstitutionId">The id of the banking institution to delete.</param>
    public async Task<bool> DeleteBankingInstitution(BankingInstitutionRepository repository,
        Guid bankingInstitutionId)
    {
        return await repository.DeleteBankingInstitution(bankingInstitutionId);
    }
}
