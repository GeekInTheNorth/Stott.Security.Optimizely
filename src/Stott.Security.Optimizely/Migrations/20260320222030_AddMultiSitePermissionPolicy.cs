using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stott.Security.Optimizely.Migrations
{
    /// <inheritdoc />
    public partial class AddMultiSitePermissionPolicy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_permissionpolicy_lookUp",
                table: "tbl_stott_permissionpolicy");

            migrationBuilder.AddColumn<string>(
                name: "AppId",
                table: "tbl_stott_permissionpolicy",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HostName",
                table: "tbl_stott_permissionpolicy",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "idx_permissionpolicy_lookUp",
                table: "tbl_stott_permissionpolicy",
                columns: new[] { "Directive", "AppId", "HostName" });

            migrationBuilder.AddColumn<string>(
                name: "AppId",
                table: "tbl_stott_permissionpolicysettings",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HostName",
                table: "tbl_stott_permissionpolicysettings",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "idx_permissionpolicysettings_AppId_HostName",
                table: "tbl_stott_permissionpolicysettings",
                columns: new[] { "AppId", "HostName" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_permissionpolicy_lookUp",
                table: "tbl_stott_permissionpolicy");

            migrationBuilder.DropColumn(
                name: "AppId",
                table: "tbl_stott_permissionpolicy");

            migrationBuilder.DropColumn(
                name: "HostName",
                table: "tbl_stott_permissionpolicy");

            migrationBuilder.CreateIndex(
                name: "idx_permissionpolicy_lookUp",
                table: "tbl_stott_permissionpolicy",
                column: "Directive");

            migrationBuilder.DropIndex(
                name: "idx_permissionpolicysettings_AppId_HostName",
                table: "tbl_stott_permissionpolicysettings");

            migrationBuilder.DropColumn(
                name: "AppId",
                table: "tbl_stott_permissionpolicysettings");

            migrationBuilder.DropColumn(
                name: "HostName",
                table: "tbl_stott_permissionpolicysettings");
        }
    }
}
