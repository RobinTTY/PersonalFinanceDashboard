using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountIdToTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EfTagEfTransaction");

            migrationBuilder.AddColumn<string>(
                name: "AccountId",
                table: "Transactions",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "TagEntityTransactionEntity",
                columns: table => new
                {
                    TagsId = table.Column<string>(type: "TEXT", nullable: false),
                    TransactionsId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagEntityTransactionEntity", x => new { x.TagsId, x.TransactionsId });
                    table.ForeignKey(
                        name: "FK_TagEntityTransactionEntity_Tags_TagsId",
                        column: x => x.TagsId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TagEntityTransactionEntity_Transactions_TransactionsId",
                        column: x => x.TransactionsId,
                        principalTable: "Transactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TagEntityTransactionEntity_TransactionsId",
                table: "TagEntityTransactionEntity",
                column: "TransactionsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TagEntityTransactionEntity");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "Transactions");

            migrationBuilder.CreateTable(
                name: "EfTagEfTransaction",
                columns: table => new
                {
                    TagsId = table.Column<string>(type: "TEXT", nullable: false),
                    TransactionsId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EfTagEfTransaction", x => new { x.TagsId, x.TransactionsId });
                    table.ForeignKey(
                        name: "FK_EfTagEfTransaction_Tags_TagsId",
                        column: x => x.TagsId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EfTagEfTransaction_Transactions_TransactionsId",
                        column: x => x.TransactionsId,
                        principalTable: "Transactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EfTagEfTransaction_TransactionsId",
                table: "EfTagEfTransaction",
                column: "TransactionsId");
        }
    }
}
