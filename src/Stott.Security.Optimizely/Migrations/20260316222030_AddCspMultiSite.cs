using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stott.Security.Optimizely.Migrations
{
    /// <inheritdoc />
    public partial class AddCspMultiSite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AppId",
                table: "tbl_CspSource",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HostName",
                table: "tbl_CspSource",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AppId",
                table: "tbl_CspSettings",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HostName",
                table: "tbl_CspSettings",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AppId",
                table: "tbl_CspSandbox",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HostName",
                table: "tbl_CspSandbox",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "idx_CspSource_AppId_HostName",
                table: "tbl_CspSource",
                columns: new[] { "AppId", "HostName" });

            migrationBuilder.CreateIndex(
                name: "idx_CspSettings_AppId_HostName",
                table: "tbl_CspSettings",
                columns: new[] { "AppId", "HostName" });

            migrationBuilder.CreateIndex(
                name: "idx_CspSandbox_AppId_HostName",
                table: "tbl_CspSandbox",
                columns: new[] { "AppId", "HostName" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_CspSource_AppId_HostName",
                table: "tbl_CspSource");

            migrationBuilder.DropIndex(
                name: "idx_CspSettings_AppId_HostName",
                table: "tbl_CspSettings");

            migrationBuilder.DropIndex(
                name: "idx_CspSandbox_AppId_HostName",
                table: "tbl_CspSandbox");

            migrationBuilder.DropColumn(
                name: "AppId",
                table: "tbl_CspSource");

            migrationBuilder.DropColumn(
                name: "HostName",
                table: "tbl_CspSource");

            migrationBuilder.DropColumn(
                name: "AppId",
                table: "tbl_CspSettings");

            migrationBuilder.DropColumn(
                name: "HostName",
                table: "tbl_CspSettings");

            migrationBuilder.DropColumn(
                name: "AppId",
                table: "tbl_CspSandbox");

            migrationBuilder.DropColumn(
                name: "HostName",
                table: "tbl_CspSandbox");
        }
    }
}
