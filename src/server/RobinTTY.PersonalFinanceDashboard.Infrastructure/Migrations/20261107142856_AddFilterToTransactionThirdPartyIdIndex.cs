using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFilterToTransactionThirdPartyIdIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Transactions_BankAccountId_ThirdPartyId",
                table: "Transactions");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_BankAccountId_ThirdPartyId",
                table: "Transactions",
                columns: new[] { "BankAccountId", "ThirdPartyId" },
                unique: true,
                filter: "\"ThirdPartyId\" <> '00000000-0000-0000-0000-000000000000'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Transactions_BankAccountId_ThirdPartyId",
                table: "Transactions");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_BankAccountId_ThirdPartyId",
                table: "Transactions",
                columns: new[] { "BankAccountId", "ThirdPartyId" },
                unique: true);
        }
    }
}
