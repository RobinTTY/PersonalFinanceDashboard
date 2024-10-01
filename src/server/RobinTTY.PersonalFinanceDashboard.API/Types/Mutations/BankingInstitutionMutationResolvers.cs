using HotChocolate.Types;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Repositories;

namespace RobinTTY.PersonalFinanceDashboard.Api.Types.Mutations;

/// <summary>
/// <see cref="BankingInstitution"/> related mutation resolvers.
/// </summary>
[MutationType]
public class BankingInstitutionMutationResolvers
{
    public async Task<BankingInstitution> CreateBankingInstitution(BankingInstitutionRepository repository,
        BankingInstitution bankingInstitution)
    {
        return await repository.AddBankingInstitution(bankingInstitution);
    }

    /// <summary>
    /// TODO: Return something more than bool? Generalized response across all resolvers?
    /// </summary>
    /// <param name="repository"></param>
    /// <param name="bankingInstitutionId"></param>
    /// <returns></returns>
    public async Task<bool> DeleteBankingInstitution(BankingInstitutionRepository repository,
        string bankingInstitutionId)
    {
        return await repository.DeleteBankingInstitution(bankingInstitutionId);
    }
}
