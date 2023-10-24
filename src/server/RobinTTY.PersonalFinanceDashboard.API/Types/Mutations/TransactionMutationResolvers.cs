using System.Threading.Tasks;
using HotChocolate.Types;
using RobinTTY.PersonalFinanceDashboard.API.Repositories;
using RobinTTY.PersonalFinanceDashboard.Core.Models;

namespace RobinTTY.PersonalFinanceDashboard.Api.Types.Mutations;

[MutationType]
public class TransactionMutationResolvers
{
    /// <summary>
    /// Create a new transaction.
    /// </summary>
    /// TODO: Change param to CreateTransactionInput type
    public async Task<Transaction> CreateTransaction(Transaction transaction, TransactionRepository repository)
    {
        await repository.Add(transaction);
        return transaction;
    }

    public async Task<Response> DeleteTransaction(string transactionId, TransactionRepository repository)
    {
        var result = await repository.Delete(transactionId);
        // TODO: error if result false
        return new Response {Id = transactionId};
    }
}

public class Response
{
    public string Id { get; set; }
}