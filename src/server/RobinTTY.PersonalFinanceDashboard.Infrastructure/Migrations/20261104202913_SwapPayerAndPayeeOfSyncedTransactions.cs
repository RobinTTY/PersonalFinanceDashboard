using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Migrations
{
    /// <summary>
    /// Transactions synced from GoCardless used to store the creditor as payer and the debtor as payee.
    /// The mapping is fixed, but re-syncing only corrects transactions GoCardless still returns
    /// (usually ~90 days), so this swaps the values of all previously synced transactions.
    /// Only synced transactions are linked to a bank account, manually created ones are left untouched.
    /// </summary>
    public partial class SwapPayerAndPayeeOfSyncedTransactions : Migration
    {
        // SQLite evaluates every right-hand side against the original row, so this swaps the two columns
        private const string SwapPayerAndPayeeSql =
            "UPDATE Transactions SET Payer = Payee, Payee = Payer WHERE BankAccountId IS NOT NULL;";

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(SwapPayerAndPayeeSql);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(SwapPayerAndPayeeSql);
        }
    }
}
