using HotChocolate.Types;
using RobinTTY.PersonalFinanceDashboard.Api.Models;
using RobinTTY.PersonalFinanceDashboard.Api.Resolvers.Inputs;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Repositories;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Services.DataSynchronization.Interfaces;

namespace RobinTTY.PersonalFinanceDashboard.Api.Resolvers.Mutations;

/// <summary>
/// <see cref="BankAccount"/> related mutation resolvers.
/// </summary>
[MutationType]
public class BankAccountMutationResolvers
{
    /// <summary>
    /// Create a new banking account.
    /// </summary>
    /// <param name="repository">The injected repository to use for data retrieval.</param>
    /// <param name="input">The banking account to create.</param>
    public async Task<BankAccount> CreateBankAccount(BankAccountRepository repository, CreateBankAccountInput input)
    {
        // Manually created accounts have no third party id
        var bankAccount = new BankAccount(Guid.Empty, input.Name, input.Description, input.Balance, input.Currency,
            input.Iban, input.Bic, input.Bban, input.OwnerName, input.AccountType);
        return await repository.AddBankAccount(bankAccount);
    }

    /// <summary>
    /// Update the name and description of an existing banking account.
    /// </summary>
    /// <param name="repository">The injected repository to use for data retrieval.</param>
    /// <param name="input">The banking account values to update.</param>
    public async Task<BankAccount> UpdateBankAccount(BankAccountRepository repository, UpdateBankAccountInput input)
    {
        return await repository.UpdateBankAccount(input.Id, input.Name, input.Description)
               ?? throw MutationErrors.NotFound<BankAccount>(input.Id, ErrorCodes.BankAccountNotFound);
    }

    /// <summary>
    /// Sets whether a bank account is included in analytics such as graphs and account views.
    /// </summary>
    /// <param name="repository">The injected repository to use for data retrieval.</param>
    /// <param name="bankAccountId">The id of the bank account to update.</param>
    /// <param name="includeInAnalytics">Whether the account should be included in analytics.</param>
    /// <returns>The updated bank account if one is matched, otherwise <see langword="null"/>.</returns>
    public async Task<BankAccount?> SetBankAccountIncludeInAnalytics(BankAccountRepository repository,
        Guid bankAccountId, bool includeInAnalytics)
    {
        return await repository.SetIncludeInAnalytics(bankAccountId, includeInAnalytics);
    }

    /// <summary>
    /// Delete an existing bank account.
    /// </summary>
    /// <param name="repository">The injected repository to use for data retrieval.</param>
    /// <param name="bankAccountId">The id of the bank account to delete.</param>
    public async Task<DeleteResponse> DeleteBankAccount(BankAccountRepository repository,
        Guid bankAccountId)
    {
        var deleted = await repository.DeleteBankAccount(bankAccountId);
        if (deleted)
            return new DeleteResponse { DeletedId = bankAccountId };

        throw MutationErrors.NotFound<BankAccount>(bankAccountId, ErrorCodes.BankAccountNotFound);
    }

    /// <summary>
    /// Synchronizes bank account data with third-party data providers.
    /// </summary>
    /// <param name="syncHandler">The handler responsible for managing the data synchronization process.</param>
    /// <returns>A boolean value indicating whether the synchronization was successful.</returns>
    public async Task<bool> SynchronizeBankAccountData(IBankAccountSyncHandler syncHandler)
    {
        return await syncHandler.SynchronizeData(forceThirdPartySync: true);
    }
}