using RobinTTY.PersonalFinanceDashboard.Api.Models;
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
    /// <param name="cancellationToken">A token to signal cancellation.</param>
    public async Task<Transaction> CreateTransaction(TransactionRepository repository, CreateTransactionInput input,
        CancellationToken cancellationToken)
    {
        var transaction = new Transaction
        {
            Source = DataSource.Manual,
            BankTransactionId = null,
            Amount = input.Amount,
            Category = string.IsNullOrWhiteSpace(input.Category) ? null : input.Category,
            Currency = input.Currency,
            Notes = string.IsNullOrWhiteSpace(input.Notes) ? null : input.Notes,
            Payee = input.Payee,
            Payer = input.Payer,
            Description = input.Description,
            ValueDate = input.ValueDate,
            Id = null
        };
        return await repository.AddTransaction(transaction, input.AccountId, cancellationToken)
               ?? throw MutationErrors.NotFound<BankAccount>(input.AccountId, ErrorCodes.BankAccountNotFound);
    }

    /// <summary>
    /// Update the category and notes of an existing transaction.
    /// </summary>
    /// <param name="repository">The injected repository to use for data mutation.</param>
    /// <param name="input">The transaction values to update.</param>
    /// <param name="cancellationToken">A token to signal cancellation.</param>
    public async Task<Transaction> UpdateTransaction(TransactionRepository repository, UpdateTransactionInput input,
        CancellationToken cancellationToken)
    {
        return await repository.UpdateTransaction(input.Id, input.Category, input.Notes, cancellationToken)
               ?? throw MutationErrors.NotFound<Transaction>(input.Id, ErrorCodes.TransactionNotFound);
    }

    /// <summary>
    /// Delete an existing transaction. A synchronized transaction can only be deleted once its account is no longer
    /// synchronized, since the synchronization would recreate it.
    /// </summary>
    /// <param name="repository">The injected repository to use for data mutation.</param>
    /// <param name="transactionId">The id of the transaction to delete.</param>
    /// <param name="cancellationToken">A token to signal cancellation.</param>
    public async Task<DeleteResponse> DeleteTransaction(TransactionRepository repository, Guid transactionId,
        CancellationToken cancellationToken)
    {
        return await repository.DeleteTransaction(transactionId, cancellationToken) switch
        {
            DeleteResult.Deleted => new DeleteResponse { DeletedId = transactionId },
            DeleteResult.Synchronized => throw MutationErrors.Create(
                $"Transaction with id {transactionId} belongs to an account that is still synchronized.",
                ErrorCodes.TransactionSynchronized),
            _ => throw MutationErrors.NotFound<Transaction>(transactionId, ErrorCodes.TransactionNotFound)
        };
    }

    /// <summary>
    /// Synchronizes transaction data with third-party data providers.
    /// </summary>
    /// <param name="synchronizer">Runs the synchronization.</param>
    /// <param name="cancellationToken">A token to signal cancellation.</param>
    /// <returns>A boolean value indicating whether the synchronization was successful.</returns>
    public async Task<bool> SynchronizeTransactionData(IDataSynchronizer synchronizer,
        CancellationToken cancellationToken)
    {
        return await synchronizer.Synchronize(ThirdPartyDataType.Transactions, forceThirdPartySync: true,
            cancellationToken);
    }
}
