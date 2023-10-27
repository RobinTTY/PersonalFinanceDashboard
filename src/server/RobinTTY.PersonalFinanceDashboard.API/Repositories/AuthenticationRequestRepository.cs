using RobinTTY.NordigenApiClient.Models.Responses;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.ThirdPartyDataProviders;

namespace RobinTTY.PersonalFinanceDashboard.API.Repositories;

/// <summary>
/// Manages <see cref="AuthenticationRequest"/> data retrieval.
/// </summary>
public class AuthenticationRequestRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly GoCardlessDataProvider _dataProvider;

    /// <summary>
    /// Creates a new instance of <see cref="AuthenticationRequestRepository"/>.
    /// </summary>
    /// <param name="dbContext">The <see cref="ApplicationDbContext"/> to use for data retrieval.</param>
    /// <param name="dataProvider">The data provider to use for data retrieval.</param>
    public AuthenticationRequestRepository(ApplicationDbContext dbContext, GoCardlessDataProvider dataProvider)
    {
        _dbContext = dbContext;
        _dataProvider = dataProvider;
    }

    /// <summary>
    /// Gets the <see cref="AuthenticationRequest"/> matching the specified id.
    /// </summary>
    /// <param name="authenticationId">The id of the <see cref="AuthenticationRequest"/> to retrieve.</param>
    /// <returns>The <see cref="AuthenticationRequest"/> if one ist matched otherwise <see langword="null"/>.</returns>
    public async Task<AuthenticationRequest?> Get(string authenticationId)
    {
        var requests = await _dataProvider.GetAuthenticationRequest(authenticationId);
        return requests.Result!;
    }

    /// <summary>
    /// Gets all <see cref="AuthenticationRequest"/>s.
    /// </summary>
    /// <returns>A list of all <see cref="AuthenticationRequest"/>s.</returns>
    public async Task<IEnumerable<AuthenticationRequest>> GetAll()
    {
        // TODO: limit
        var requests = await _dataProvider.GetAuthenticationRequests(100);
        return requests.Result!;
    }

    /// <summary>
    /// Adds a new <see cref="AuthenticationRequest"/>.
    /// </summary>
    /// <param name="institutionId">The id of the institution to create a <see cref="AuthenticationRequest"/> for.</param>
    /// <param name="redirectUri">The URI of the page to redirect to after successful authentication.</param>
    /// <returns>The added <see cref="AuthenticationRequest"/>.</returns>
    public async Task<AuthenticationRequest> Add(string institutionId, string redirectUri)
    {
        // TODO: URI validation (here or in resolver?)
        var request = await _dataProvider.CreateAuthenticationRequest(institutionId, new Uri(redirectUri));
        return request.Result!;
    }

    /// <summary>
    /// Deletes an existing <see cref="AuthenticationRequest"/>.
    /// </summary>
    /// <param name="authenticationId">The id of the <see cref="AuthenticationRequest"/> to delete.</param>
    /// <returns>TODO</returns>
    public async Task<BasicResponse> Delete(string authenticationId)
    {
        var request = await _dataProvider.DeleteAuthenticationRequest(authenticationId);
        return request.Result!;
    }
}