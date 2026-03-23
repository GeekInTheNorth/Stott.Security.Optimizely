using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stott.Security.Optimizely.Migrations
{
    /// <inheritdoc />
    public partial class AddMultiSiteCustomHeaders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_HeaderName_LookUp",
                table: "tbl_CspCustomHeader");

            migrationBuilder.AddColumn<string>(
                name: "AppId",
                table: "tbl_CspCustomHeader",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HostName",
                table: "tbl_CspCustomHeader",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "idx_CustomHeader_LookUp",
                table: "tbl_CspCustomHeader",
                columns: new[] { "HeaderName", "AppId", "HostName" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_CustomHeader_LookUp",
                table: "tbl_CspCustomHeader");

            migrationBuilder.DropColumn(
                name: "AppId",
                table: "tbl_CspCustomHeader");

            migrationBuilder.DropColumn(
                name: "HostName",
                table: "tbl_CspCustomHeader");

            migrationBuilder.CreateIndex(
                name: "idx_HeaderName_LookUp",
                table: "tbl_CspCustomHeader",
                column: "HeaderName");
        }
    }
}
