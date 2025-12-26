using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddThirdPartyIdToTransactionModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ThirdPartyTransactionId",
                table: "Transactions",
                newName: "BankTransactionId");

            migrationBuilder.AddColumn<Guid>(
                name: "ThirdPartyId",
                table: "Transactions",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ThirdPartyId",
                table: "Transactions");

            migrationBuilder.RenameColumn(
                name: "BankTransactionId",
                table: "Transactions",
                newName: "ThirdPartyTransactionId");
        }
    }
}
