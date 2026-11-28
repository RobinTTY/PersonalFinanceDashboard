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
    /// <param name="cancellationToken">A token to signal cancellation.</param>
    public async Task<BankAccount> CreateBankAccount(BankAccountRepository repository, CreateBankAccountInput input,
        CancellationToken cancellationToken)
    {
        var bankAccount = new BankAccount(DataSource.Manual, null, input.Name, input.Description, input.Balance,
            input.Currency, input.Iban, input.Bic, input.Bban, input.OwnerName, input.AccountType);
        return await repository.AddBankAccount(bankAccount, cancellationToken);
    }

    /// <summary>
    /// Update the name and description of an existing banking account.
    /// </summary>
    /// <param name="repository">The injected repository to use for data retrieval.</param>
    /// <param name="input">The banking account values to update.</param>
    /// <param name="cancellationToken">A token to signal cancellation.</param>
    public async Task<BankAccount> UpdateBankAccount(BankAccountRepository repository, UpdateBankAccountInput input,
        CancellationToken cancellationToken)
    {
        return await repository.UpdateBankAccount(input.Id, input.Name, input.Description, cancellationToken)
               ?? throw MutationErrors.NotFound<BankAccount>(input.Id, ErrorCodes.BankAccountNotFound);
    }

    /// <summary>
    /// Sets whether a bank account is included in analytics such as graphs and account views.
    /// </summary>
    /// <param name="repository">The injected repository to use for data retrieval.</param>
    /// <param name="bankAccountId">The id of the bank account to update.</param>
    /// <param name="includeInAnalytics">Whether the account should be included in analytics.</param>
    /// <param name="cancellationToken">A token to signal cancellation.</param>
    public async Task<BankAccount> SetBankAccountIncludeInAnalytics(BankAccountRepository repository,
        Guid bankAccountId, bool includeInAnalytics, CancellationToken cancellationToken)
    {
        return await repository.SetIncludeInAnalytics(bankAccountId, includeInAnalytics, cancellationToken)
               ?? throw MutationErrors.NotFound<BankAccount>(bankAccountId, ErrorCodes.BankAccountNotFound);
    }

    /// <summary>
    /// Delete an existing bank account and its transactions. A synchronized account can only be deleted once its
    /// bank is disconnected or its access expired, since the synchronization would recreate it.
    /// </summary>
    /// <param name="repository">The injected repository to use for data retrieval.</param>
    /// <param name="bankAccountId">The id of the bank account to delete.</param>
    /// <param name="cancellationToken">A token to signal cancellation.</param>
    public async Task<DeleteResponse> DeleteBankAccount(BankAccountRepository repository,
        Guid bankAccountId, CancellationToken cancellationToken)
    {
        return await repository.DeleteBankAccount(bankAccountId, cancellationToken) switch
        {
            DeleteResult.Deleted => new DeleteResponse { DeletedId = bankAccountId },
            DeleteResult.Synchronized => throw MutationErrors.Create(
                $"Bank account with id {bankAccountId} is still synchronized. Disconnect its bank first.",
                ErrorCodes.BankAccountSynchronized),
            _ => throw MutationErrors.NotFound<BankAccount>(bankAccountId, ErrorCodes.BankAccountNotFound)
        };
    }

    /// <summary>
    /// Synchronizes bank account data with third-party data providers.
    /// </summary>
    /// <param name="synchronizer">Runs the synchronization.</param>
    /// <param name="cancellationToken">A token to signal cancellation.</param>
    /// <returns>A boolean value indicating whether the synchronization was successful.</returns>
    public async Task<bool> SynchronizeBankAccountData(IDataSynchronizer synchronizer,
        CancellationToken cancellationToken)
    {
        return await synchronizer.Synchronize(ThirdPartyDataType.BankAccounts, forceThirdPartySync: true,
            cancellationToken);
    }
}
