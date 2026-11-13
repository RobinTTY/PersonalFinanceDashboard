using Microsoft.EntityFrameworkCore;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
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
    public async Task AddAuthenticationRequest_SavesTheCreatedRequest()
    {
        _env.GoCardless.Respond(RequisitionsPath, RequisitionJson(RequestId, "CR", _env.UtcNow));

        await using (var dbContext = _env.CreateDbContext())
        {
            var response = await _env.CreateAuthenticationRequestRepository(dbContext)
                .AddAuthenticationRequest("SANDBOXFINANCE_SFIN0000", RedirectUri);
            Assert.True(response.IsSuccessful);
        }

        Assert.Equal(RequestId, (await GetRequests()).Single().ThirdPartyId);
    }

    [Fact]
    public async Task AddAuthenticationRequest_FailedRequestSavesNothing()
    {
        await using (var dbContext = _env.CreateDbContext())
        {
            var response = await _env.CreateAuthenticationRequestRepository(dbContext)
                .AddAuthenticationRequest("UNKNOWN", RedirectUri);
            Assert.False(response.IsSuccessful);
        }

        Assert.Empty(await GetRequests());
    }

    [Fact]
    public async Task DeleteAuthenticationRequest_RemovesItAfterGoCardlessDeletedIt()
    {
        var requestId = await SeedRequest();
        _env.GoCardless.Respond(RequisitionPath(RequestId), Deleted("Requisition deleted"));

        await using (var dbContext = _env.CreateDbContext())
        {
            var response = await _env.CreateAuthenticationRequestRepository(dbContext)
                .DeleteAuthenticationRequest(requestId);
            Assert.True(response.IsSuccessful);
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
            var response = await _env.CreateAuthenticationRequestRepository(dbContext)
                .DeleteAuthenticationRequest(requestId);
            Assert.False(response.IsSuccessful);
        }

        Assert.Single(await GetRequests());
    }

    [Fact]
    public async Task DeleteAuthenticationRequest_UnknownIdDoesNotCallGoCardless()
    {
        await using var dbContext = _env.CreateDbContext();

        var response = await _env.CreateAuthenticationRequestRepository(dbContext)
            .DeleteAuthenticationRequest(Guid.NewGuid());

        Assert.False(response.IsSuccessful);
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