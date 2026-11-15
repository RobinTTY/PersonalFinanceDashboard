using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RobinTTY.PersonalFinanceDashboard.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceThirdPartyIdWithDataSource : Migration
    {
        private const string EmptyGuid = "00000000-0000-0000-0000-000000000000";

        // The tables whose third party id was a GUID, which was an empty GUID for manually created entities
        private static readonly string[] GuidTables = ["Transactions", "BankAccounts", "AuthenticationRequests"];

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Transactions_BankAccountId_ThirdPartyId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_BankingInstitutions_ThirdPartyId",
                table: "BankingInstitutions");

            migrationBuilder.DropIndex(
                name: "IX_BankAccounts_ThirdPartyId",
                table: "BankAccounts");

            migrationBuilder.DropIndex(
                name: "IX_AuthenticationRequests_ThirdPartyId",
                table: "AuthenticationRequests");

            foreach (var table in GuidTables)
            {
                AddSourceColumns(migrationBuilder, table);
                // GUIDs are stored in uppercase, while GoCardless ids are lowercase
                migrationBuilder.Sql($"""
                                      UPDATE "{table}" SET "Source" = 'GoCardless', "ExternalId" = lower("ThirdPartyId")
                                      WHERE "ThirdPartyId" <> '{EmptyGuid}';
                                      """);
                migrationBuilder.DropColumn(name: "ThirdPartyId", table: table);
            }

            AddSourceColumns(migrationBuilder, "BankingInstitutions");
            migrationBuilder.Sql("""
                                 UPDATE "BankingInstitutions" SET "Source" = 'GoCardless', "ExternalId" = "ThirdPartyId"
                                 WHERE "ThirdPartyId" <> '';
                                 """);
            migrationBuilder.DropColumn(name: "ThirdPartyId", table: "BankingInstitutions");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_BankAccountId_Source_ExternalId",
                table: "Transactions",
                columns: new[] { "BankAccountId", "Source", "ExternalId" },
                unique: true,
                filter: "\"ExternalId\" IS NOT NULL");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Transaction_ExternalIdMatchesSource",
                table: "Transactions",
                sql: "(\"Source\" = 'Manual') = (\"ExternalId\" IS NULL)");

            migrationBuilder.CreateIndex(
                name: "IX_BankingInstitutions_Source_ExternalId",
                table: "BankingInstitutions",
                columns: new[] { "Source", "ExternalId" },
                unique: true,
                filter: "\"ExternalId\" IS NOT NULL");

            migrationBuilder.AddCheckConstraint(
                name: "CK_BankingInstitution_ExternalIdMatchesSource",
                table: "BankingInstitutions",
                sql: "(\"Source\" = 'Manual') = (\"ExternalId\" IS NULL)");

            migrationBuilder.CreateIndex(
                name: "IX_BankAccounts_Source_ExternalId",
                table: "BankAccounts",
                columns: new[] { "Source", "ExternalId" },
                unique: true,
                filter: "\"ExternalId\" IS NOT NULL");

            migrationBuilder.AddCheckConstraint(
                name: "CK_BankAccount_ExternalIdMatchesSource",
                table: "BankAccounts",
                sql: "(\"Source\" = 'Manual') = (\"ExternalId\" IS NULL)");

            migrationBuilder.CreateIndex(
                name: "IX_AuthenticationRequests_Source_ExternalId",
                table: "AuthenticationRequests",
                columns: new[] { "Source", "ExternalId" },
                unique: true,
                filter: "\"ExternalId\" IS NOT NULL");

            migrationBuilder.AddCheckConstraint(
                name: "CK_AuthenticationRequest_ExternalIdMatchesSource",
                table: "AuthenticationRequests",
                sql: "(\"Source\" = 'Manual') = (\"ExternalId\" IS NULL)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Transactions_BankAccountId_Source_ExternalId",
                table: "Transactions");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Transaction_ExternalIdMatchesSource",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_BankingInstitutions_Source_ExternalId",
                table: "BankingInstitutions");

            migrationBuilder.DropCheckConstraint(
                name: "CK_BankingInstitution_ExternalIdMatchesSource",
                table: "BankingInstitutions");

            migrationBuilder.DropIndex(
                name: "IX_BankAccounts_Source_ExternalId",
                table: "BankAccounts");

            migrationBuilder.DropCheckConstraint(
                name: "CK_BankAccount_ExternalIdMatchesSource",
                table: "BankAccounts");

            migrationBuilder.DropIndex(
                name: "IX_AuthenticationRequests_Source_ExternalId",
                table: "AuthenticationRequests");

            migrationBuilder.DropCheckConstraint(
                name: "CK_AuthenticationRequest_ExternalIdMatchesSource",
                table: "AuthenticationRequests");

            foreach (var table in GuidTables)
            {
                migrationBuilder.AddColumn<string>(
                    name: "ThirdPartyId",
                    table: table,
                    type: "TEXT",
                    nullable: false,
                    defaultValue: EmptyGuid);
                migrationBuilder.Sql($"""
                                      UPDATE "{table}" SET "ThirdPartyId" = upper("ExternalId")
                                      WHERE "ExternalId" IS NOT NULL;
                                      """);
                DropSourceColumns(migrationBuilder, table);
            }

            migrationBuilder.AddColumn<string>(
                name: "ThirdPartyId",
                table: "BankingInstitutions",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
            migrationBuilder.Sql("""
                                 UPDATE "BankingInstitutions" SET "ThirdPartyId" = "ExternalId"
                                 WHERE "ExternalId" IS NOT NULL;
                                 """);
            DropSourceColumns(migrationBuilder, "BankingInstitutions");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_BankAccountId_ThirdPartyId",
                table: "Transactions",
                columns: new[] { "BankAccountId", "ThirdPartyId" },
                unique: true,
                filter: "\"ThirdPartyId\" <> '00000000-0000-0000-0000-000000000000'");

            migrationBuilder.CreateIndex(
                name: "IX_BankingInstitutions_ThirdPartyId",
                table: "BankingInstitutions",
                column: "ThirdPartyId",
                unique: true,
                filter: "\"ThirdPartyId\" <> ''");

            migrationBuilder.CreateIndex(
                name: "IX_BankAccounts_ThirdPartyId",
                table: "BankAccounts",
                column: "ThirdPartyId",
                unique: true,
                filter: "\"ThirdPartyId\" <> '00000000-0000-0000-0000-000000000000'");

            migrationBuilder.CreateIndex(
                name: "IX_AuthenticationRequests_ThirdPartyId",
                table: "AuthenticationRequests",
                column: "ThirdPartyId",
                unique: true);
        }

        private static void AddSourceColumns(MigrationBuilder migrationBuilder, string table)
        {
            migrationBuilder.AddColumn<string>(
                name: "Source",
                table: table,
                type: "TEXT",
                nullable: false,
                defaultValue: "Manual");

            migrationBuilder.AddColumn<string>(
                name: "ExternalId",
                table: table,
                type: "TEXT",
                nullable: true);
        }

        private static void DropSourceColumns(MigrationBuilder migrationBuilder, string table)
        {
            migrationBuilder.DropColumn(name: "Source", table: table);
            migrationBuilder.DropColumn(name: "ExternalId", table: table);
        }
    }
}
