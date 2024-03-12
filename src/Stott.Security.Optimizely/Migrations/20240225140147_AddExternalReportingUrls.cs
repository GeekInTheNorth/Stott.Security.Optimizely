using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stott.Security.Optimizely.Migrations
{
    public partial class AddExternalReportingUrls : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExternalReportToUrl",
                table: "tbl_CspSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExternalReportUriUrl",
                table: "tbl_CspSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "UseExternalReporting",
                table: "tbl_CspSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "UseInternalReporting",
                table: "tbl_CspSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExternalReportToUrl",
                table: "tbl_CspSettings");

            migrationBuilder.DropColumn(
                name: "ExternalReportUriUrl",
                table: "tbl_CspSettings");

            migrationBuilder.DropColumn(
                name: "UseExternalReporting",
                table: "tbl_CspSettings");

            migrationBuilder.DropColumn(
                name: "UseInternalReporting",
                table: "tbl_CspSettings");
        }
    }
}
