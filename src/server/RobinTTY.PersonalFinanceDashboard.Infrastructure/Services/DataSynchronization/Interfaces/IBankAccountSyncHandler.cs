namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Services.DataSynchronization.Interfaces;

/// <summary>
/// Synchronizes the details and balances of bank accounts with the data providers they originate from.
/// </summary>
public interface IBankAccountSyncHandler
{
    /// <summary>
    /// Retrieves the details and balances of the bank accounts that are due for retrieval and updates the database.
    /// Failed retrievals are recorded per account and retried later.
    /// </summary>
    /// <param name="internalBankAccountId">The id of a single bank account to retrieve, <see langword="null"/> for all
    /// accounts of authentication requests that currently grant access.</param>
    /// <param name="forceThirdPartySync">Whether to retrieve the data even if it isn't due.</param>
    /// <param name="cancellationToken">A token to signal cancellation.</param>
    /// <returns><see langword="false"/> if the given bank account doesn't exist, <see langword="true"/> otherwise.</returns>
    Task<bool> SynchronizeData(Guid? internalBankAccountId = null, bool forceThirdPartySync = false,
        CancellationToken cancellationToken = default);
}
