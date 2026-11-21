using RobinTTY.PersonalFinanceDashboard.Core.Models;

namespace RobinTTY.PersonalFinanceDashboard.Core.DataProviders;

/// <summary>
/// A third party service providing banking data, e.g. an open banking aggregator. All ids passed to and returned
/// by a provider are its own ids (<see cref="Models.Base.SourcedEntity.ExternalId"/>), and returned entities have
/// their <see cref="Models.Base.SourcedEntity.Source"/> set to <see cref="Source"/>.
/// </summary>
public interface IBankDataProvider
{
    /// <summary>
    /// The data source the provider's entities originate from.
    /// </summary>
    DataSource Source { get; }

    /// <summary>
    /// Gets a banking institution.
    /// </summary>
    /// <param name="institutionId">The provider's id of the institution.</param>
    /// <param name="cancellationToken">A token to signal cancellation.</param>
    Task<ThirdPartyResponse<BankingInstitution>> GetBankingInstitution(string institutionId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the banking institutions available through the provider.
    /// </summary>
    /// <param name="country">Optional ISO 3166 country code to limit the institutions to.</param>
    /// <param name="cancellationToken">A token to signal cancellation.</param>
    Task<ThirdPartyResponse<IReadOnlyList<BankingInstitution>>> GetBankingInstitutions(string? country = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an authentication request, including the ids of its associated accounts.
    /// </summary>
    /// <param name="authenticationRequestId">The provider's id of the authentication request.</param>
    /// <param name="cancellationToken">A token to signal cancellation.</param>
    Task<ThirdPartyResponse<AuthenticationRequest>> GetAuthenticationRequest(string authenticationRequestId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all authentication requests, including the ids of their associated accounts.
    /// </summary>
    /// <param name="cancellationToken">A token to signal cancellation.</param>
    Task<ThirdPartyResponse<IReadOnlyList<AuthenticationRequest>>> GetAuthenticationRequests(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates an authentication request, which the user completes by following its authentication link.
    /// </summary>
    /// <param name="institutionId">The provider's id of the institution to authenticate with.</param>
    /// <param name="redirectUri">The URI the user is redirected to once the authentication is completed.</param>
    /// <param name="cancellationToken">A token to signal cancellation.</param>
    Task<ThirdPartyResponse<AuthenticationRequest>> CreateAuthenticationRequest(string institutionId,
        Uri redirectUri, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an authentication request, revoking the access to its accounts.
    /// </summary>
    /// <param name="authenticationRequestId">The provider's id of the authentication request.</param>
    /// <param name="cancellationToken">A token to signal cancellation.</param>
    /// <returns>The error if the deletion failed, otherwise <see langword="null"/>.</returns>
    Task<ThirdPartyError?> DeleteAuthenticationRequest(string authenticationRequestId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets bank accounts including their current balance.
    /// </summary>
    /// <param name="accounts">The provider's ids of the accounts, mapped to whether details that rarely change
    /// (e.g. name, currency, IBAN and institution) should be retrieved as well.</param>
    /// <param name="cancellationToken">A token to signal cancellation.</param>
    /// <returns>A response for each requested account, keyed by the provider's id of the account.</returns>
    Task<IReadOnlyDictionary<string, ThirdPartyResponse<BankAccount>>> GetBankAccounts(
        IReadOnlyDictionary<string, bool> accounts,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the transactions of a bank account. The returned transactions reference a <see cref="BankAccount"/>
    /// that only carries the account's source and external id.
    /// </summary>
    /// <param name="accountId">The provider's id of the account.</param>
    /// <param name="dateFrom">The date of the earliest transactions to get, <see langword="null"/> to get all the
    /// provider has.</param>
    /// <param name="cancellationToken">A token to signal cancellation.</param>
    Task<ThirdPartyResponse<IReadOnlyList<Transaction>>> GetTransactions(string accountId, DateTime? dateFrom = null,
        CancellationToken cancellationToken = default);
}