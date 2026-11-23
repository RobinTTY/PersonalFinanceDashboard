using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using RobinTTY.NordigenApiClient.Models.Responses;
using RobinTTY.PersonalFinanceDashboard.Core.DataProviders;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.Core.Models.Base;
using BankAccount = RobinTTY.PersonalFinanceDashboard.Core.Models.BankAccount;
using NordigenBankAccount = RobinTTY.NordigenApiClient.Models.Responses.BankAccount;
using NordigenTransaction = RobinTTY.NordigenApiClient.Models.Responses.Transaction;
using Transaction = RobinTTY.PersonalFinanceDashboard.Core.Models.Transaction;

namespace RobinTTY.PersonalFinanceDashboard.ThirdPartyDataProviders;

/// <summary>
/// Maps between GoCardless (Nordigen API client) types and the application's types.
/// </summary>
public static class GoCardlessTypeExtensions
{
    /// <summary>
    /// Converts a GoCardless id to the format stored as <see cref="SourcedEntity.ExternalId"/>.
    /// </summary>
    public static string ToExternalId(this Guid goCardlessId) => goCardlessId.ToString("D");

    /// <summary>
    /// Maps a <see cref="NordigenApiResponse{TResult,TError}"/> to a <see cref="ThirdPartyResponse{TResult}"/>,
    /// applying <paramref name="map"/> to the result only when the request was successful.
    /// </summary>
    /// <param name="response">The Nordigen response to map.</param>
    /// <param name="map">Projection applied to the result when the request succeeded.</param>
    public static ThirdPartyResponse<TResult> ToThirdPartyResponse<TSource, TResult, TError>(
        this NordigenApiResponse<TSource, TError> response, Func<TSource, TResult> map)
        where TSource : class
        where TResult : notnull
        where TError : BasicResponse
    {
        return response.IsSuccess
            ? ThirdPartyResponse<TResult>.Success(map(response.Result!))
            : ThirdPartyResponse<TResult>.Failure(response.ToThirdPartyError());
    }

    /// <summary>
    /// Maps the error of a failed <see cref="NordigenApiResponse{TResult,TError}"/> to a <see cref="ThirdPartyError"/>,
    /// including when to retry if the request was rate limited.
    /// </summary>
    public static ThirdPartyError ToThirdPartyError<TSource, TError>(this NordigenApiResponse<TSource, TError> response)
        where TSource : class
        where TError : BasicResponse =>
        response.Error!.ToThirdPartyError() with { RetryAfter = response.GetRetryAfter() };

    /// <summary>
    /// Gets how long to wait before retrying a rate limited request, <see langword="null"/> if the request wasn't
    /// rate limited.
    /// </summary>
    public static TimeSpan? GetRetryAfter<TSource, TError>(this NordigenApiResponse<TSource, TError> response)
        where TSource : class
        where TError : BasicResponse
    {
        if (response.StatusCode != HttpStatusCode.TooManyRequests)
            return null;

        // The accounts endpoint has its own, stricter limit
        var seconds = response.RateLimits?.RemainingSecondsInTimeWindowAccountsEndpoint ??
                      response.RateLimits?.RemainingSecondsInTimeWindow;
        return seconds is null ? null : TimeSpan.FromSeconds(seconds.Value);
    }

    /// <summary>
    /// Maps an error returned by GoCardless to a <see cref="ThirdPartyError"/>.
    /// </summary>
    public static ThirdPartyError ToThirdPartyError(this BasicResponse error) =>
        new(error.Summary ?? "The request to GoCardless failed.", error.Detail);

    /// <summary>
    /// Maps a <see cref="Institution"/> to a <see cref="BankingInstitution"/>.
    /// </summary>
    public static BankingInstitution ToBankingInstitution(this Institution institution)
    {
        return new BankingInstitution(DataSource.GoCardless, institution.Id, institution.Bic, institution.Name,
            institution.Logo, institution.Countries);
    }

    /// <summary>
    /// Maps a <see cref="Requisition"/> to an <see cref="AuthenticationRequest"/>.
    /// </summary>
    /// <param name="requisition">The requisition to map.</param>
    /// <param name="accessValidForDays">How long the agreement of the requisition grants access, if known.</param>
    public static AuthenticationRequest ToAuthenticationRequest(this Requisition requisition,
        int? accessValidForDays = null)
    {
        return new AuthenticationRequest(DataSource.GoCardless, requisition.Id.ToExternalId(),
            requisition.Status.ToAuthenticationStatus(), requisition.AuthenticationLink, requisition.Created,
            requisition.Accounts
                .Select(accountId => new BankAccount(DataSource.GoCardless, accountId.ToExternalId()))
                .ToList())
        {
            AccessValidForDays = accessValidForDays,
            Institution = new BankingInstitution
            {
                Source = DataSource.GoCardless,
                ExternalId = requisition.InstitutionId
            }
        };
    }

    /// <summary>
    /// Maps a <see cref="NordigenBankAccount"/> to a <see cref="BankAccount"/>.
    /// </summary>
    /// <param name="accountId">The id of the account to map.</param>
    /// <param name="generalAccountInfo"></param>
    /// <param name="accountDetails"></param>
    /// <param name="balances"></param>
    /// <returns></returns>
    public static BankAccount CreateBankAccount(Guid accountId, NordigenBankAccount? generalAccountInfo,
        BankAccountDetails? accountDetails, List<Balance> balances)
    {
        return new BankAccount
        (
            source: DataSource.GoCardless,
            externalId: accountId.ToExternalId(),
            accountType: accountDetails?.CashAccountType is { } cashAccountType
                ? Enum.GetName(cashAccountType)
                : null,
            name: null,
            description: null,
            providerName: accountDetails?.Product,
            providerDescription: accountDetails?.Details,
            balance: SelectBalance(balances),
            currency: accountDetails?.Currency,
            iban: accountDetails?.Iban,
            bic: accountDetails?.Bic,
            bban: accountDetails?.Bban,
            ownerName: accountDetails?.OwnerName,
            transactions: [],
            associatedInstitution: generalAccountInfo != null
                ? new BankingInstitution
                {
                    Source = DataSource.GoCardless,
                    ExternalId = generalAccountInfo.InstitutionId
                }
                : null,
            associatedAuthenticationRequests: []);
    }

    // Booked balances come first, since the net worth history subtracts booked transactions from the balance.
    // Available balances can include a credit line, so they're only used if a bank reports nothing else.
    private static readonly BalanceType[] BalancePreference =
    [
        BalanceType.InterimBooked, BalanceType.ClosingBooked, BalanceType.Expected,
        BalanceType.InterimAvailable, BalanceType.ClosingAvailable
    ];

    private static decimal? SelectBalance(List<Balance> balances) =>
        BalancePreference
            .Select(type => balances.FirstOrDefault(balance => balance.BalanceType == type))
            .FirstOrDefault(balance => balance != null)
            ?.BalanceAmount.Amount;

    /// <summary>
    /// Maps the booked and pending transactions of a GoCardless account to <see cref="Transaction"/>s, giving each
    /// one a <see cref="SourcedEntity.ExternalId"/> that is stable across syncs.
    /// </summary>
    /// <param name="transactions">The transactions as returned by GoCardless, in the order of the response.</param>
    /// <param name="goCardlessAccountId">The GoCardless id of the account the transactions belong to.</param>
    /// <returns>The mapped transactions.</returns>
    public static List<Transaction> ToTransactions(this AccountTransactions transactions, Guid goCardlessAccountId)
    {
        // Counts how often each transaction content occurred, so identical transactions (e.g. two equal
        // purchases on the same day) still get distinct ids. Booked transactions come first, so their ids don't
        // depend on the pending ones.
        var contentOccurrences = new Dictionary<string, int>();

        return transactions.BookedTransactions
            .Select(transaction => (Transaction: transaction, Status: TransactionStatus.Booked))
            .Concat((transactions.PendingTransactions ?? [])
                .Select(transaction => (Transaction: transaction, Status: TransactionStatus.Pending)))
            .Select(entry =>
            {
                var transaction = entry.Transaction;
                var externalId = GetTransactionId(transaction, goCardlessAccountId, contentOccurrences)
                    .ToExternalId();

                // The debtor sends the money (payer), the creditor receives it (payee)
                return new Transaction(DataSource.GoCardless, externalId, transaction.TransactionId,
                    transaction.ValueDateTime ?? transaction.ValueDate, payer: transaction.DebtorName,
                    payee: transaction.CreditorName, transaction.TransactionAmount.Amount,
                    transaction.TransactionAmount.Currency, category: null, notes: null, [],
                    new BankAccount(DataSource.GoCardless, goCardlessAccountId.ToExternalId()), entry.Status,
                    transaction.BookingDateTime ?? transaction.BookingDate, GetDescription(transaction));
            }).ToList();
    }

    /// <summary>
    /// Gets what a transaction is for. Card payments often have no creditor name, the merchant is only named here.
    /// </summary>
    private static string? GetDescription(NordigenTransaction transaction) =>
        transaction.RemittanceInformationUnstructured is { Length: > 0 } description
            ? description
            : transaction.RemittanceInformationUnstructuredArray is { Count: > 0 } lines
                ? string.Join(' ', lines)
                : null;

    /// <summary>
    /// Determines the id of a transaction. The GoCardless id is used when it is a GUID. All identifiers are
    /// optional though, so otherwise a deterministic GUID is derived from the ids provided by the bank, or as a
    /// last resort from the transaction's content.
    /// </summary>
    private static Guid GetTransactionId(NordigenTransaction transaction, Guid goCardlessAccountId,
        Dictionary<string, int> contentOccurrences)
    {
        if (Guid.TryParse(transaction.InternalTransactionId, out var internalTransactionId))
            return internalTransactionId;

        string key;
        if (!string.IsNullOrWhiteSpace(transaction.InternalTransactionId))
            key = $"internal:{transaction.InternalTransactionId.Trim()}";
        else if (!string.IsNullOrWhiteSpace(transaction.TransactionId))
            key = $"bank:{transaction.TransactionId.Trim()}";
        else if (!string.IsNullOrWhiteSpace(transaction.EntryReference))
            key = $"ref:{transaction.EntryReference.Trim()}";
        else
        {
            var content = GetContentKey(transaction);
            var occurrence = contentOccurrences[content] = contentOccurrences.GetValueOrDefault(content) + 1;
            key = $"content:{content}#{occurrence}";
        }

        // Bank ids are only unique within an account, so the account id is part of the hashed input
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes($"{goCardlessAccountId}|{key}"));
        return new Guid(hash.AsSpan(0, 16));
    }

    /// <summary>
    /// Combines the fields that describe a transaction and don't change once it is booked.
    /// </summary>
    private static string GetContentKey(NordigenTransaction transaction)
    {
        string?[] fields =
        [
            (transaction.BookingDateTime ?? transaction.BookingDate)?.ToString("O", CultureInfo.InvariantCulture),
            (transaction.ValueDateTime ?? transaction.ValueDate)?.ToString("O", CultureInfo.InvariantCulture),
            transaction.TransactionAmount.Amount.ToString(CultureInfo.InvariantCulture),
            transaction.TransactionAmount.Currency,
            transaction.DebtorName,
            transaction.CreditorName,
            transaction.RemittanceInformationUnstructured,
            transaction.EndToEndId
        ];

        // Unit separator, so that e.g. ("ab", "c") and ("a", "bc") don't produce the same key
        return string.Join('\u001F', fields.Select(field => field ?? string.Empty));
    }

    /// <summary>
    /// Maps a <see cref="RequisitionStatus"/> to an <see cref="AuthenticationStatus"/>.
    /// </summary>
    /// <param name="status">The requisition status to map.</param>
    /// <returns>The mapped authentication status, <see cref="AuthenticationStatus.Unknown"/> for statuses that
    /// GoCardless didn't document or that aren't handled yet.</returns>
    private static AuthenticationStatus ToAuthenticationStatus(this RequisitionStatus status)
    {
        return status switch
        {
            RequisitionStatus.Created or RequisitionStatus.GivingConsent
                or RequisitionStatus.UndergoingAuthentication or RequisitionStatus.SelectingAccounts
                or RequisitionStatus.GrantingAccess => AuthenticationStatus.RequiresUserAction,
            RequisitionStatus.Suspended or RequisitionStatus.Rejected => AuthenticationStatus.Failed,
            RequisitionStatus.Linked => AuthenticationStatus.Active,
            RequisitionStatus.Expired => AuthenticationStatus.Expired,
            _ => AuthenticationStatus.Unknown
        };
    }
}
