using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddThirdPartyDataMetadataForIndividualEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ThirdPartyDataRetrievalMetadata",
                keyColumn: "Id",
                keyValue: new Guid("6a7aa33d-1a81-46fe-80b2-d449ba851861"));

            migrationBuilder.DeleteData(
                table: "ThirdPartyDataRetrievalMetadata",
                keyColumn: "Id",
                keyValue: new Guid("80fbbf1a-c484-4369-aef9-6f24c3b02fa8"));

            migrationBuilder.AddColumn<Guid>(
                name: "SynchronizationEntityId",
                table: "ThirdPartyDataRetrievalMetadata",
                type: "TEXT",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "ThirdPartyDataRetrievalMetadata",
                keyColumn: "Id",
                keyValue: new Guid("0b53ec13-5a8c-4910-bfff-1ba06c3f3859"),
                column: "SynchronizationEntityId",
                value: null);

            migrationBuilder.UpdateData(
                table: "ThirdPartyDataRetrievalMetadata",
                keyColumn: "Id",
                keyValue: new Guid("f948e52f-ad17-44b8-9cdf-e0b952f139b3"),
                column: "SynchronizationEntityId",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SynchronizationEntityId",
                table: "ThirdPartyDataRetrievalMetadata");

            migrationBuilder.InsertData(
                table: "ThirdPartyDataRetrievalMetadata",
                columns: new[] { "Id", "DataSource", "DataType", "LastRetrievalTime", "RetrievalInterval" },
                values: new object[,]
                {
                    { new Guid("6a7aa33d-1a81-46fe-80b2-d449ba851861"), 1, 3, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(7, 0, 0, 0, 0) },
                    { new Guid("80fbbf1a-c484-4369-aef9-6f24c3b02fa8"), 1, 4, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 12, 0, 0, 0) }
                });
        }
    }
}
