using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ExcludeManualEntitiesFromThirdPartyIdIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BankingInstitutions_ThirdPartyId",
                table: "BankingInstitutions");

            migrationBuilder.DropIndex(
                name: "IX_BankAccounts_ThirdPartyId",
                table: "BankAccounts");

            migrationBuilder.CreateIndex(
                name: "IX_BankingInstitutions_ThirdPartyId",
                table: "BankingInstitutions",
                column: "ThirdPartyId",
                unique: true,
                filter: "\"ThirdPartyId\" <> ''");

            migrationBuilder.CreateIndex(
                name: "IX_BankAccounts_ThirdPartyId",
                table: "BankAccounts",
                column: "ThirdPartyId",
                unique: true,
                filter: "\"ThirdPartyId\" <> '00000000-0000-0000-0000-000000000000'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BankingInstitutions_ThirdPartyId",
                table: "BankingInstitutions");

            migrationBuilder.DropIndex(
                name: "IX_BankAccounts_ThirdPartyId",
                table: "BankAccounts");

            migrationBuilder.CreateIndex(
                name: "IX_BankingInstitutions_ThirdPartyId",
                table: "BankingInstitutions",
                column: "ThirdPartyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BankAccounts_ThirdPartyId",
                table: "BankAccounts",
                column: "ThirdPartyId",
                unique: true);
        }
    }
}
