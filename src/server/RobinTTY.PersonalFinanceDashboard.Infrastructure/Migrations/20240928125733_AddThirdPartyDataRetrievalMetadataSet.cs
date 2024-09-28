using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddThirdPartyDataRetrievalMetadataSet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ThirdPartyDataRetrievalMetadata",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    DataType = table.Column<int>(type: "INTEGER", nullable: false),
                    DataSource = table.Column<int>(type: "INTEGER", nullable: false),
                    LastRetrievalTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    RetrievalInterval = table.Column<TimeSpan>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThirdPartyDataRetrievalMetadata", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "ThirdPartyDataRetrievalMetadata",
                columns: new[] { "Id", "DataSource", "DataType", "LastRetrievalTime", "RetrievalInterval" },
                values: new object[] { new Guid("f948e52f-ad17-44b8-9cdf-e0b952f139b3"), 1, 1, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(7, 0, 0, 0, 0) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ThirdPartyDataRetrievalMetadata");
        }
    }
}
