using HotChocolate.Types;
using RobinTTY.PersonalFinanceDashboard.Api.Resolvers.Inputs;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Repositories;

namespace RobinTTY.PersonalFinanceDashboard.Api.Resolvers.Mutations;

/// <summary>
/// <see cref="Transaction"/> related mutation resolvers.
/// </summary>
[MutationType]
public class TransactionMutationResolvers
{
    /// <summary>
    /// Create a new transaction.
    /// </summary>
    /// <param name="repository">The injected repository to use for data mutation.</param>
    /// <param name="input">The transaction to create.</param>
    public async Task<Transaction> CreateTransaction(TransactionRepository repository, CreateTransactionInput input)
    {
        var transaction = new Transaction
        {
            ThirdPartyTransactionId = null,
            AccountId = input.AccountId,
            Amount = input.Amount,
            Category = input.Category,
            Currency = input.Currency,
            Notes = input.Notes,
            Payee = input.Payee,
            Payer = input.Payer,
            ValueDate = input.ValueDate,
            Id = null
        };
        await repository.AddTransaction(transaction);
        return transaction;
    }

    /// <summary>
    /// Update an existing transaction.
    /// </summary>
    /// <param name="repository">The injected repository to use for data retrieval.</param>
    /// <param name="transaction">The transaction to update.</param>
    public async Task<Transaction> UpdateTransaction(TransactionRepository repository,
        Transaction transaction)
    {
        return await repository.UpdateTransaction(transaction);
    }

    /// <summary>
    /// Delete an existing transaction.
    /// </summary>
    /// <param name="repository">The injected repository to use for data mutation.</param>
    /// <param name="transactionId">The id of the transaction to delete.</param>
    public async Task<Response> DeleteTransaction(TransactionRepository repository, Guid transactionId)
    {
        var result = await repository.DeleteTransaction(transactionId);
        // TODO: error if result false
        return new Response {Id = Guid.NewGuid()};
    }
}

/// <summary>
/// TODO
/// </summary>
public class Response
{
    /// <summary>
    /// TODO
    /// </summary>
    public required Guid Id { get; set; }
}
