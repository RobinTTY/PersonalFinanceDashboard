using RobinTTY.NordigenApiClient;
using RobinTTY.NordigenApiClient.Models;
using RobinTTY.NordigenApiClient.Models.Errors;
using RobinTTY.PersonalFinanceDashboard.Core.Models;

namespace RobinTTY.PersonalFinanceDashboard.ThirdPartyDataProviders;

/// <summary>
/// Data provider for banking data from <see href="https://gocardless.com/bank-account-data/"/>.
/// </summary>
public class GoCardlessDataProvider
{
    private readonly NordigenClient _client;

    public GoCardlessDataProvider(HttpClient httpClient)
    {
        var credentials = new NordigenClientCredentials("d08de1cd-1971-4d06-880b-75db2910cffe", "83e96fb6364a21a788696e811526d39701ca633c06c7e2560ba2a5eb8e48b236c84c2ad155beeab9da19235ff63d5c40c79e017a040d95f3fd086fffe50a546b");
        _client = new NordigenClient(httpClient, credentials);
    }

    // TODO: Maybe already return a generic ThirdPartyError type here
    public async Task<ThirdPartyResponse<IQueryable<BankingInstitution>, InstitutionsError>> GetBankingInstitutions(string? country = null)
    {
        var institutionsRequest = country is null ? await _client.InstitutionsEndpoint.GetInstitutions(country) : await _client.InstitutionsEndpoint.GetInstitutions();
        
        if (institutionsRequest.IsSuccess)
        {
            var institutions =
                institutionsRequest.Result.Select(inst => new BankingInstitution(inst.Name, inst.Bic, inst.Logo));
            return new ThirdPartyResponse<IQueryable<BankingInstitution>, InstitutionsError>(true, institutions.AsQueryable(), null);
        }

        return new ThirdPartyResponse<IQueryable<BankingInstitution>, InstitutionsError>(false, null, institutionsRequest.Error);
    }
}

