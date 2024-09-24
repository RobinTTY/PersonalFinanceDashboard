using HotChocolate.Types;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Repositories;

namespace RobinTTY.PersonalFinanceDashboard.Api.Types.Mutations;

/// <summary>
/// <see cref="Transaction"/> related mutation resolvers.
/// </summary>
[MutationType]
public class TransactionMutationResolvers
{
    /// <summary>
    /// Create a new transaction.
    /// </summary>
    /// <param name="transaction">The transaction to create.</param>
    /// <param name="repository">The repository to use for data mutation.</param>
    /// TODO: Change param to CreateTransactionInput type
    public async Task<Transaction> CreateTransaction(Transaction transaction, TransactionRepository repository)
    {
        await repository.Add(transaction);
        return transaction;
    }

    /// <summary>
    /// Delete an existing transaction.
    /// </summary>
    /// <param name="transactionId">The id of the transaction to delete.</param>
    /// <param name="repository">The repository to use for data mutation.</param>
    public async Task<Response> DeleteTransaction(string transactionId, TransactionRepository repository)
    {
        var result = await repository.Delete(transactionId);
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