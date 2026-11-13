using Microsoft.EntityFrameworkCore;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Tests.TestUtilities;
using static RobinTTY.PersonalFinanceDashboard.Infrastructure.Tests.TestUtilities.GoCardlessJson;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Tests.DataSynchronization;

public class AuthenticationRequestSyncHandlerTests : IDisposable
{
    private static readonly Guid RequestId = Guid.Parse("e1000000-0000-0000-0000-000000000001");
    private static readonly Guid OtherRequestId = Guid.Parse("e2000000-0000-0000-0000-000000000002");
    private static readonly Guid AccountId = Guid.Parse("a1000000-0000-0000-0000-000000000001");
    private static readonly Guid OtherAccountId = Guid.Parse("a2000000-0000-0000-0000-000000000002");

    private readonly SyncTestEnvironment _env = new();

    public void Dispose() => _env.Dispose();

    [Fact]
    public async Task NewRequests_AreAddedWithTheirAccounts()
    {
        _env.GoCardless.Respond(RequisitionsPath,
            Requisitions(Requisition(RequestId, "LN", _env.UtcNow.AddDays(-1), AccountId, OtherAccountId)));

        Assert.True(await _env.SyncAuthenticationRequests());

        var request = (await GetRequests()).Single();
        Assert.Equal(AuthenticationStatus.Active, request.Status);
        Assert.Equal([AccountId, OtherAccountId], request.AssociatedAccounts.Select(a => a.ThirdPartyId).Order());
    }

    [Fact]
    public async Task AccountSharedByTwoRequests_IsAddedOnce()
    {
        _env.GoCardless.Respond(RequisitionsPath, Requisitions(
            Requisition(RequestId, "LN", _env.UtcNow.AddDays(-30), AccountId),
            Requisition(OtherRequestId, "LN", _env.UtcNow.AddDays(-1), AccountId, OtherAccountId)));

        Assert.True(await _env.SyncAuthenticationRequests());

        await using var dbContext = _env.CreateDbContext();
        Assert.Equal(2, await dbContext.BankAccounts.CountAsync());
        var requests = await GetRequests();
        Assert.Single(requests.Single(r => r.ThirdPartyId == RequestId).AssociatedAccounts);
        Assert.Equal(2, requests.Single(r => r.ThirdPartyId == OtherRequestId).AssociatedAccounts.Count);
    }

    [Fact]
    public async Task ExistingRequest_StatusIsUpdated()
    {
        await _env.Seed(TestData.Request(RequestId, _env.UtcNow.AddDays(-1), TestData.Account(AccountId)));
        _env.GoCardless.Respond(RequisitionsPath,
            Requisitions(Requisition(RequestId, "EX", _env.UtcNow.AddDays(-1), AccountId)));

        await _env.SyncAuthenticationRequests();

        Assert.Equal(AuthenticationStatus.Expired, (await GetRequests()).Single().Status);
    }

    [Fact]
    public async Task RequestsMissingFromTheResponse_AreRemoved()
    {
        await _env.Seed(TestData.Request(RequestId, _env.UtcNow.AddDays(-1)),
            TestData.Request(OtherRequestId, _env.UtcNow.AddDays(-1)));
        _env.GoCardless.Respond(RequisitionsPath, Requisitions(Requisition(RequestId, "LN", _env.UtcNow.AddDays(-1))));

        await _env.SyncAuthenticationRequests();

        Assert.Equal(RequestId, (await GetRequests()).Single().ThirdPartyId);
    }

    [Fact]
    public async Task FreshRequests_AreOnlyFetchedAgainAfterSevenDays()
    {
        _env.GoCardless.Respond(RequisitionsPath, Requisitions(Requisition(RequestId, "LN", _env.UtcNow)));

        await _env.SyncAuthenticationRequests();
        await _env.SyncAuthenticationRequests();
        Assert.Equal(1, _env.GoCardless.RequestCount(RequisitionsPath));

        _env.Time.Advance(TimeSpan.FromDays(7));
        await _env.SyncAuthenticationRequests();
        Assert.Equal(2, _env.GoCardless.RequestCount(RequisitionsPath));
    }

    private async Task<List<AuthenticationRequest>> GetRequests()
    {
        await using var dbContext = _env.CreateDbContext();
        return await dbContext.AuthenticationRequests.Include(request => request.AssociatedAccounts).ToListAsync();
    }
}