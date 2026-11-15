using RobinTTY.PersonalFinanceDashboard.Core.DataProviders;
using RobinTTY.PersonalFinanceDashboard.Core.Models;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Tests.DataProviders;

public class BankDataProviderRegistryTests
{
    [Fact]
    public void TryGet_ReturnsTheProviderOfTheSource()
    {
        var provider = new StubProvider(DataSource.GoCardless);
        var registry = new BankDataProviderRegistry([provider]);

        Assert.True(registry.TryGet(DataSource.GoCardless, out var found));
        Assert.Same(provider, found);
    }

    [Fact]
    public void TryGet_ManualDataHasNoProvider()
    {
        var registry = new BankDataProviderRegistry([new StubProvider(DataSource.GoCardless)]);

        Assert.False(registry.TryGet(DataSource.Manual, out _));
    }

    [Fact]
    public void Constructor_RejectsTwoProvidersForTheSameSource()
    {
        Assert.Throws<ArgumentException>(() => new BankDataProviderRegistry(
            [new StubProvider(DataSource.GoCardless), new StubProvider(DataSource.GoCardless)]));
    }

    [Fact]
    public void Constructor_RejectsAProviderForManualData()
    {
        Assert.Throws<ArgumentException>(() => new BankDataProviderRegistry([new StubProvider(DataSource.Manual)]));
    }

    private sealed class StubProvider(DataSource source) : IBankDataProvider
    {
        public DataSource Source => source;

        public Task<ThirdPartyResponse<BankingInstitution>> GetBankingInstitution(string institutionId,
            CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<ThirdPartyResponse<IReadOnlyList<BankingInstitution>>> GetBankingInstitutions(
            string? country = null, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<ThirdPartyResponse<AuthenticationRequest>> GetAuthenticationRequest(
            string authenticationRequestId, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<ThirdPartyResponse<IReadOnlyList<AuthenticationRequest>>> GetAuthenticationRequests(
            CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<ThirdPartyResponse<AuthenticationRequest>> CreateAuthenticationRequest(string institutionId,
            Uri redirectUri, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<ThirdPartyError?> DeleteAuthenticationRequest(string authenticationRequestId,
            CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<IReadOnlyDictionary<string, ThirdPartyResponse<BankAccount>>> GetBankAccounts(
            IReadOnlyDictionary<string, bool> accounts, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<ThirdPartyResponse<IReadOnlyList<Transaction>>> GetTransactions(string accountId,
            CancellationToken cancellationToken = default) => throw new NotSupportedException();
    }
}