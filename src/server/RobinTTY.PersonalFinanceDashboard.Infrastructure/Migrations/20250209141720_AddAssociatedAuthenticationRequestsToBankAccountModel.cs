using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAssociatedAuthenticationRequestsToBankAccountModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BankAccounts_AuthenticationRequests_AuthenticationRequestId",
                table: "BankAccounts");

            migrationBuilder.DropIndex(
                name: "IX_BankAccounts_AuthenticationRequestId",
                table: "BankAccounts");

            migrationBuilder.DropColumn(
                name: "AuthenticationRequestId",
                table: "BankAccounts");

            migrationBuilder.CreateTable(
                name: "AuthenticationRequestBankAccount",
                columns: table => new
                {
                    AssociatedAccountsId = table.Column<Guid>(type: "TEXT", nullable: false),
                    AssociatedAuthenticationRequestsId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthenticationRequestBankAccount", x => new { x.AssociatedAccountsId, x.AssociatedAuthenticationRequestsId });
                    table.ForeignKey(
                        name: "FK_AuthenticationRequestBankAccount_AuthenticationRequests_AssociatedAuthenticationRequestsId",
                        column: x => x.AssociatedAuthenticationRequestsId,
                        principalTable: "AuthenticationRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AuthenticationRequestBankAccount_BankAccounts_AssociatedAccountsId",
                        column: x => x.AssociatedAccountsId,
                        principalTable: "BankAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuthenticationRequestBankAccount_AssociatedAuthenticationRequestsId",
                table: "AuthenticationRequestBankAccount",
                column: "AssociatedAuthenticationRequestsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuthenticationRequestBankAccount");

            migrationBuilder.AddColumn<Guid>(
                name: "AuthenticationRequestId",
                table: "BankAccounts",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BankAccounts_AuthenticationRequestId",
                table: "BankAccounts",
                column: "AuthenticationRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_BankAccounts_AuthenticationRequests_AuthenticationRequestId",
                table: "BankAccounts",
                column: "AuthenticationRequestId",
                principalTable: "AuthenticationRequests",
                principalColumn: "Id");
        }
    }
}
