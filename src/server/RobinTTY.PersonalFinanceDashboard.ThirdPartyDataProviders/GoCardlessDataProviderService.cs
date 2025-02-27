﻿using RobinTTY.NordigenApiClient;
using RobinTTY.NordigenApiClient.Models.Errors;
using RobinTTY.NordigenApiClient.Models.Responses;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using BankAccount = RobinTTY.PersonalFinanceDashboard.Core.Models.BankAccount;
using Guid = System.Guid;
using Transaction = RobinTTY.PersonalFinanceDashboard.Core.Models.Transaction;

namespace RobinTTY.PersonalFinanceDashboard.ThirdPartyDataProviders;

/// <summary>
/// Data provider for banking data from <see href="https://gocardless.com/bank-account-data/"/>.
/// </summary>
// TODO: Probably should already return a generic ThirdPartyError (type) here
// TODO: Cancellation token support
public class GoCardlessDataProviderService(NordigenClient client)
{
    public async Task<ThirdPartyResponse<BankingInstitution?, BasicResponse>> GetBankingInstitution(
        string institutionId)
    {
        var response = await client.InstitutionsEndpoint.GetInstitution(institutionId);
        
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
    /// <returns>The available banking <see cref="Institution"/>s.</returns>
    public async Task<ThirdPartyResponse<IEnumerable<BankingInstitution>, BasicResponse>> GetBankingInstitutions(
        string? country = null)
    {
        var response = await client.InstitutionsEndpoint.GetInstitutions(country);

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
    /// <returns>The <see cref="AuthenticationRequest"/> which matches the institutionId, as far as it exists.</returns>
    public async Task<ThirdPartyResponse<AuthenticationRequest?, BasicResponse?>> GetAuthenticationRequest(
        string requisitionId)
    {
        var response = await client.RequisitionsEndpoint.GetRequisition(requisitionId);
        // TODO: handle request failure
        var requisition = response.Result!;
        var result = new AuthenticationRequest(requisition.Id,
            ConvertRequisitionStatus(requisition.Status), requisition.AuthenticationLink,
            requisition.Accounts.Select(accountId => new BankAccount(accountId)
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
    /// <returns>All existing <see cref="Requisition"/>s.</returns>
    public async Task<ThirdPartyResponse<IEnumerable<AuthenticationRequest>, BasicResponse?>> GetAuthenticationRequests(
        int requisitionLimit)
    {
        var response = await client.RequisitionsEndpoint.GetRequisitions(requisitionLimit, 0);
        // TODO: handle request failure
        var requisitions = response.Result!.Results;
        var result = requisitions.Select(req => new AuthenticationRequest(req.Id,
            ConvertRequisitionStatus(req.Status),
            req.AuthenticationLink, req.Accounts.Select(accountId => new BankAccount(accountId)
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
    /// <returns>The created <see cref="Requisition"/>.</returns>
    public async Task<ThirdPartyResponse<AuthenticationRequest, CreateRequisitionError>> CreateAuthenticationRequest(
        string institutionId, Uri redirectUri)
    {
        var response =
            await client.RequisitionsEndpoint.CreateRequisition(institutionId, redirectUri,
                reference: Guid.NewGuid().ToString());

        if (response.IsSuccess)
        {
            var requisition = response.Result;
            var authenticationRequest = new AuthenticationRequest(requisition.Id,
                ConvertRequisitionStatus(requisition.Status), requisition.AuthenticationLink,
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
        Guid authenticationId)
    {
        var response = await client.RequisitionsEndpoint.DeleteRequisition(authenticationId);
        return new ThirdPartyResponse<BasicResponse, BasicResponse>(response.IsSuccess, response.Result,
            response.Error);
    }

    public async Task<ThirdPartyResponse<BankAccount, AccountsError>> GetBankAccount(Guid accountId)
    {
        var accountDetailsTask = client.AccountsEndpoint.GetAccountDetails(accountId);
        var balanceTask = client.AccountsEndpoint.GetBalances(accountId);

        // TODO: Handle possible null values
        var accountDetailsRequest = await accountDetailsTask;
        var balanceRequest = await balanceTask;
        var accountDetailsResult = accountDetailsRequest.Result!;
        var balanceResult = balanceRequest.Result!;
        var bankAccount = ConvertAccount(accountId, accountDetailsResult, balanceResult);

        return new ThirdPartyResponse<BankAccount, AccountsError>(
            accountDetailsRequest.IsSuccess && balanceRequest.IsSuccess, bankAccount, accountDetailsRequest.Error);
    }

    public async Task<List<ThirdPartyResponse<BankAccount, AccountsError>>> GetBankAccounts(
        IEnumerable<Guid> accountIds)
    {
        var tasks = accountIds.Select(GetBankAccount).ToList();
        await Task.WhenAll(tasks);
        return tasks.Select(task => task.Result).ToList();
    }

    public async Task<ThirdPartyResponse<IEnumerable<Transaction>, AccountsError>> GetTransactions(Guid accountId)
    {
        var response = await client.AccountsEndpoint.GetTransactions(accountId);
        // TODO: Also return pending transactions
        var transactions = response.Result!.BookedTransactions.Select(transaction =>
            new Transaction(Guid.NewGuid(), transaction.InternalTransactionId ?? Guid.NewGuid().ToString(), accountId,
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

    private AuthenticationStatus ConvertRequisitionStatus(RequisitionStatus status)
    {
        return status switch
        {
            RequisitionStatus.Created => AuthenticationStatus.RequiresUserAction,
            RequisitionStatus.GivingConsent => AuthenticationStatus.RequiresUserAction,
            RequisitionStatus.UndergoingAuthentication => AuthenticationStatus.RequiresUserAction,
            RequisitionStatus.SelectingAccounts => AuthenticationStatus.RequiresUserAction,
            RequisitionStatus.GrantingAccess => AuthenticationStatus.RequiresUserAction,
            RequisitionStatus.Undefined => AuthenticationStatus.Failed,
            RequisitionStatus.Suspended => AuthenticationStatus.Failed,
            RequisitionStatus.Rejected => AuthenticationStatus.Failed,
            RequisitionStatus.Linked => AuthenticationStatus.Successful,
            RequisitionStatus.Expired => AuthenticationStatus.Expired,
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
        };
    }

    private BankAccount ConvertAccount(Guid accountId, BankAccountDetails account, List<Balance> balances)
    {
        return new BankAccount
        (
            // TODO: Convert Account
            thirdPartyId: accountId,
            accountType: account.CashAccountType.HasValue
                ? Enum.GetName(typeof(CashAccountType), account.CashAccountType)
                : null,
            name: account.Product,
            description: account.Details,
            // TODO: The balance type could be something different (which one should be grabbed?)
            balance: balances.First(bal =>
                    bal.BalanceType is BalanceType.ClosingBooked or BalanceType.InterimAvailable)
                .BalanceAmount.Amount,
            currency: account.Currency,
            iban: account.Iban,
            bic: account.Bic,
            bban: account.Bban,
            ownerName: account.OwnerName,
            transactions: [],
            associatedInstitution: new BankingInstitution("id", "bic", "name", new Uri("http://www.example.com"), [])
            {
                Id = Guid.Empty
            },
            associatedAuthenticationRequests: (List<AuthenticationRequest>) [])
        {
            Id = null
        };
    }
}
