using Riok.Mapperly.Abstractions;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Entities;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Mappers;

[Mapper]
public partial class TransactionMapper
{
    public partial Transaction TransactionEntityToTransaction(TransactionEntity transaction);
    public partial TransactionEntity TransactionToTransactionEntity(Transaction transaction);
}
