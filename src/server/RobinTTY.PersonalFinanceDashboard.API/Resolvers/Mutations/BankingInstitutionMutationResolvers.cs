using HotChocolate.Types;
using RobinTTY.PersonalFinanceDashboard.Api.Models;
using RobinTTY.PersonalFinanceDashboard.Api.Resolvers.Inputs;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Repositories;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Services.DataSynchronization.Interfaces;

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
    /// <param name="input">The banking institution to create.</param>
    public async Task<BankingInstitution> CreateBankingInstitution(BankingInstitutionRepository repository,
        CreateBankingInstitutionInput input)
    {
        // Manually created institutions have no third party id
        var bankingInstitution =
            new BankingInstitution(string.Empty, input.Bic, input.Name, input.LogoUri, input.Countries);
        return await repository.AddBankingInstitution(bankingInstitution);
    }

    /// <summary>
    /// Update an existing banking institution.
    /// </summary>
    /// <param name="repository">The injected repository to use for data retrieval.</param>
    /// <param name="input">The banking institution values to update.</param>
    public async Task<BankingInstitution> UpdateBankingInstitution(BankingInstitutionRepository repository,
        UpdateBankingInstitutionInput input)
    {
        return await repository.UpdateBankingInstitution(input.Id, input.Name, input.Bic, input.LogoUri,
                   input.Countries)
               ?? throw MutationErrors.NotFound<BankingInstitution>(input.Id, ErrorCodes.BankingInstitutionNotFound);
    }

    /// <summary>
    /// Delete an existing banking institution.
    /// </summary>
    /// <param name="repository">The injected repository to use for data retrieval.</param>
    /// <param name="bankingInstitutionId">The id of the banking institution to delete.</param>
    public async Task<DeleteResponse> DeleteBankingInstitution(BankingInstitutionRepository repository,
        Guid bankingInstitutionId)
    {
        var deleted = await repository.DeleteBankingInstitution(bankingInstitutionId);
        if (deleted)
            return new DeleteResponse { DeletedId = bankingInstitutionId };

        throw MutationErrors.NotFound<BankingInstitution>(bankingInstitutionId,
            ErrorCodes.BankingInstitutionNotFound);
    }

    /// <summary>
    /// Synchronizes banking institution data with third-party data providers.
    /// </summary>
    /// <param name="syncHandler">The handler responsible for managing the data synchronization process.</param>
    /// <returns>A boolean value indicating whether the synchronization was successful.</returns>
    public async Task<bool> SynchronizeBankingInstitutionData(IBankingInstitutionSyncHandler syncHandler)
    {
        return await syncHandler.SynchronizeData(forceThirdPartySync: true);
    }
}