using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using RobinTTY.NordigenApiClient.Models.Responses;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using BankAccount = RobinTTY.PersonalFinanceDashboard.Core.Models.BankAccount;
using NordigenBankAccount = RobinTTY.NordigenApiClient.Models.Responses.BankAccount;
using NordigenTransaction = RobinTTY.NordigenApiClient.Models.Responses.Transaction;
using Transaction = RobinTTY.PersonalFinanceDashboard.Core.Models.Transaction;

namespace RobinTTY.PersonalFinanceDashboard.ThirdPartyDataProviders;

public static class GoCardlessTypeExtensions
{
    /// <summary>
    /// Maps a <see cref="NordigenApiResponse{TResult,TError}"/> to a <see cref="ThirdPartyResponse{TResponse,TError}"/>,
    /// applying <paramref name="map"/> to the result only when the request was successful.
    /// </summary>
    /// <param name="response">The Nordigen response to map.</param>
    /// <param name="map">Projection applied to the result when the request succeeded.</param>
    public static ThirdPartyResponse<TResult, TError> ToThirdPartyResponse<TSource, TResult, TError>(
        this NordigenApiResponse<TSource, TError> response, Func<TSource, TResult> map)
        where TSource : class
        where TError : class
    {
        return response.IsSuccess
            ? new ThirdPartyResponse<TResult, TError>(true, map(response.Result!), null)
            : new ThirdPartyResponse<TResult, TError>(false, default, response.Error);
    }

    /// <summary>
    /// Maps a <see cref="Institution"/> to a <see cref="BankingInstitution"/>.
    /// </summary>
    public static BankingInstitution ToBankingInstitution(this Institution institution)
    {
        return new BankingInstitution(institution.Id, institution.Bic, institution.Name, institution.Logo,
            institution.Countries);
    }

    /// <summary>
    /// Maps a <see cref="Requisition"/> to an <see cref="AuthenticationRequest"/>.
    /// </summary>
    public static AuthenticationRequest ToAuthenticationRequest(this Requisition requisition)
    {
        return new AuthenticationRequest(requisition.Id, requisition.Status.ToAuthenticationStatus(),
            requisition.AuthenticationLink, requisition.Created,
            requisition.Accounts.Select(accountId => new BankAccount(accountId)).ToList());
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
            thirdPartyId: accountId,
            accountType: accountDetails?.CashAccountType is { } cashAccountType
                ? Enum.GetName(cashAccountType)
                : null,
            name: accountDetails?.Product,
            description: accountDetails?.Details,
            balance: SelectBalance(balances),
            currency: accountDetails?.Currency,
            iban: accountDetails?.Iban,
            bic: accountDetails?.Bic,
            bban: accountDetails?.Bban,
            ownerName: accountDetails?.OwnerName,
            transactions: [],
            associatedInstitution: generalAccountInfo != null
                ? new BankingInstitution { ThirdPartyId = generalAccountInfo.InstitutionId }
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
    /// Maps the booked transactions of a GoCardless account to <see cref="Transaction"/>s, giving each one a
    /// <see cref="Transaction.ThirdPartyId"/> that is stable across syncs.
    /// </summary>
    /// <param name="transactions">The transactions as returned by GoCardless, in the order of the response.</param>
    /// <param name="goCardlessAccountId">The GoCardless id of the account the transactions belong to.</param>
    /// <returns>The mapped transactions.</returns>
    public static List<Transaction> ToTransactions(this IEnumerable<NordigenTransaction> transactions,
        Guid goCardlessAccountId)
    {
        // Counts how often each transaction content occurred, so identical transactions (e.g. two equal
        // purchases on the same day) still get distinct ids
        var contentOccurrences = new Dictionary<string, int>();

        return transactions.Select(transaction =>
        {
            var thirdPartyId = GetTransactionId(transaction, goCardlessAccountId, contentOccurrences);
            var valueDate = transaction.ValueDateTime ?? transaction.ValueDate;

            // The debtor sends the money (payer), the creditor receives it (payee)
            return new Transaction(thirdPartyId, transaction.TransactionId, valueDate,
                payer: transaction.DebtorName, payee: transaction.CreditorName, transaction.TransactionAmount.Amount,
                transaction.TransactionAmount.Currency, string.Empty, string.Empty, [],
                new BankAccount { ThirdPartyId = goCardlessAccountId });
        }).ToList();
    }

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
    /// <returns>The mapped authentication status.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the <paramref name="status"/> is not a valid <see cref="RequisitionStatus"/>.</exception>
    private static AuthenticationStatus ToAuthenticationStatus(this RequisitionStatus status)
    {
        return status switch
        {
            RequisitionStatus.Created or RequisitionStatus.GivingConsent
                or RequisitionStatus.UndergoingAuthentication or RequisitionStatus.SelectingAccounts
                or RequisitionStatus.GrantingAccess => AuthenticationStatus.RequiresUserAction,
            RequisitionStatus.Undefined or RequisitionStatus.Suspended
                or RequisitionStatus.Rejected => AuthenticationStatus.Failed,
            RequisitionStatus.Linked => AuthenticationStatus.Active,
            RequisitionStatus.Expired => AuthenticationStatus.Expired,
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
        };
    }
}