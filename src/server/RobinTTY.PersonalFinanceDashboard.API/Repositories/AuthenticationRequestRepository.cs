using RobinTTY.NordigenApiClient.Models.Responses;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.ThirdPartyDataProviders;

namespace RobinTTY.PersonalFinanceDashboard.API.Repositories;

/// <summary>
/// Manages <see cref="AuthenticationRequest"/> data retrieval.
/// </summary>
public class AuthenticationRequestRepository
{
    private readonly GoCardlessDataProvider _dataProvider;
    private readonly ApplicationDbContext _dbContext;

    /// <summary>
    /// Creates a new instance of <see cref="AuthenticationRequestRepository"/>.
    /// </summary>
    /// <param name="dataProvider">The data provider to use for data retrieval.</param>
    /// <param name="dbContext">The <see cref="ApplicationDbContext"/> to use for data retrieval.</param>
    public AuthenticationRequestRepository(GoCardlessDataProvider dataProvider, ApplicationDbContext dbContext)
    {
        _dataProvider = dataProvider;
        _dbContext = dbContext;
    }

    public async Task<AuthenticationRequest?> Get(string authenticationId)
    {
        var requests = await _dataProvider.GetAuthenticationRequest(authenticationId);
        return requests.Result!;
    }

    public async Task<IEnumerable<AuthenticationRequest>> GetAll()
    {
        // TODO: limit
        var requests = await _dataProvider.GetAuthenticationRequests(100);
        return requests.Result!;
    }

    public async Task<AuthenticationRequest> Add(string institutionId, string redirectUri)
    {
        var request = await _dataProvider.CreateAuthenticationRequest(institutionId, new Uri(redirectUri));
        return request.Result!;
    }

    public async Task<BasicResponse> Delete(string authenticationId)
    {
        var request = await _dataProvider.DeleteAuthenticationRequest(authenticationId);
        return request.Result!;
    }
}