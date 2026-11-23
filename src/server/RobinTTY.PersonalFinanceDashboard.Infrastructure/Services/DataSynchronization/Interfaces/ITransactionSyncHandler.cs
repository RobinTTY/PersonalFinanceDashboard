namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Services.DataSynchronization.Interfaces;

/// <summary>
/// Synchronizes the transactions of bank accounts with the data providers they originate from.
/// </summary>
public interface ITransactionSyncHandler
{
    /// <summary>
    /// Retrieves the transactions of the accounts of authentication requests that currently grant access, if they
    /// are due for retrieval, and updates the database. Failed retrievals are recorded per account and retried
    /// later.
    /// </summary>
    /// <param name="forceThirdPartySync">Whether to retrieve the data even if it isn't due.</param>
    /// <param name="cancellationToken">A token to signal cancellation.</param>
    /// <returns>Always <see langword="true"/>, as failures are handled per account.</returns>
    Task<bool> SynchronizeData(bool forceThirdPartySync = false, CancellationToken cancellationToken = default);
}
