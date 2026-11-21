using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRetrieveFromToRetrievalMetadata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "RetrieveFrom",
                table: "ThirdPartyDataRetrievalMetadata",
                type: "TEXT",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "ThirdPartyDataRetrievalMetadata",
                keyColumn: "Id",
                keyValue: new Guid("0b53ec13-5a8c-4910-bfff-1ba06c3f3859"),
                column: "RetrieveFrom",
                value: null);

            migrationBuilder.UpdateData(
                table: "ThirdPartyDataRetrievalMetadata",
                keyColumn: "Id",
                keyValue: new Guid("f948e52f-ad17-44b8-9cdf-e0b952f139b3"),
                column: "RetrieveFrom",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RetrieveFrom",
                table: "ThirdPartyDataRetrievalMetadata");
        }
    }
}
