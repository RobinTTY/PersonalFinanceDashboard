using Riok.Mapperly.Abstractions;
using RobinTTY.PersonalFinanceDashboard.API.EfModels;
using RobinTTY.PersonalFinanceDashboard.Core.Models;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Mappers;

[Mapper]
public partial class TransactionMapper
{
    public partial Transaction TransactionEntityToTransaction(TransactionEntity transaction);
    public partial TransactionEntity TransactionToTransactionEntity(Transaction transaction);
}
