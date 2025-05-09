using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixTablesBeingMergedToCommonBaseType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuthenticationRequestBankAccount_ThirdPartyEntity_AssociatedAccountsId",
                table: "AuthenticationRequestBankAccount");

            migrationBuilder.DropForeignKey(
                name: "FK_AuthenticationRequestBankAccount_ThirdPartyEntity_AssociatedAuthenticationRequestsId",
                table: "AuthenticationRequestBankAccount");

            migrationBuilder.DropForeignKey(
                name: "FK_ThirdPartyEntity_BankingInstitutions_AssociatedInstitutionId",
                table: "ThirdPartyEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_ThirdPartyEntity_BankAccountId",
                table: "Transactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ThirdPartyEntity",
                table: "ThirdPartyEntity");

            migrationBuilder.DropColumn(
                name: "AuthenticationLink",
                table: "ThirdPartyEntity");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "ThirdPartyEntity");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "ThirdPartyEntity");

            migrationBuilder.RenameTable(
                name: "ThirdPartyEntity",
                newName: "BankAccounts");

            migrationBuilder.RenameIndex(
                name: "IX_ThirdPartyEntity_ThirdPartyId",
                table: "BankAccounts",
                newName: "IX_BankAccounts_ThirdPartyId");

            migrationBuilder.RenameIndex(
                name: "IX_ThirdPartyEntity_AssociatedInstitutionId",
                table: "BankAccounts",
                newName: "IX_BankAccounts_AssociatedInstitutionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BankAccounts",
                table: "BankAccounts",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "AuthenticationRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    AuthenticationLink = table.Column<string>(type: "TEXT", nullable: false),
                    ThirdPartyId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthenticationRequests", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BankingInstitutions_ThirdPartyId",
                table: "BankingInstitutions",
                column: "ThirdPartyId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthenticationRequests_ThirdPartyId",
                table: "AuthenticationRequests",
                column: "ThirdPartyId");

            migrationBuilder.AddForeignKey(
                name: "FK_AuthenticationRequestBankAccount_AuthenticationRequests_AssociatedAuthenticationRequestsId",
                table: "AuthenticationRequestBankAccount",
                column: "AssociatedAuthenticationRequestsId",
                principalTable: "AuthenticationRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AuthenticationRequestBankAccount_BankAccounts_AssociatedAccountsId",
                table: "AuthenticationRequestBankAccount",
                column: "AssociatedAccountsId",
                principalTable: "BankAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BankAccounts_BankingInstitutions_AssociatedInstitutionId",
                table: "BankAccounts",
                column: "AssociatedInstitutionId",
                principalTable: "BankingInstitutions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_BankAccounts_BankAccountId",
                table: "Transactions",
                column: "BankAccountId",
                principalTable: "BankAccounts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuthenticationRequestBankAccount_AuthenticationRequests_AssociatedAuthenticationRequestsId",
                table: "AuthenticationRequestBankAccount");

            migrationBuilder.DropForeignKey(
                name: "FK_AuthenticationRequestBankAccount_BankAccounts_AssociatedAccountsId",
                table: "AuthenticationRequestBankAccount");

            migrationBuilder.DropForeignKey(
                name: "FK_BankAccounts_BankingInstitutions_AssociatedInstitutionId",
                table: "BankAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_BankAccounts_BankAccountId",
                table: "Transactions");

            migrationBuilder.DropTable(
                name: "AuthenticationRequests");

            migrationBuilder.DropIndex(
                name: "IX_BankingInstitutions_ThirdPartyId",
                table: "BankingInstitutions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BankAccounts",
                table: "BankAccounts");

            migrationBuilder.RenameTable(
                name: "BankAccounts",
                newName: "ThirdPartyEntity");

            migrationBuilder.RenameIndex(
                name: "IX_BankAccounts_ThirdPartyId",
                table: "ThirdPartyEntity",
                newName: "IX_ThirdPartyEntity_ThirdPartyId");

            migrationBuilder.RenameIndex(
                name: "IX_BankAccounts_AssociatedInstitutionId",
                table: "ThirdPartyEntity",
                newName: "IX_ThirdPartyEntity_AssociatedInstitutionId");

            migrationBuilder.AddColumn<string>(
                name: "AuthenticationLink",
                table: "ThirdPartyEntity",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "ThirdPartyEntity",
                type: "TEXT",
                maxLength: 21,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "ThirdPartyEntity",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ThirdPartyEntity",
                table: "ThirdPartyEntity",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AuthenticationRequestBankAccount_ThirdPartyEntity_AssociatedAccountsId",
                table: "AuthenticationRequestBankAccount",
                column: "AssociatedAccountsId",
                principalTable: "ThirdPartyEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AuthenticationRequestBankAccount_ThirdPartyEntity_AssociatedAuthenticationRequestsId",
                table: "AuthenticationRequestBankAccount",
                column: "AssociatedAuthenticationRequestsId",
                principalTable: "ThirdPartyEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ThirdPartyEntity_BankingInstitutions_AssociatedInstitutionId",
                table: "ThirdPartyEntity",
                column: "AssociatedInstitutionId",
                principalTable: "BankingInstitutions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_ThirdPartyEntity_BankAccountId",
                table: "Transactions",
                column: "BankAccountId",
                principalTable: "ThirdPartyEntity",
                principalColumn: "Id");
        }
    }
}
