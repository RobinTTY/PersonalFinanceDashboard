using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAccessValidityToAuthenticationRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AccessValidForDays",
                table: "AuthenticationRequests",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccessValidForDays",
                table: "AuthenticationRequests");
        }
    }
}
