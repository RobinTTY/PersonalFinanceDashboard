using Microsoft.Extensions.Logging;
using RobinTTY.NordigenApiClient;
using RobinTTY.NordigenApiClient.Models.Errors;
using RobinTTY.NordigenApiClient.Models.Responses;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using BankAccount = RobinTTY.PersonalFinanceDashboard.Core.Models.BankAccount;
using Transaction = RobinTTY.PersonalFinanceDashboard.Core.Models.Transaction;

namespace RobinTTY.PersonalFinanceDashboard.ThirdPartyDataProviders;

/// <summary>
/// Data provider for banking data from <see href="https://gocardless.com/bank-account-data/"/>.
/// </summary>
// TODO: Probably should already return a generic ThirdPartyError (type) here
// TODO: These methods need to be reworked quite a bit
public class GoCardlessDataProviderService(NordigenClient client, ILogger<GoCardlessDataProviderService> logger)
{
    public async Task<ThirdPartyResponse<BankingInstitution, BasicResponse>> GetBankingInstitution(string institutionId,
        CancellationToken cancellationToken = default)
    {
        var response = await client.InstitutionsEndpoint.GetInstitution(institutionId, cancellationToken);
        return response.ToThirdPartyResponse(institution => institution.ToBankingInstitution());
    }

    /// <summary>
    /// Gets the banking institutions which are available via this data provider.
    /// </summary>
    /// <param name="country">Optional country filter to apply to the query.</param>
    /// <param name="cancellationToken">An optional token to signal cancellation.</param>
    /// <returns>The available banking <see cref="Institution"/>s.</returns>
    public async Task<ThirdPartyResponse<IEnumerable<BankingInstitution>, BasicResponse>> GetBankingInstitutions(
        string? country = null, CancellationToken cancellationToken = default)
    {
        var response = await client.InstitutionsEndpoint.GetInstitutions(country, cancellationToken: cancellationToken);
        return response.ToThirdPartyResponse(institutions =>
            institutions.Select(institution => institution.ToBankingInstitution()));
    }

    /// <summary>
    /// Gets an existing authentication request (requisitions).
    /// </summary>
    /// <param name="requisitionId">The id of the requisition to be retrieved.</param>
    /// <param name="cancellationToken">An optional token to signal cancellation.</param>
    /// <returns>The <see cref="AuthenticationRequest"/> which matches the institutionId, as far as it exists.</returns>
    public async Task<ThirdPartyResponse<AuthenticationRequest?, BasicResponse?>> GetAuthenticationRequest(
        Guid requisitionId, CancellationToken cancellationToken = default)
    {
        var response = await client.RequisitionsEndpoint.GetRequisition(requisitionId, cancellationToken);
        return response.ToThirdPartyResponse(requisition => requisition.ToAuthenticationRequest())!;
    }

    // TODO: Cancellation token support
    /// <summary>
    /// Gets existing authentication requests (requisitions) for all banking institutions.
    /// </summary>
    /// <param name="requisitionLimit">The maximum number of requisitions to get.</param>
    /// <param name="cancellationToken">An optional token to signal cancellation.</param>
    /// <returns>All existing <see cref="Requisition"/>s.</returns>
    public async Task<ThirdPartyResponse<IEnumerable<AuthenticationRequest>, BasicResponse?>> GetAuthenticationRequests(
        int requisitionLimit, CancellationToken cancellationToken = default)
    {
        var response = await client.RequisitionsEndpoint.GetRequisitions(requisitionLimit, 0, cancellationToken);
        // TODO: use paged responses
        return response.ToThirdPartyResponse(paged =>
            paged.Results.Select(requisition => requisition.ToAuthenticationRequest()))!;
    }

    /// <summary>
    /// Creates a new authentication request (requisition) for the chosen institution and returns it.
    /// </summary>
    /// <param name="institutionId">Id of the institution for which to create the requisition.</param>
    /// <param name="redirectUri"><see cref="Uri"/> which will be redirected too when the user completes authentication for the given requisition.</param>
    /// <param name="cancellationToken">An optional token to signal cancellation.</param>
    /// <returns>The created <see cref="Requisition"/>.</returns>
    public async Task<ThirdPartyResponse<AuthenticationRequest, CreateRequisitionError>> CreateAuthenticationRequest(
        string institutionId, Uri redirectUri, CancellationToken cancellationToken = default)
    {
        var response =
            await client.RequisitionsEndpoint.CreateRequisition(institutionId, redirectUri,
                reference: Guid.NewGuid().ToString(), cancellationToken: cancellationToken);

        return response.ToThirdPartyResponse(requisition => requisition.ToAuthenticationRequest());
    }

    public async Task<ThirdPartyResponse<BasicResponse, BasicResponse>> DeleteAuthenticationRequest(
        Guid authenticationId, CancellationToken cancellationToken = default)
    {
        var response = await client.RequisitionsEndpoint.DeleteRequisition(authenticationId, cancellationToken);
        return new ThirdPartyResponse<BasicResponse, BasicResponse>(response.IsSuccess, response.Result,
            response.Error);
    }

    /// <summary>
    /// Gets bank accounts by their ids with per-account metadata inclusion control. This includes general account information, account details, and balances.
    /// </summary>
    /// <param name="accountMetadataPreferences">A dictionary mapping account IDs to a boolean value indicating whether not to include account metadata during the fetch operation.</param>
    /// <param name="cancellationToken">An optional token to signal cancellation.</param>
    /// <returns>API responses containing the fetched bank accounts or in the case of failure, the error.</returns>
    public async Task<List<ThirdPartyResponse<BankAccount, AccountsError>>> GetBankAccounts(
        IDictionary<Guid, bool> accountMetadataPreferences, CancellationToken cancellationToken = default)
    {
        var tasks = accountMetadataPreferences.Select(kvp => GetBankAccount(kvp.Key, kvp.Value, cancellationToken))
            .ToList();
        await Task.WhenAll(tasks);

        return tasks.Select(task => task.Result).ToList();
    }

    private async Task<ThirdPartyResponse<BankAccount, AccountsError>> GetBankAccount(Guid accountId,
        bool includeMetadata, CancellationToken cancellationToken = default)
    {
        Task<NordigenApiResponse<NordigenApiClient.Models.Responses.BankAccount, BasicResponse>>?
            generalAccountInfoTask = null;
        Task<NordigenApiResponse<BankAccountDetails, AccountsError>>? accountDetailsTask = null;
        var balanceTask = client.AccountsEndpoint.GetBalances(accountId, cancellationToken);

        if (includeMetadata)
        {
            generalAccountInfoTask = client.AccountsEndpoint.GetAccount(accountId, cancellationToken);
            accountDetailsTask = client.AccountsEndpoint.GetAccountDetails(accountId, cancellationToken);
        }

        var generalAccountInfoResponse = generalAccountInfoTask != null ? await generalAccountInfoTask : null;
        var accountDetailsResponse = accountDetailsTask != null ? await accountDetailsTask : null;
        var balanceResponse = await balanceTask;

        var errors = ExtractAndLogErrors(accountId, includeMetadata, balanceResponse, generalAccountInfoResponse,
            accountDetailsResponse);
        if (errors.Count > 0)
        {
            return new ThirdPartyResponse<BankAccount, AccountsError>(false, null, new AccountsError
            {
                Summary = $"Failed to retrieve data for account {accountId}",
                Detail = string.Join("; ", errors)
            });
        }

        var generalAccountInfoResult = generalAccountInfoResponse?.Result;
        var accountDetailsResult = accountDetailsResponse?.Result;
        var balanceResult = balanceResponse.Result;
        var bankAccount = GoCardlessTypeExtensions.CreateBankAccount(accountId, generalAccountInfoResult,
            accountDetailsResult, balanceResult!);

        return new ThirdPartyResponse<BankAccount, AccountsError>(true, bankAccount, null);
    }

    private List<string> ExtractAndLogErrors(Guid accountId, bool includeMetadata,
        NordigenApiResponse<List<Balance>, AccountsError> balanceResponse,
        NordigenApiResponse<NordigenApiClient.Models.Responses.BankAccount, BasicResponse>? generalAccountInfoResponse,
        NordigenApiResponse<BankAccountDetails, AccountsError>? accountDetailsResponse)
    {
        var errorMessages = new List<string>();
        if (!balanceResponse.IsSuccess)
        {
            var error = $"Summary: {balanceResponse.Error.Summary}, Details: {balanceResponse.Error.Detail}";

            errorMessages.Add(error);
            logger.LogWarning("Failed to retrieve balances for account {accountId}: {error}", accountId, error);
        }

        if (includeMetadata)
        {
            if (generalAccountInfoResponse is { IsSuccess: false })
            {
                var error =
                    $"Summary: {generalAccountInfoResponse.Error.Summary}, Details: {generalAccountInfoResponse.Error.Detail}";

                errorMessages.Add(error);
                logger.LogWarning("Failed to retrieve general account metadata for account {accountId}: {error}",
                    accountId, error);
            }

            if (accountDetailsResponse is { IsSuccess: false })
            {
                var error =
                    $"Summary: {accountDetailsResponse.Error.Summary}, Details: {accountDetailsResponse.Error.Detail}";

                errorMessages.Add(error);
                logger.LogWarning("Failed to retrieve detailed account metadata for account {accountId}: {error}",
                    accountId, error);
            }
        }

        return errorMessages;
    }

    public async Task<ThirdPartyResponse<IEnumerable<Transaction>, AccountsError>> GetTransactions(
        Guid goCardlessAccountId, CancellationToken cancellationToken = default)
    {
        var response =
            await client.AccountsEndpoint.GetTransactions(goCardlessAccountId, cancellationToken: cancellationToken);

        // TODO: Also return pending transactions
        return response.ToThirdPartyResponse(result => result.BookedTransactions.Select(transaction =>
        {
            var thirdPartyId = Guid.TryParse(transaction.InternalTransactionId, out var parsedId)
                ? parsedId
                : Guid.Empty;
            var valueDate = transaction.ValueDateTime ?? transaction.ValueDate;

            return new Transaction(thirdPartyId, transaction.TransactionId, valueDate, transaction.CreditorName,
                transaction.DebtorName, transaction.TransactionAmount.Amount,
                transaction.TransactionAmount.Currency, string.Empty, string.Empty, [],
                new BankAccount { ThirdPartyId = goCardlessAccountId });
        }));
    }
}