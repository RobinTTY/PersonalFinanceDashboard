using RobinTTY.NordigenApiClient;
using RobinTTY.NordigenApiClient.Models;
using RobinTTY.NordigenApiClient.Models.Errors;
using RobinTTY.NordigenApiClient.Models.Requests;
using RobinTTY.NordigenApiClient.Models.Responses;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using BankAccount = RobinTTY.PersonalFinanceDashboard.Core.Models.BankAccount;
using Transaction = RobinTTY.PersonalFinanceDashboard.Core.Models.Transaction;

namespace RobinTTY.PersonalFinanceDashboard.ThirdPartyDataProviders;

/// <summary>
/// Data provider for banking data from <see href="https://gocardless.com/bank-account-data/"/>.
/// </summary>
// TODO: Probably should already return a generic ThirdPartyError (type) here
public class GoCardlessDataProvider
{
    private readonly NordigenClient _client;

    public GoCardlessDataProvider(HttpClient httpClient, NordigenClientCredentials credentials)
    {
        // TODO: injection
        _client = new NordigenClient(httpClient, credentials);
    }

    // TODO: Cancellation token support
    /// <summary>
    /// Gets the banking institutions which are available via this data provider.
    /// </summary>
    /// <param name="country">Optional country filter to apply to the query.</param>
    /// <returns>The available banking <see cref="Institution"/>s.</returns>
    public async Task<ThirdPartyResponse<IQueryable<BankingInstitution>, InstitutionsError>> GetBankingInstitutions(string? country = null)
    {
        var response = await _client.InstitutionsEndpoint.GetInstitutions(country);

        if (response.IsSuccess)
        {
            var institutions =
                response.Result.Select(inst => new BankingInstitution(inst.Id, inst.Name, inst.Bic, inst.Logo));
            return new ThirdPartyResponse<IQueryable<BankingInstitution>, InstitutionsError>(true, institutions.AsQueryable(), null);
        }

        return new ThirdPartyResponse<IQueryable<BankingInstitution>, InstitutionsError>(false, null, response.Error);
    }

    /// <summary>
    /// Gets an existing authentication request (requisitions).
    /// </summary>
    /// <param name="requisitionId">The id of the requisition to be retrieved.</param>
    /// <returns>The <see cref="AuthenticationRequest"/> which matches the id, as far as it exists.</returns>
    public async Task<ThirdPartyResponse<AuthenticationRequest?, BasicError?>> GetAuthenticationRequest(string requisitionId)
    {
        var response = await _client.RequisitionsEndpoint.GetRequisition(requisitionId);
        // TODO: handle request failure
        var requisition = response.Result!;
        var result = new AuthenticationRequest(requisition.Id.ToString(), requisition.Accounts.Select(guid => guid.ToString()),
            ConvertRequisitionStatus(requisition.Status), requisition.AuthenticationLink);
        return new ThirdPartyResponse<AuthenticationRequest?, BasicError?>(response.IsSuccess, result, response.Error);
    }

    // TODO: Cancellation token support
    /// <summary>
    /// Gets existing authentication requests (requisitions) for all banking institutions.
    /// </summary>
    /// <param name="requisitionLimit">The maximum number of requisitions to get.</param>
    /// <returns>All existing <see cref="Requisition"/>s.</returns>
    public async Task<ThirdPartyResponse<IQueryable<AuthenticationRequest>, BasicError?>> GetAuthenticationRequests(int requisitionLimit)
    {
        var response = await _client.RequisitionsEndpoint.GetRequisitions(requisitionLimit, 0);
        // TODO: handle request failure
        var requisitions = response.Result!.Results;
        var result = requisitions.Select(req => new AuthenticationRequest(req.Id.ToString(),
            req.Accounts.Select(guid => guid.ToString()), ConvertRequisitionStatus(req.Status), req.AuthenticationLink)).AsQueryable();
        return new ThirdPartyResponse<IQueryable<AuthenticationRequest>, BasicError?>(response.IsSuccess, result, response.Error);

    }

    /// <summary>
    /// Creates a new authentication request (requisition) for the chosen institution and returns it.
    /// </summary>
    /// <param name="institutionId">Id of the institution for which to create the requisition.</param>
    /// <param name="redirectUri"><see cref="Uri"/> which will be redirected too when the user completes authentication for the given requisition.</param>
    /// <returns>The created <see cref="Requisition"/>.</returns>
    public async Task<ThirdPartyResponse<AuthenticationRequest, CreateRequisitionError>> CreateAuthenticationRequest(string institutionId, Uri redirectUri)
    {
        var requisitionRequest = new CreateRequisitionRequest(redirectUri, institutionId, Guid.NewGuid().ToString(), "EN");
        var response = await _client.RequisitionsEndpoint.CreateRequisition(requisitionRequest);

        if (response.IsSuccess)
        {
            var requisition = response.Result;
            var authenticationRequest = new AuthenticationRequest(requisition.Id.ToString(), requisition.Accounts.Select(guid => guid.ToString()),
                ConvertRequisitionStatus(requisition.Status), requisition.AuthenticationLink);
            return new ThirdPartyResponse<AuthenticationRequest, CreateRequisitionError>(response.IsSuccess, authenticationRequest, null);
        }

        return new ThirdPartyResponse<AuthenticationRequest, CreateRequisitionError>(response.IsSuccess, null, response.Error);
    }

    public async Task<ThirdPartyResponse<BasicResponse, BasicError>> DeleteAuthenticationRequest(string authenticationId)
    {
        var response = await _client.RequisitionsEndpoint.DeleteRequisition(authenticationId);
        return new ThirdPartyResponse<BasicResponse, BasicError>(response.IsSuccess, response.Result, response.Error);
    }

    public async Task<ThirdPartyResponse<BankAccount, AccountsError>> GetBankAccount(string accountId)
    {
        var accountDetailsTask = _client.AccountsEndpoint.GetAccountDetails(accountId);
        var balanceTask = _client.AccountsEndpoint.GetBalances(accountId);

        // TODO: Handle possible null values
        var accountDetailsRequest = await accountDetailsTask;
        var balanceRequest = await balanceTask;
        var accountDetailsResult = accountDetailsRequest.Result!;
        var balanceResult = balanceRequest.Result!;
        var bankAccount = ConvertAccount(accountId, accountDetailsResult, balanceResult);

        return new ThirdPartyResponse<BankAccount, AccountsError>(
            accountDetailsRequest.IsSuccess && balanceRequest.IsSuccess, bankAccount, accountDetailsRequest.Error);
    }

    public async Task<List<ThirdPartyResponse<BankAccount, AccountsError>>> GetBankAccounts(IEnumerable<string> accountIds)
    {
        var tasks = accountIds.Select(GetBankAccount).ToList();
        await Task.WhenAll(tasks);
        return tasks.Select(task => task.Result).ToList();
    }

    private AuthenticationStatus ConvertRequisitionStatus(RequisitionStatus status)
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
            RequisitionStatus.Linked => AuthenticationStatus.Successful,
            RequisitionStatus.Expired => AuthenticationStatus.Expired,
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
        };
    }

    private BankAccount ConvertAccount(string accountId, BankAccountDetails account, List<Balance> balances)
    {
        return new BankAccount
        (
            // TODO: Convert Account
            id: accountId,
            accountType: account.CashAccountType.HasValue ? Enum.GetName(typeof(CashAccountType), account.CashAccountType) : null,
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
            transactions: new List<Transaction>()
        );
    }
}
