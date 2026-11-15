using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRetrievalFailureTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ConsecutiveFailures",
                table: "ThirdPartyDataRetrievalMetadata",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "LastError",
                table: "ThirdPartyDataRetrievalMetadata",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NextRetryTime",
                table: "ThirdPartyDataRetrievalMetadata",
                type: "TEXT",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "ThirdPartyDataRetrievalMetadata",
                keyColumn: "Id",
                keyValue: new Guid("0b53ec13-5a8c-4910-bfff-1ba06c3f3859"),
                columns: new[] { "ConsecutiveFailures", "LastError", "NextRetryTime" },
                values: new object[] { 0, null, null });

            migrationBuilder.UpdateData(
                table: "ThirdPartyDataRetrievalMetadata",
                keyColumn: "Id",
                keyValue: new Guid("f948e52f-ad17-44b8-9cdf-e0b952f139b3"),
                columns: new[] { "ConsecutiveFailures", "LastError", "NextRetryTime" },
                values: new object[] { 0, null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConsecutiveFailures",
                table: "ThirdPartyDataRetrievalMetadata");

            migrationBuilder.DropColumn(
                name: "LastError",
                table: "ThirdPartyDataRetrievalMetadata");

            migrationBuilder.DropColumn(
                name: "NextRetryTime",
                table: "ThirdPartyDataRetrievalMetadata");
        }
    }
}
