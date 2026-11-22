using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInstitutionToAuthenticationRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "InstitutionId",
                table: "AuthenticationRequests",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuthenticationRequests_InstitutionId",
                table: "AuthenticationRequests",
                column: "InstitutionId");

            migrationBuilder.AddForeignKey(
                name: "FK_AuthenticationRequests_BankingInstitutions_InstitutionId",
                table: "AuthenticationRequests",
                column: "InstitutionId",
                principalTable: "BankingInstitutions",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuthenticationRequests_BankingInstitutions_InstitutionId",
                table: "AuthenticationRequests");

            migrationBuilder.DropIndex(
                name: "IX_AuthenticationRequests_InstitutionId",
                table: "AuthenticationRequests");

            migrationBuilder.DropColumn(
                name: "InstitutionId",
                table: "AuthenticationRequests");
        }
    }
}
