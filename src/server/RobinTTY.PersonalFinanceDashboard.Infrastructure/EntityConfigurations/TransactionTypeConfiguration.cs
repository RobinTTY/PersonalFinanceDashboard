using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RobinTTY.PersonalFinanceDashboard.Core.Models;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.EntityConfigurations;

public class TransactionTypeConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        // Scoped to the bank account so manually created transactions (no account, empty third party id) don't
        // conflict, since SQLite treats NULL values as distinct
        builder.HasIndex("BankAccountId", nameof(Transaction.ThirdPartyId)).IsUnique();
    }
}