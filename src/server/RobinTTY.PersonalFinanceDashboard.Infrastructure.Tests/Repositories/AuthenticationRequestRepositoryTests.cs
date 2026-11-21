using Microsoft.EntityFrameworkCore;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Repositories;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Tests.TestUtilities;
using static RobinTTY.PersonalFinanceDashboard.Infrastructure.Tests.TestUtilities.GoCardlessJson;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Tests.Repositories;

public class AuthenticationRequestRepositoryTests : IDisposable
{
    private static readonly Guid RequestId = Guid.Parse("e1000000-0000-0000-0000-000000000001");
    private static readonly Uri RedirectUri = new("https://localhost/");

    private readonly SyncTestEnvironment _env = new();

    public void Dispose() => _env.Dispose();

    [Fact]
    public async Task GetAuthenticationRequest_SyncsTheAccountsOfAnActiveRequest()
    {
        var accountId = Guid.NewGuid();
        var request = TestData.Request(RequestId, _env.UtcNow.AddDays(-1), TestData.Account(accountId));
        await _env.Seed(request);
        _env.GoCardless.Respond(RequisitionPath(RequestId),
            RequisitionJson(RequestId, "LN", _env.UtcNow.AddDays(-1), accountId));
        _env.GoCardless.Respond(BalancesPath(accountId), Balances(42m));

        await using var dbContext = _env.CreateDbContext();
        await _env.CreateAuthenticationRequestRepository(dbContext).GetAuthenticationRequest(request.Id!.Value);

        Assert.Equal(1, _env.GoCardless.RequestCount(BalancesPath(accountId)));
    }

    [Fact]
    public async Task AddAuthenticationRequest_SavesTheCreatedRequest()
    {
        _env.GoCardless.Respond(RequisitionsPath, RequisitionJson(RequestId, "CR", _env.UtcNow));

        await using (var dbContext = _env.CreateDbContext())
        {
            var response = await _env.CreateAuthenticationRequestRepository(dbContext)
                .AddAuthenticationRequest(TestData.Institution("SANDBOXFINANCE_SFIN0000"), RedirectUri);
            Assert.IsType<CreateAuthenticationRequestResult.Created>(response);
        }

        Assert.Equal(RequestId.ToString(), (await GetRequests()).Single().ExternalId);
    }

    [Fact]
    public async Task AddAuthenticationRequest_SendsTheExternalInstitutionIdToGoCardless()
    {
        _env.GoCardless.Respond(RequisitionsPath, RequisitionJson(RequestId, "CR", _env.UtcNow));

        await using var dbContext = _env.CreateDbContext();
        await _env.CreateAuthenticationRequestRepository(dbContext)
            .AddAuthenticationRequest(TestData.Institution("SANDBOXFINANCE_SFIN0000"), RedirectUri);

        Assert.Contains("\"SANDBOXFINANCE_SFIN0000\"", _env.GoCardless.LastRequestBody);
    }

    [Fact]
    public async Task AddAuthenticationRequest_FailedRequestSavesNothing()
    {
        await using (var dbContext = _env.CreateDbContext())
        {
            var response = await _env.CreateAuthenticationRequestRepository(dbContext)
                .AddAuthenticationRequest(TestData.Institution("UNKNOWN"), RedirectUri);
            Assert.IsType<CreateAuthenticationRequestResult.ProviderFailed>(response);
        }

        Assert.Empty(await GetRequests());
    }

    [Fact]
    public async Task AddAuthenticationRequest_ManualInstitutionIsNotConnectable()
    {
        await using var dbContext = _env.CreateDbContext();

        var response = await _env.CreateAuthenticationRequestRepository(dbContext)
            .AddAuthenticationRequest(TestData.ManualInstitution(), RedirectUri);

        Assert.IsType<CreateAuthenticationRequestResult.NotConnectable>(response);
        Assert.Equal(0, _env.GoCardless.TotalRequestCount);
    }

    [Fact]
    public async Task DeleteAuthenticationRequest_RemovesItAfterGoCardlessDeletedIt()
    {
        var requestId = await SeedRequest();
        _env.GoCardless.Respond(RequisitionPath(RequestId), Deleted("Requisition deleted"));

        await using (var dbContext = _env.CreateDbContext())
        {
            var error = await _env.CreateAuthenticationRequestRepository(dbContext)
                .DeleteAuthenticationRequest(requestId);
            Assert.Null(error);
        }

        Assert.Empty(await GetRequests());
    }

    [Fact]
    public async Task DeleteAuthenticationRequest_KeepsItWhenGoCardlessFails()
    {
        // No response is configured, so the request fails
        var requestId = await SeedRequest();

        await using (var dbContext = _env.CreateDbContext())
        {
            var error = await _env.CreateAuthenticationRequestRepository(dbContext)
                .DeleteAuthenticationRequest(requestId);
            Assert.NotNull(error);
        }

        Assert.Single(await GetRequests());
    }

    [Fact]
    public async Task DeleteAuthenticationRequest_UnknownIdDoesNotCallGoCardless()
    {
        await using var dbContext = _env.CreateDbContext();

        var error = await _env.CreateAuthenticationRequestRepository(dbContext)
            .DeleteAuthenticationRequest(Guid.NewGuid());

        Assert.NotNull(error);
        Assert.Equal(0, _env.GoCardless.TotalRequestCount);
    }

    private async Task<Guid> SeedRequest()
    {
        var request = TestData.Request(RequestId, _env.UtcNow.AddDays(-1));
        await _env.Seed(request);
        return request.Id!.Value;
    }

    private async Task<List<AuthenticationRequest>> GetRequests()
    {
        await using var dbContext = _env.CreateDbContext();
        return await dbContext.AuthenticationRequests.ToListAsync();
    }
}