using RobinTTY.NordigenApiClient;
using RobinTTY.NordigenApiClient.Models.Errors;
using RobinTTY.NordigenApiClient.Models.Responses;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using BankAccount = RobinTTY.PersonalFinanceDashboard.Core.Models.BankAccount;
using Transaction = RobinTTY.PersonalFinanceDashboard.Core.Models.Transaction;

namespace RobinTTY.PersonalFinanceDashboard.ThirdPartyDataProviders;

/// <summary>
/// Data provider for banking data from <see href="https://gocardless.com/bank-account-data/"/>.
/// </summary>
// TODO: Probably should already return a generic ThirdPartyError (type) here
// TODO: These methods need to be reworked quite a bit
public class GoCardlessDataProviderService(NordigenClient client)
{
    public async Task<ThirdPartyResponse<BankingInstitution?, BasicResponse>> GetBankingInstitution(
        string institutionId, CancellationToken cancellationToken = default)
    {
        var response = await client.InstitutionsEndpoint.GetInstitution(institutionId, cancellationToken);

        // TODO: Maybe don't transform the data here but when it's added to db, so we don't need to add the GUID
        BankingInstitution? result = null;
        if (response.IsSuccess)
            result = new BankingInstitution(response.Result.Id, response.Result.Bic, response.Result.Name,
                response.Result.Logo, response.Result.Countries)
            {
                Id = Guid.NewGuid()
            };
        return new ThirdPartyResponse<BankingInstitution?, BasicResponse>(response.IsSuccess, result, response.Error);
    }

    /// <summary>
    /// Gets the banking institutions which are available via this data provider.
    /// </summary>
    /// <param name="country">Optional country filter to apply to the query.</param>
    /// <param name="cancellationToken">An optional token to signal cancellation.</param>
    /// <returns>The available banking <see cref="Institution"/>s.</returns>
    public async Task<ThirdPartyResponse<IEnumerable<BankingInstitution>, BasicResponse>> GetBankingInstitutions(
        string? country = null, CancellationToken cancellationToken = default)
    {
        var response = await client.InstitutionsEndpoint.GetInstitutions(country, cancellationToken: cancellationToken);

        if (response.IsSuccess)
        {
            // TODO: Maybe don't transform the data here but when it's added to db, so we don't need to add the GUID
            var institutions =
                response.Result.Select(inst =>
                    new BankingInstitution(inst.Id, inst.Bic, inst.Name, inst.Logo, inst.Countries)
                    {
                        Id = Guid.NewGuid()
                    });
            return new ThirdPartyResponse<IEnumerable<BankingInstitution>, BasicResponse>(true,
                institutions, null);
        }

        return new ThirdPartyResponse<IEnumerable<BankingInstitution>, BasicResponse>(false, null, response.Error);
    }

    /// <summary>
    /// Gets an existing authentication request (requisitions).
    /// </summary>
    /// <param name="requisitionId">The institutionId of the requisition to be retrieved.</param>
    /// <param name="cancellationToken">An optional token to signal cancellation.</param>
    /// <returns>The <see cref="AuthenticationRequest"/> which matches the institutionId, as far as it exists.</returns>
    public async Task<ThirdPartyResponse<AuthenticationRequest?, BasicResponse?>> GetAuthenticationRequest(
        string requisitionId, CancellationToken cancellationToken = default)
    {
        var response = await client.RequisitionsEndpoint.GetRequisition(requisitionId, cancellationToken);
        // TODO: handle request failure
        var requisition = response.Result!;
        var result = new AuthenticationRequest(requisition.Id, requisition.Status.ToAuthenticationStatus(),
            requisition.AuthenticationLink, requisition.Accounts.Select(accountId => new BankAccount(accountId)
            {
                Id = null
            }).ToList())
        {
            Id = null
        };
        return new ThirdPartyResponse<AuthenticationRequest?, BasicResponse?>(response.IsSuccess, result,
            response.Error);
    }

    // TODO: Cancellation token support
    /// <summary>
    /// Gets existing authentication requests (requisitions) for all banking institutions.
    /// </summary>
    /// <param name="requisitionLimit">The maximum number of requisitions to get.</param>
    /// <param name="cancellationToken">An optional token to signal cancellation.</param>
    /// <returns>All existing <see cref="Requisition"/>s.</returns>
    public async Task<ThirdPartyResponse<IEnumerable<AuthenticationRequest>, BasicResponse?>> GetAuthenticationRequests(
        int requisitionLimit, CancellationToken cancellationToken = default)
    {
        var response = await client.RequisitionsEndpoint.GetRequisitions(requisitionLimit, 0, cancellationToken);
        // TODO: handle request failure
        var requisitions = response.Result!.Results;
        var result = requisitions.Select(req => new AuthenticationRequest(req.Id,
            req.Status.ToAuthenticationStatus(), req.AuthenticationLink, req.Accounts.Select(accountId =>
                new BankAccount(accountId)
                {
                    Id = null
                }).ToList())
        {
            Id = null
        });
        return new ThirdPartyResponse<IEnumerable<AuthenticationRequest>, BasicResponse?>(response.IsSuccess, result,
            response.Error);
    }

    /// <summary>
    /// Creates a new authentication request (requisition) for the chosen institution and returns it.
    /// </summary>
    /// <param name="institutionId">Id of the institution for which to create the requisition.</param>
    /// <param name="redirectUri"><see cref="Uri"/> which will be redirected too when the user completes authentication for the given requisition.</param>
    /// <param name="cancellationToken">An optional token to signal cancellation.</param>
    /// <returns>The created <see cref="Requisition"/>.</returns>
    public async Task<ThirdPartyResponse<AuthenticationRequest, CreateRequisitionError>> CreateAuthenticationRequest(
        string institutionId, Uri redirectUri, CancellationToken cancellationToken = default)
    {
        var response =
            await client.RequisitionsEndpoint.CreateRequisition(institutionId, redirectUri,
                reference: Guid.NewGuid().ToString(), cancellationToken: cancellationToken);

        if (response.IsSuccess)
        {
            var requisition = response.Result;
            var authenticationRequest = new AuthenticationRequest(requisition.Id,
                requisition.Status.ToAuthenticationStatus(), requisition.AuthenticationLink,
                requisition.Accounts.Select(accountId => new BankAccount(accountId)
                {
                    Id = null
                }).ToList())
            {
                Id = null
            };

            return new ThirdPartyResponse<AuthenticationRequest, CreateRequisitionError>(response.IsSuccess,
                authenticationRequest, null);
        }

        return new ThirdPartyResponse<AuthenticationRequest, CreateRequisitionError>(response.IsSuccess, null,
            response.Error);
    }

    public async Task<ThirdPartyResponse<BasicResponse, BasicResponse>> DeleteAuthenticationRequest(
        Guid authenticationId, CancellationToken cancellationToken = default)
    {
        var response = await client.RequisitionsEndpoint.DeleteRequisition(authenticationId, cancellationToken);
        return new ThirdPartyResponse<BasicResponse, BasicResponse>(response.IsSuccess, response.Result,
            response.Error);
    }

    public async Task<List<ThirdPartyResponse<BankAccount, AccountsError>>> GetBankAccounts(
        IEnumerable<Guid> accountIds, CancellationToken cancellationToken = default)
    {
        var tasks = accountIds.Select(accountId => GetBankAccount(accountId, cancellationToken)).ToList();
        await Task.WhenAll(tasks);
        return tasks.Select(task => task.Result).ToList();
    }

    public async Task<ThirdPartyResponse<IEnumerable<Transaction>, AccountsError>> GetTransactions(Guid accountId,
        CancellationToken cancellationToken = default)
    {
        var response = await client.AccountsEndpoint.GetTransactions(accountId, cancellationToken: cancellationToken);
        // TODO: Also return pending transactions
        var transactions = response.Result!.BookedTransactions.Select(transaction =>
            new Transaction(Guid.NewGuid(), Guid.Parse(transaction.InternalTransactionId), transaction.TransactionId ?? null, accountId,
                transaction.ValueDateTime ?? transaction.ValueDate,
                transaction.CreditorName, transaction.DebtorName, transaction.TransactionAmount.Amount,
                transaction.TransactionAmount.Currency,
                "example-category", "example-notes", [])
            {
                Id = null
            });
        return new ThirdPartyResponse<IEnumerable<Transaction>, AccountsError>(response.IsSuccess, transactions,
            response.Error);
    }

    private async Task<ThirdPartyResponse<BankAccount, AccountsError>> GetBankAccount(Guid accountId,
        CancellationToken cancellationToken = default)
    {
        var generalAccountInfoTask = client.AccountsEndpoint.GetAccount(accountId, cancellationToken);
        var accountDetailsTask = client.AccountsEndpoint.GetAccountDetails(accountId, cancellationToken);
        var balanceTask = client.AccountsEndpoint.GetBalances(accountId, cancellationToken);

        var generalAccountInfoResponse = await generalAccountInfoTask;
        var accountDetailsResponse = await accountDetailsTask;
        var balanceResponse = await balanceTask;

        if (!accountDetailsResponse.IsSuccess || !balanceResponse.IsSuccess || !generalAccountInfoResponse.IsSuccess)
            return new ThirdPartyResponse<BankAccount, AccountsError>(
                false, null, accountDetailsResponse.Error);

        var generalAccountInfoResult = generalAccountInfoResponse.Result;
        var accountDetailsResult = accountDetailsResponse.Result;
        var balanceResult = balanceResponse.Result;
        var bankAccount = GoCardlessTypeExtensions.CreateBankAccount(accountId, generalAccountInfoResult,
            accountDetailsResult, balanceResult);


        return new ThirdPartyResponse<BankAccount, AccountsError>(
            accountDetailsResponse.IsSuccess && balanceResponse.IsSuccess, bankAccount, null);
    }
}