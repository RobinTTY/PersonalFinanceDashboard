using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueIndexToTransactionThirdPartyId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Transactions_BankAccountId",
                table: "Transactions");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_BankAccountId_ThirdPartyId",
                table: "Transactions",
                columns: new[] { "BankAccountId", "ThirdPartyId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Transactions_BankAccountId_ThirdPartyId",
                table: "Transactions");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_BankAccountId",
                table: "Transactions",
                column: "BankAccountId");
        }
    }
}
