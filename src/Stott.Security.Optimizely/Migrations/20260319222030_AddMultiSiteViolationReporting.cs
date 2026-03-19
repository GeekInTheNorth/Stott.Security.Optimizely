using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stott.Security.Optimizely.Migrations
{
    /// <inheritdoc />
    public partial class AddMultiSiteViolationReporting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_CspViolationSummary_LookUp",
                table: "tbl_CspViolationSummary");

            migrationBuilder.AddColumn<string>(
                name: "AppId",
                table: "tbl_CspViolationSummary",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HostName",
                table: "tbl_CspViolationSummary",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "idx_CspViolationSummary_LookUp",
                table: "tbl_CspViolationSummary",
                columns: new[] { "BlockedUri", "ViolatedDirective", "AppId", "HostName" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_CspViolationSummary_LookUp",
                table: "tbl_CspViolationSummary");

            migrationBuilder.DropColumn(
                name: "AppId",
                table: "tbl_CspViolationSummary");

            migrationBuilder.DropColumn(
                name: "HostName",
                table: "tbl_CspViolationSummary");

            migrationBuilder.CreateIndex(
                name: "idx_CspViolationSummary_LookUp",
                table: "tbl_CspViolationSummary",
                columns: new[] { "BlockedUri", "ViolatedDirective" });
        }
    }
}
