using Microsoft.EntityFrameworkCore;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Tests.TestUtilities;
using RobinTTY.PersonalFinanceDashboard.TestUtilities;
using static RobinTTY.PersonalFinanceDashboard.TestUtilities.GoCardlessJson;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Tests.DataSynchronization;

public sealed class AuthenticationRequestSyncHandlerTests : IDisposable
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
        Assert.Equal([AccountId.ToString(), OtherAccountId.ToString()],
            request.AssociatedAccounts.Select(a => a.ExternalId).Order());
    }

    [Fact]
    public async Task Requests_AreLinkedToTheirInstitution()
    {
        // E.g. a request created before institutions were stored on it
        await _env.Seed(TestData.Institution("SANDBOXFINANCE_SFIN0000"),
            TestData.Request(RequestId, _env.UtcNow.AddDays(-1)));
        _env.GoCardless.Respond(RequisitionsPath, Requisitions(
            Requisition(RequestId, "LN", _env.UtcNow.AddDays(-1)),
            Requisition(OtherRequestId, "LN", _env.UtcNow.AddDays(-1))));

        Assert.True(await _env.SyncAuthenticationRequests());

        Assert.All(await GetRequests(),
            request => Assert.Equal("SANDBOXFINANCE_SFIN0000", request.Institution?.ExternalId));
    }

    [Fact]
    public async Task UnknownInstitution_IsNotAdded()
    {
        _env.GoCardless.Respond(RequisitionsPath,
            Requisitions(Requisition(RequestId, "LN", _env.UtcNow.AddDays(-1))));

        Assert.True(await _env.SyncAuthenticationRequests());

        Assert.Null((await GetRequests()).Single().Institution);
        await using var dbContext = _env.CreateDbContext();
        Assert.Empty(dbContext.BankingInstitutions.Where(institution => institution.Source == DataSource.GoCardless));
    }

    [Fact]
    public async Task AccountsOfExpiredRequests_AreNotAdded()
    {
        // E.g. the old ids of accounts that were taken over by a reconnect
        _env.GoCardless.Respond(RequisitionsPath,
            Requisitions(Requisition(RequestId, "EX", _env.UtcNow.AddDays(-100), AccountId)));

        Assert.True(await _env.SyncAuthenticationRequests());

        Assert.Empty((await GetRequests()).Single().AssociatedAccounts);
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
        Assert.Single(requests.Single(r => r.ExternalId == RequestId.ToString()).AssociatedAccounts);
        Assert.Equal(2, requests.Single(r => r.ExternalId == OtherRequestId.ToString()).AssociatedAccounts.Count);
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
    public async Task UndocumentedStatus_IsStoredAsUnknown()
    {
        _env.GoCardless.Respond(RequisitionsPath,
            Requisitions(Requisition(RequestId, "XY", _env.UtcNow.AddDays(-1)),
                Requisition(OtherRequestId, "LN", _env.UtcNow.AddDays(-1))));

        Assert.True(await _env.SyncAuthenticationRequests());

        var requests = await GetRequests();
        Assert.Equal(AuthenticationStatus.Unknown, requests.Single(r => r.ExternalId == RequestId.ToString()).Status);
        Assert.Equal(AuthenticationStatus.Active,
            requests.Single(r => r.ExternalId == OtherRequestId.ToString()).Status);
    }

    [Fact]
    public async Task ExistingRequest_KeepsItsAccessValidity()
    {
        // GoCardless only returns the agreement's id, the validity is retrieved when the request is created
        var request = TestData.Request(RequestId, _env.UtcNow.AddDays(-1), TestData.Account(AccountId));
        request.AccessValidForDays = 30;
        await _env.Seed(request);
        _env.GoCardless.Respond(RequisitionsPath,
            Requisitions(Requisition(RequestId, "LN", _env.UtcNow.AddDays(-1), AccountId)));

        await _env.SyncAuthenticationRequests();

        Assert.Equal(30, (await GetRequests()).Single().AccessValidForDays);
    }

    [Fact]
    public async Task RequestsMissingFromTheResponse_AreRemoved()
    {
        await _env.Seed(TestData.Request(RequestId, _env.UtcNow.AddDays(-1)),
            TestData.Request(OtherRequestId, _env.UtcNow.AddDays(-1)));
        _env.GoCardless.Respond(RequisitionsPath, Requisitions(Requisition(RequestId, "LN", _env.UtcNow.AddDays(-1))));

        await _env.SyncAuthenticationRequests();

        Assert.Equal(RequestId.ToString(), (await GetRequests()).Single().ExternalId);
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
        return await dbContext.AuthenticationRequests
            .Include(request => request.AssociatedAccounts)
            .Include(request => request.Institution)
            .ToListAsync();
    }
}
