using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBaseEntityInheritanceWherePossible : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ThirdPartyId",
                table: "Transactions",
                newName: "ThirdPartyTransactionId");

            migrationBuilder.AddColumn<Guid>(
                name: "ThirdPartyId",
                table: "BankAccounts",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ThirdPartyId",
                table: "BankAccounts");

            migrationBuilder.RenameColumn(
                name: "ThirdPartyTransactionId",
                table: "Transactions",
                newName: "ThirdPartyId");
        }
    }
}
