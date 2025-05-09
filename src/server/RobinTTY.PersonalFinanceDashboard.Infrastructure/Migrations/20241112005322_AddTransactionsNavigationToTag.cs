using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTransactionsNavigationToTag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Transactions_TransactionId",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_TransactionId",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "TransactionId",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "AssociatedAccounts",
                table: "AuthenticationRequests");

            migrationBuilder.AddColumn<string>(
                name: "AuthenticationRequestId",
                table: "BankAccounts",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TagTransaction",
                columns: table => new
                {
                    TagsId = table.Column<string>(type: "TEXT", nullable: false),
                    TransactionsId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagTransaction", x => new { x.TagsId, x.TransactionsId });
                    table.ForeignKey(
                        name: "FK_TagTransaction_Tags_TagsId",
                        column: x => x.TagsId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TagTransaction_Transactions_TransactionsId",
                        column: x => x.TransactionsId,
                        principalTable: "Transactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BankAccounts_AuthenticationRequestId",
                table: "BankAccounts",
                column: "AuthenticationRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_TagTransaction_TransactionsId",
                table: "TagTransaction",
                column: "TransactionsId");

            migrationBuilder.AddForeignKey(
                name: "FK_BankAccounts_AuthenticationRequests_AuthenticationRequestId",
                table: "BankAccounts",
                column: "AuthenticationRequestId",
                principalTable: "AuthenticationRequests",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BankAccounts_AuthenticationRequests_AuthenticationRequestId",
                table: "BankAccounts");

            migrationBuilder.DropTable(
                name: "TagTransaction");

            migrationBuilder.DropIndex(
                name: "IX_BankAccounts_AuthenticationRequestId",
                table: "BankAccounts");

            migrationBuilder.DropColumn(
                name: "AuthenticationRequestId",
                table: "BankAccounts");

            migrationBuilder.AddColumn<string>(
                name: "TransactionId",
                table: "Tags",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AssociatedAccounts",
                table: "AuthenticationRequests",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_TransactionId",
                table: "Tags",
                column: "TransactionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Transactions_TransactionId",
                table: "Tags",
                column: "TransactionId",
                principalTable: "Transactions",
                principalColumn: "Id");
        }
    }
}
