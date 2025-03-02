using RobinTTY.NordigenApiClient.Models.Responses;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using BankAccount = RobinTTY.PersonalFinanceDashboard.Core.Models.BankAccount;

namespace RobinTTY.PersonalFinanceDashboard.ThirdPartyDataProviders;

public static class GoCardlessTypeExtensions
{
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
    
    public static BankAccount CreateBankAccount(Guid accountId, BankAccountDetails account, List<Balance> balances)
    {
        return new BankAccount
        (
            // TODO: Convert Account
            thirdPartyId: accountId,
            accountType: account.CashAccountType.HasValue
                ? Enum.GetName(typeof(CashAccountType), account.CashAccountType)
                : null,
            name: account.Product,
            description: account.Details,
            // TODO: The balance type could be something different (which one should be grabbed?)
            balance: balances.First(bal =>
                    bal.BalanceType is BalanceType.ClosingBooked or BalanceType.InterimAvailable)
                .BalanceAmount.Amount,
            currency: account.Currency,
            iban: account.Iban,
            bic: account.Bic,
            bban: account.Bban,
            ownerName: account.OwnerName,
            transactions: [],
            associatedInstitution: new BankingInstitution("id", "bic", "name", new Uri("http://www.example.com"), [])
            {
                Id = Guid.Empty
            },
            associatedAuthenticationRequests: [])
        {
            Id = null
        };
    }
}