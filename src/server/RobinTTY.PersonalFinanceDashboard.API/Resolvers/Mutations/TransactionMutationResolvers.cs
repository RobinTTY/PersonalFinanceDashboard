using HotChocolate.Types;
using RobinTTY.PersonalFinanceDashboard.Api.Resolvers.Inputs;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Repositories;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Services.DataSynchronization.Interfaces;

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
            BankTransactionId = null,
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
    
    /// <summary>
    /// Synchronizes transaction data with third-party data providers.
    /// </summary>
    /// <param name="syncHandler">The handler responsible for managing the data synchronization process.</param>
    /// <returns>A boolean value indicating whether the synchronization was successful.</returns>
    public async Task<bool> SynchronizeTransactionData(ITransactionSyncHandler syncHandler)
    {
        return await syncHandler.SynchronizeData(forceThirdPartySync: true);
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
