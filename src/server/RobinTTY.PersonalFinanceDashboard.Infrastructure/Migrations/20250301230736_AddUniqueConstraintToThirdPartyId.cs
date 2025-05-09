using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueConstraintToThirdPartyId : Migration
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

            migrationBuilder.DropIndex(
                name: "IX_AuthenticationRequests_ThirdPartyId",
                table: "AuthenticationRequests");

            migrationBuilder.InsertData(
                table: "ThirdPartyDataRetrievalMetadata",
                columns: new[] { "Id", "DataSource", "DataType", "LastRetrievalTime", "RetrievalInterval" },
                values: new object[] { new Guid("6a7aa33d-1a81-46fe-80b2-d449ba851861"), 1, 3, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(7, 0, 0, 0, 0) });

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

            migrationBuilder.CreateIndex(
                name: "IX_AuthenticationRequests_ThirdPartyId",
                table: "AuthenticationRequests",
                column: "ThirdPartyId",
                unique: true);
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

            migrationBuilder.DropIndex(
                name: "IX_AuthenticationRequests_ThirdPartyId",
                table: "AuthenticationRequests");

            migrationBuilder.DeleteData(
                table: "ThirdPartyDataRetrievalMetadata",
                keyColumn: "Id",
                keyValue: new Guid("6a7aa33d-1a81-46fe-80b2-d449ba851861"));

            migrationBuilder.CreateIndex(
                name: "IX_BankingInstitutions_ThirdPartyId",
                table: "BankingInstitutions",
                column: "ThirdPartyId");

            migrationBuilder.CreateIndex(
                name: "IX_BankAccounts_ThirdPartyId",
                table: "BankAccounts",
                column: "ThirdPartyId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthenticationRequests_ThirdPartyId",
                table: "AuthenticationRequests",
                column: "ThirdPartyId");
        }
    }
}
