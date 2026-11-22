using Microsoft.Extensions.Logging;
using RobinTTY.NordigenApiClient;
using RobinTTY.NordigenApiClient.Models.Errors;
using RobinTTY.NordigenApiClient.Models.Responses;
using RobinTTY.PersonalFinanceDashboard.Core.DataProviders;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using BankAccount = RobinTTY.PersonalFinanceDashboard.Core.Models.BankAccount;
using Transaction = RobinTTY.PersonalFinanceDashboard.Core.Models.Transaction;

namespace RobinTTY.PersonalFinanceDashboard.ThirdPartyDataProviders;

/// <summary>
/// Provides banking data from <see href="https://gocardless.com/bank-account-data/">GoCardless Bank Account Data</see>.
/// GoCardless calls authentication requests "requisitions". Ids of requisitions, accounts and transactions are GUIDs.
/// </summary>
public class GoCardlessBankDataProvider(NordigenClient client, ILogger<GoCardlessBankDataProvider> logger)
    : IBankDataProvider
{
    /// <inheritdoc />
    public DataSource Source => DataSource.GoCardless;

    /// <inheritdoc />
    public async Task<ThirdPartyResponse<BankingInstitution>> GetBankingInstitution(string institutionId,
        CancellationToken cancellationToken = default)
    {
        var response = await client.InstitutionsEndpoint.GetInstitution(institutionId, cancellationToken);
        return response.ToThirdPartyResponse(institution => institution.ToBankingInstitution());
    }

    /// <inheritdoc />
    public async Task<ThirdPartyResponse<IReadOnlyList<BankingInstitution>>> GetBankingInstitutions(
        string? country = null, CancellationToken cancellationToken = default)
    {
        var response = await client.InstitutionsEndpoint.GetInstitutions(country, cancellationToken: cancellationToken);
        return response.ToThirdPartyResponse(IReadOnlyList<BankingInstitution> (institutions) =>
            institutions.Select(institution => institution.ToBankingInstitution()).ToList());
    }

    /// <inheritdoc />
    public async Task<ThirdPartyResponse<AuthenticationRequest>> GetAuthenticationRequest(
        string authenticationRequestId, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(authenticationRequestId, out var requisitionId))
            return ThirdPartyResponse<AuthenticationRequest>.Failure(InvalidId(authenticationRequestId));

        var response = await client.RequisitionsEndpoint.GetRequisition(requisitionId, cancellationToken);
        return response.ToThirdPartyResponse(requisition => requisition.ToAuthenticationRequest());
    }

    /// <inheritdoc />
    public async Task<ThirdPartyResponse<IReadOnlyList<AuthenticationRequest>>> GetAuthenticationRequests(
        CancellationToken cancellationToken = default)
    {
        const int pageSize = 100;
        var requisitions = new List<Requisition>();

        while (true)
        {
            var response =
                await client.RequisitionsEndpoint.GetRequisitions(pageSize, requisitions.Count, cancellationToken);
            if (!response.IsSuccess)
                return ThirdPartyResponse<IReadOnlyList<AuthenticationRequest>>.Failure(response.ToThirdPartyError());

            var page = response.Result!;
            requisitions.AddRange(page.Results);
            if (page.Next == null || page.Results.Count == 0)
                break;
        }

        return ThirdPartyResponse<IReadOnlyList<AuthenticationRequest>>.Success(
            requisitions.Select(requisition => requisition.ToAuthenticationRequest()).ToList());
    }

    /// <inheritdoc />
    public async Task<ThirdPartyResponse<AuthenticationRequest>> CreateAuthenticationRequest(string institutionId,
        Uri redirectUri, CancellationToken cancellationToken = default)
    {
        var response =
            await client.RequisitionsEndpoint.CreateRequisition(institutionId, redirectUri,
                reference: Guid.NewGuid().ToString(), cancellationToken: cancellationToken);

        if (response.IsSuccess)
            return ThirdPartyResponse<AuthenticationRequest>.Success(response.Result!.ToAuthenticationRequest());

        // Errors for single request fields (e.g. an unknown institution id) aren't part of the summary
        var error = response.Error!;
        BasicResponse?[] fieldErrors =
        [
            error.InstitutionIdError, error.RedirectError, error.AgreementError, error.ReferenceError,
            error.UserLanguageError, error.SocialSecurityNumberError, error.AccountSelectionError
        ];
        var basicError = error.Summary != null ? error : fieldErrors.FirstOrDefault(e => e != null) ?? error;

        return ThirdPartyResponse<AuthenticationRequest>.Failure(basicError.ToThirdPartyError());
    }

    /// <inheritdoc />
    public async Task<ThirdPartyError?> DeleteAuthenticationRequest(string authenticationRequestId,
        CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(authenticationRequestId, out var requisitionId))
            return InvalidId(authenticationRequestId);

        var response = await client.RequisitionsEndpoint.DeleteRequisition(requisitionId, cancellationToken);
        return response.IsSuccess ? null : response.ToThirdPartyError();
    }

    /// <inheritdoc />
    /// <remarks>The balance, general account information and account details are separate GoCardless requests,
    /// so the latter two are only made if the account's details are requested.</remarks>
    public async Task<IReadOnlyDictionary<string, ThirdPartyResponse<BankAccount>>> GetBankAccounts(
        IReadOnlyDictionary<string, bool> accounts, CancellationToken cancellationToken = default)
    {
        var responses = await Task.WhenAll(accounts.Select(async account =>
            (Id: account.Key, Response: await GetBankAccount(account.Key, account.Value, cancellationToken))));
        return responses.ToDictionary(response => response.Id, response => response.Response);
    }

    private async Task<ThirdPartyResponse<BankAccount>> GetBankAccount(string externalAccountId,
        bool includeMetadata, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(externalAccountId, out var accountId))
            return ThirdPartyResponse<BankAccount>.Failure(InvalidId(externalAccountId));

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
            TimeSpan?[] retryAfter =
            [
                balanceResponse.GetRetryAfter(), generalAccountInfoResponse?.GetRetryAfter(),
                accountDetailsResponse?.GetRetryAfter()
            ];
            return ThirdPartyResponse<BankAccount>.Failure(new ThirdPartyError(
                $"Failed to retrieve data for account {accountId}.", string.Join("; ", errors), retryAfter.Max()));
        }

        var bankAccount = GoCardlessTypeExtensions.CreateBankAccount(accountId, generalAccountInfoResponse?.Result,
            accountDetailsResponse?.Result, balanceResponse.Result!);

        return ThirdPartyResponse<BankAccount>.Success(bankAccount);
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
            logger.LogWarning("Failed to retrieve balances for account {AccountId}: {Error}", accountId, error);
        }

        if (includeMetadata)
        {
            if (generalAccountInfoResponse is { IsSuccess: false })
            {
                var error =
                    $"Summary: {generalAccountInfoResponse.Error.Summary}, Details: {generalAccountInfoResponse.Error.Detail}";

                errorMessages.Add(error);
                logger.LogWarning("Failed to retrieve general account metadata for account {AccountId}: {Error}",
                    accountId, error);
            }

            if (accountDetailsResponse is { IsSuccess: false })
            {
                var error =
                    $"Summary: {accountDetailsResponse.Error.Summary}, Details: {accountDetailsResponse.Error.Detail}";

                errorMessages.Add(error);
                logger.LogWarning("Failed to retrieve detailed account metadata for account {AccountId}: {Error}",
                    accountId, error);
            }
        }

        return errorMessages;
    }

    /// <inheritdoc />
    public async Task<ThirdPartyResponse<IReadOnlyList<Transaction>>> GetTransactions(string accountId,
        DateTime? dateFrom = null, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(accountId, out var goCardlessAccountId))
            return ThirdPartyResponse<IReadOnlyList<Transaction>>.Failure(InvalidId(accountId));

        var response =
            await client.AccountsEndpoint.GetTransactions(goCardlessAccountId,
                dateFrom is { } from ? DateOnly.FromDateTime(from) : null, cancellationToken: cancellationToken);

        // TODO: Also return pending transactions
        return response.ToThirdPartyResponse(IReadOnlyList<Transaction> (result) =>
            result.BookedTransactions.ToTransactions(goCardlessAccountId));
    }

    /// <summary>
    /// Creates the error returned for an id that isn't a GoCardless id.
    /// </summary>
    private static ThirdPartyError InvalidId(string externalId) =>
        new("Invalid id.", $"\"{externalId}\" is not a valid GoCardless id.");
}
