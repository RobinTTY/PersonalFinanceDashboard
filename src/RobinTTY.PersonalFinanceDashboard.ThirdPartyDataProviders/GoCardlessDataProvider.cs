using RobinTTY.NordigenApiClient;
using RobinTTY.NordigenApiClient.Models;
using RobinTTY.NordigenApiClient.Models.Errors;
using RobinTTY.NordigenApiClient.Models.Requests;
using RobinTTY.NordigenApiClient.Models.Responses;
using RobinTTY.PersonalFinanceDashboard.Core.Models;

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

    // TODO: Cancellation token support
    /// <summary>
    /// Gets existing authentication requests (requisitions) for all banking institutions.
    /// </summary>
    /// <param name="requisitionLimit">The maximum number of requisitions to get.</param>
    /// <param name="requisitionId">Optional id filter to apply to the query. Only the requisition which matches the filter will be returned as far as it exists.</param>
    /// <returns>All existing <see cref="Requisition"/>s.</returns>
    public async Task<ThirdPartyResponse<IQueryable<AuthenticationRequest>, BasicError>> GetAuthenticationRequests(int requisitionLimit, Guid? requisitionId = null)
    {
        if (requisitionId is null)
        {
            var response = await _client.RequisitionsEndpoint.GetRequisitions(requisitionLimit, 0);
            // TODO: handle request failure
            var requisitions = response.Result!.Results;
            var result = requisitions.Select(req => new AuthenticationRequest(req.Id.ToString(),
                req.Accounts.Select(guid => guid.ToString()), ConvertRequisitionStatus(req.Status), req.AuthenticationLink)).AsQueryable();
            return new ThirdPartyResponse<IQueryable<AuthenticationRequest>, BasicError>(response.IsSuccess, result, response.Error);
        }
        else
        {
            var response = await _client.RequisitionsEndpoint.GetRequisition(requisitionId.Value);
            // TODO: handle request failure
            var requisition = response.Result!;
            var result = new AuthenticationRequest(requisition.Id.ToString(), requisition.Accounts.Select(guid => guid.ToString()),
                ConvertRequisitionStatus(requisition.Status), requisition.AuthenticationLink);
            return new ThirdPartyResponse<IQueryable<AuthenticationRequest>, BasicError>(response.IsSuccess,
                new List<AuthenticationRequest> {result}.AsQueryable(), response.Error);
        }
        
    }

    /// <summary>
    /// Creates a new authentication request (requisition) for the chosen institution and returns it.
    /// </summary>
    /// <param name="institutionId">Id of the institution for which to create the requisition.</param>
    /// <param name="redirectUri"><see cref="Uri"/> which will be redirected too when the user completes authentication for the given requisition.</param>
    /// <returns>The created <see cref="Requisition"/>.</returns>
    public async Task<ThirdPartyResponse<AuthenticationRequest, CreateRequisitionError>> GetNewAuthenticationRequest(string institutionId, Uri redirectUri)
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
}
