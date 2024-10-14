using HotChocolate.Types;
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
    /// <param name="transaction">The transaction to create.</param>
    /// TODO: Change param to CreateTransactionInput type
    public async Task<Transaction> CreateTransaction(TransactionRepository repository, Transaction transaction)
    {
        await repository.AddTransaction(transaction);
        return transaction;
    }

    /// <summary>
    /// Delete an existing transaction.
    /// </summary>
    /// <param name="repository">The injected repository to use for data mutation.</param>
    /// <param name="transactionId">The id of the transaction to delete.</param>
    public async Task<Response> DeleteTransaction(TransactionRepository repository, string transactionId)
    {
        var result = await repository.DeleteTransaction(transactionId);
        // TODO: error if result false
        return new Response {Id = transactionId};
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
    public required string Id { get; set; }
}
