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

    public GoCardlessDataProvider(HttpClient httpClient)
    {
        var credentials = new NordigenClientCredentials("d08de1cd-1971-4d06-880b-75db2910cffe", "83e96fb6364a21a788696e811526d39701ca633c06c7e2560ba2a5eb8e48b236c84c2ad155beeab9da19235ff63d5c40c79e017a040d95f3fd086fffe50a546b");
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
        var response = country is null ? await _client.InstitutionsEndpoint.GetInstitutions(country) : await _client.InstitutionsEndpoint.GetInstitutions();
        
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
    /// Gets existing requisitions for all banking institutions.
    /// </summary>
    /// <param name="requisitionLimit">The maximum number of requisitions to get.</param>
    /// <param name="requisitionId">Optional id filter to apply to the query. Only the requisition which matches the filter will be returned as far as it exists.</param>
    /// <returns>All existing <see cref="Requisition"/>s.</returns>
    public async Task<ThirdPartyResponse<IQueryable<Requisition>, BasicError>> GetBankingInstitutionRequisitions(int requisitionLimit, Guid? requisitionId = null)
    {
        if (requisitionId is null)
        {
            var response = await _client.RequisitionsEndpoint.GetRequisitions(requisitionLimit, 0);
            return new ThirdPartyResponse<IQueryable<Requisition>, BasicError>(response.IsSuccess, response.Result?.Results.AsQueryable(), response.Error);
        }
        else
        {
            var response = await _client.RequisitionsEndpoint.GetRequisition(requisitionId.Value);
            var result = response.IsSuccess ? new List<Requisition> { response.Result }.AsQueryable() : null;
            return new ThirdPartyResponse<IQueryable<Requisition>, BasicError>(response.IsSuccess, result, response.Error);
        }
        
    }

    /// <summary>
    /// Creates a new requisition for the chosen institution and returns it.
    /// </summary>
    /// <param name="institutionId">Id of the institution for which to create the requisition.</param>
    /// <param name="userLanguage">User language for the given requisition.</param>
    /// <param name="internalReference">Internal reference for identifying the requisition.</param>
    /// <param name="redirectUri"><see cref="Uri"/> which will be redirected too when the user completes authentication for the given requisition.</param>
    /// <returns>The created <see cref="Requisition"/>.</returns>
    public async Task<ThirdPartyResponse<Requisition, CreateRequisitionError>> GetNewBankingInstitutionRequisition(string institutionId, string userLanguage, string internalReference, Uri redirectUri)
    {
        var requisitionRequest = new CreateRequisitionRequest(redirectUri, institutionId, internalReference, userLanguage);
        var response = await _client.RequisitionsEndpoint.CreateRequisition(requisitionRequest);
        return new ThirdPartyResponse<Requisition, CreateRequisitionError>(response.IsSuccess, response.Result, response.Error);
    }
}

