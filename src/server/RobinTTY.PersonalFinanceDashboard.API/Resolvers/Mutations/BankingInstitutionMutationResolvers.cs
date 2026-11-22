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
    /// <param name="cancellationToken">A token to signal cancellation.</param>
    public async Task<BankingInstitution> CreateBankingInstitution(BankingInstitutionRepository repository,
        CreateBankingInstitutionInput input, CancellationToken cancellationToken)
    {
        var bankingInstitution = new BankingInstitution(DataSource.Manual, null, input.Bic, input.Name, input.LogoUri,
            input.Countries);
        return await repository.AddBankingInstitution(bankingInstitution, cancellationToken);
    }

    /// <summary>
    /// Update an existing banking institution.
    /// </summary>
    /// <param name="repository">The injected repository to use for data retrieval.</param>
    /// <param name="input">The banking institution values to update.</param>
    /// <param name="cancellationToken">A token to signal cancellation.</param>
    public async Task<BankingInstitution> UpdateBankingInstitution(BankingInstitutionRepository repository,
        UpdateBankingInstitutionInput input, CancellationToken cancellationToken)
    {
        return await repository.UpdateBankingInstitution(input.Id, input.Name, input.Bic, input.LogoUri,
                   input.Countries, cancellationToken)
               ?? throw MutationErrors.NotFound<BankingInstitution>(input.Id, ErrorCodes.BankingInstitutionNotFound);
    }

    /// <summary>
    /// Delete an existing banking institution.
    /// </summary>
    /// <param name="repository">The injected repository to use for data retrieval.</param>
    /// <param name="bankingInstitutionId">The id of the banking institution to delete.</param>
    /// <param name="cancellationToken">A token to signal cancellation.</param>
    public async Task<DeleteResponse> DeleteBankingInstitution(BankingInstitutionRepository repository,
        Guid bankingInstitutionId, CancellationToken cancellationToken)
    {
        var deleted = await repository.DeleteBankingInstitution(bankingInstitutionId, cancellationToken);
        if (deleted)
            return new DeleteResponse { DeletedId = bankingInstitutionId };

        throw MutationErrors.NotFound<BankingInstitution>(bankingInstitutionId,
            ErrorCodes.BankingInstitutionNotFound);
    }

    /// <summary>
    /// Synchronizes banking institution data with third-party data providers.
    /// </summary>
    /// <param name="synchronizer">Runs the synchronization.</param>
    /// <param name="cancellationToken">A token to signal cancellation.</param>
    /// <returns>A boolean value indicating whether the synchronization was successful.</returns>
    public async Task<bool> SynchronizeBankingInstitutionData(IDataSynchronizer synchronizer,
        CancellationToken cancellationToken)
    {
        return await synchronizer.Synchronize(ThirdPartyDataType.BankingInstitutions, forceThirdPartySync: true,
            cancellationToken);
    }
}
