using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Migrations
{
    /// <summary>
    /// The synchronization overwrote the name and description of bank accounts, including the user's changes.
    /// They are now stored separately from the values of the data provider. The current values of synchronized
    /// accounts are moved to the provider columns, as they can't be told apart from the user's.
    /// </summary>
    public partial class SeparateProviderAndUserAccountNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProviderDescription",
                table: "BankAccounts",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProviderName",
                table: "BankAccounts",
                type: "TEXT",
                nullable: true);

            // SQLite evaluates every right-hand side against the original row
            migrationBuilder.Sql(
                "UPDATE BankAccounts SET ProviderName = Name, ProviderDescription = Description, " +
                "Name = NULL, Description = NULL WHERE Source <> 'Manual';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "UPDATE BankAccounts SET Name = COALESCE(Name, ProviderName), " +
                "Description = COALESCE(Description, ProviderDescription);");

            migrationBuilder.DropColumn(
                name: "ProviderDescription",
                table: "BankAccounts");

            migrationBuilder.DropColumn(
                name: "ProviderName",
                table: "BankAccounts");
        }
    }
}
