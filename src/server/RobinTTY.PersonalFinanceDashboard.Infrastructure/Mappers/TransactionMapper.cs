﻿using Riok.Mapperly.Abstractions;
using RobinTTY.PersonalFinanceDashboard.Core.Models;
using RobinTTY.PersonalFinanceDashboard.Infrastructure.Entities;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Mappers;

/// <summary>
/// TODO
/// </summary>
[Mapper]
public partial class TransactionMapper
{
    public partial Transaction EntityToModel(TransactionEntity transaction);
    public partial TransactionEntity ModelToEntity(Transaction transaction);
}
