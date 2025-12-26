using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTransactionMetadataDefaultData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ThirdPartyDataRetrievalMetadata",
                columns: new[] { "Id", "DataSource", "DataType", "LastRetrievalTime", "RetrievalInterval" },
                values: new object[] { new Guid("80fbbf1a-c484-4369-aef9-6f24c3b02fa8"), 1, 4, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 0, 0, 0) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ThirdPartyDataRetrievalMetadata",
                keyColumn: "Id",
                keyValue: new Guid("80fbbf1a-c484-4369-aef9-6f24c3b02fa8"));
        }
    }
}
