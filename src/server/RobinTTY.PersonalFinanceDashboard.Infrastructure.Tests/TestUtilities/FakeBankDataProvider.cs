using System.Collections.Concurrent;
using RobinTTY.PersonalFinanceDashboard.Core.DataProviders;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.TestUtilities;
using RobinTTY.PersonalFinanceDashboard.ThirdPartyDataProviders;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Tests.TestUtilities;

/// <summary>
/// An in-memory <see cref="IBankDataProvider"/> for sync handler tests that don't need to exercise the GoCardless
/// mapping. Calls without a configured response fail with <see cref="NotFound"/>.
/// </summary>
internal sealed class FakeBankDataProvider : IBankDataProvider
{
    public static readonly ThirdPartyError NotFound = new("Not found", "No fake response configured");

    private readonly ConcurrentDictionary<string, Func<object>> _responses = new();
    private readonly ConcurrentQueue<string> _calls = new();

    public DataSource Source => DataSource.GoCardless;

    public int TotalCallCount => _calls.Count;

    /// <summary>
    /// The number of calls of a provider method, e.g. <c>nameof(IBankDataProvider.GetTransactions)</c>.
    /// </summary>
    /// <param name="method">The name of the method.</param>
    /// <param name="id">The provider's id the method was called with, if it takes one.</param>
    public int CallCount(string method, string? id = null) => _calls.Count(call => call == Key(method, id));

    /// <summary>
    /// Sets the result of a provider method. It's created on each call, since the handlers add the returned
    /// entities to their context.
    /// </summary>
    /// <param name="method">The name of the method, e.g. <c>nameof(IBankDataProvider.GetTransactions)</c>.</param>
    /// <param name="id">The provider's id the method is called with, if it takes one.</param>
    /// <param name="result">Creates the result.</param>
    public void Respond<TResult>(string method, string? id, Func<TResult> result) where TResult : notnull =>
        _responses[Key(method, id)] = () => result();

    /// <summary>
    /// Lets calls of a provider method fail with <paramref name="error"/>.
    /// </summary>
    public void Fail(string method, string? id, ThirdPartyError error) => _responses[Key(method, id)] = () => error;

    public void RespondTransactions(Guid accountId, params (Guid Id, decimal Amount)[] transactions) =>
        Respond(nameof(GetTransactions), accountId.ToExternalId(), IReadOnlyList<Transaction> () => transactions
            .Select(transaction => TestData.Transaction(transaction.Id,
                new BankAccount(Source, accountId.ToExternalId()), transaction.Amount))
            .ToList());

    public Task<ThirdPartyResponse<BankingInstitution>> GetBankingInstitution(string institutionId,
        CancellationToken cancellationToken = default) =>
        Call<BankingInstitution>(nameof(GetBankingInstitution), institutionId);

    public Task<ThirdPartyResponse<IReadOnlyList<BankingInstitution>>> GetBankingInstitutions(
        string? country = null, CancellationToken cancellationToken = default) =>
        Call<IReadOnlyList<BankingInstitution>>(nameof(GetBankingInstitutions));

    public Task<ThirdPartyResponse<AuthenticationRequest>> GetAuthenticationRequest(string authenticationRequestId,
        CancellationToken cancellationToken = default) =>
        Call<AuthenticationRequest>(nameof(GetAuthenticationRequest), authenticationRequestId);

    public Task<ThirdPartyResponse<IReadOnlyList<AuthenticationRequest>>> GetAuthenticationRequests(
        CancellationToken cancellationToken = default) =>
        Call<IReadOnlyList<AuthenticationRequest>>(nameof(GetAuthenticationRequests));

    public Task<ThirdPartyResponse<AuthenticationRequest>> CreateAuthenticationRequest(string institutionId,
        Uri redirectUri, CancellationToken cancellationToken = default) =>
        Call<AuthenticationRequest>(nameof(CreateAuthenticationRequest), institutionId);

    public async Task<ThirdPartyError?> DeleteAuthenticationRequest(string authenticationRequestId,
        CancellationToken cancellationToken = default) =>
        (await Call<bool>(nameof(DeleteAuthenticationRequest), authenticationRequestId)).Error;

    public async Task<IReadOnlyDictionary<string, ThirdPartyResponse<BankAccount>>> GetBankAccounts(
        IReadOnlyDictionary<string, bool> accounts, CancellationToken cancellationToken = default)
    {
        var responses = new Dictionary<string, ThirdPartyResponse<BankAccount>>();
        foreach (var accountId in accounts.Keys)
            responses[accountId] = await Call<BankAccount>(nameof(GetBankAccounts), accountId);

        return responses;
    }

    public Task<ThirdPartyResponse<IReadOnlyList<Transaction>>> GetTransactions(string accountId,
        DateTime? dateFrom = null, CancellationToken cancellationToken = default) =>
        Call<IReadOnlyList<Transaction>>(nameof(GetTransactions), accountId);

    private Task<ThirdPartyResponse<TResult>> Call<TResult>(string method, string? id = null)
        where TResult : notnull
    {
        var key = Key(method, id);
        _calls.Enqueue(key);

        var response = !_responses.TryGetValue(key, out var respond)
            ? ThirdPartyResponse<TResult>.Failure(NotFound)
            : respond() switch
            {
                ThirdPartyError error => ThirdPartyResponse<TResult>.Failure(error),
                var result => ThirdPartyResponse<TResult>.Success((TResult)result)
            };
        return Task.FromResult(response);
    }

    private static string Key(string method, string? id) => id is null ? method : $"{method}:{id}";
}
