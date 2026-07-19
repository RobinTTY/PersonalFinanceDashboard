using RobinTTY.NordigenApiClient.Models.Responses;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using BankAccount = RobinTTY.PersonalFinanceDashboard.Core.Models.BankAccount;
using NordigenBankAccount = RobinTTY.NordigenApiClient.Models.Responses.BankAccount;

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

    public static AuthenticationStatus ToAuthenticationStatus(this RequisitionStatus status)
    {
        return status switch
        {
            RequisitionStatus.Created => AuthenticationStatus.RequiresUserAction,
            RequisitionStatus.GivingConsent => AuthenticationStatus.RequiresUserAction,
            RequisitionStatus.UndergoingAuthentication => AuthenticationStatus.RequiresUserAction,
            RequisitionStatus.SelectingAccounts => AuthenticationStatus.RequiresUserAction,
            RequisitionStatus.GrantingAccess => AuthenticationStatus.RequiresUserAction,
            RequisitionStatus.Undefined => AuthenticationStatus.Failed,
            RequisitionStatus.Suspended => AuthenticationStatus.Failed,
            RequisitionStatus.Rejected => AuthenticationStatus.Failed,
            RequisitionStatus.Linked => AuthenticationStatus.Active,
            RequisitionStatus.Expired => AuthenticationStatus.Expired,
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
        };
    }

    public static BankAccount CreateBankAccount(Guid accountId, NordigenBankAccount? bankAccount,
        BankAccountDetails? account, List<Balance> balances)
    {
        return new BankAccount
        (
            thirdPartyId: accountId,
            accountType: account?.CashAccountType.HasValue ?? false
                ? Enum.GetName(typeof(CashAccountType), account.CashAccountType)
                : null,
            name: account?.Product,
            description: account?.Details,
            // TODO: The balance type could be something different (which one should be grabbed?)
            balance: balances.FirstOrDefault(bal =>
                    bal.BalanceType is BalanceType.ClosingBooked or BalanceType.InterimAvailable)
                ?.BalanceAmount.Amount,
            currency: account?.Currency,
            iban: account?.Iban,
            bic: account?.Bic,
            bban: account?.Bban,
            ownerName: account?.OwnerName,
            transactions: [],
            associatedInstitution: bankAccount != null
                ? new BankingInstitution { ThirdPartyId = bankAccount.InstitutionId }
                : null,
            associatedAuthenticationRequests: []);
    }
}