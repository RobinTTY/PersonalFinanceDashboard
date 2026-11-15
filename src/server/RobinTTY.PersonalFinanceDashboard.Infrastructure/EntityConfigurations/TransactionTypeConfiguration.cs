using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RobinTTY.PersonalFinanceDashboard.Core.Models;

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.EntityConfigurations;

public class TransactionTypeConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.HasOne(transaction => transaction.BankAccount)
            .WithMany(account => account.Transactions)
            .OnDelete(DeleteBehavior.Cascade);
        // Scoped to the bank account, since external transaction ids are only unique per account
        builder.ConfigureSource("BankAccountId");
    }
}