using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Migrations
{
    /// <summary>
    /// A missing category or note was stored as an empty string. Separate from
    /// <see cref="MakeTransactionCategoryAndNotesOptional"/>, as SQLite rebuilds the table at the end of a migration.
    /// </summary>
    public partial class StoreMissingTransactionCategoriesAndNotesAsNull : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE Transactions SET Category = NULLIF(Category, ''), Notes = NULLIF(Notes, '');");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Reverting MakeTransactionCategoryAndNotesOptional stores empty strings again
        }
    }
}
