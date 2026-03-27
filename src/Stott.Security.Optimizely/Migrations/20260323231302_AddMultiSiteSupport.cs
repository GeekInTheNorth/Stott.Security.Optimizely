using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stott.Security.Optimizely.Migrations
{
    /// <inheritdoc />
    public partial class AddMultiSiteSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_permissionpolicy_lookUp",
                table: "tbl_stott_permissionpolicy");

            migrationBuilder.DropIndex(
                name: "idx_CspViolationSummary_LookUp",
                table: "tbl_CspViolationSummary");

            migrationBuilder.DropIndex(
                name: "idx_HeaderName_LookUp",
                table: "tbl_CspCustomHeader");

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

            migrationBuilder.AlterColumn<string>(
                name: "Source",
                table: "tbl_CspSource",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

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
                name: "idx_PermissionPolicySettings_LookUp",
                table: "tbl_stott_permissionpolicysettings",
                columns: new[] { "AppId", "HostName" });

            migrationBuilder.CreateIndex(
                name: "idx_PermissionPolicy_LookUp",
                table: "tbl_stott_permissionpolicy",
                columns: new[] { "Directive", "AppId", "HostName" });

            migrationBuilder.CreateIndex(
                name: "idx_CspViolationSummary_LookUp",
                table: "tbl_CspViolationSummary",
                columns: new[] { "BlockedUri", "ViolatedDirective", "AppId", "HostName" });

            migrationBuilder.CreateIndex(
                name: "idx_CspSource_LookUp",
                table: "tbl_CspSource",
                columns: new[] { "Source", "AppId", "HostName" });

            migrationBuilder.CreateIndex(
                name: "idx_CspSettings_LookUp",
                table: "tbl_CspSettings",
                columns: new[] { "AppId", "HostName" });

            migrationBuilder.CreateIndex(
                name: "idx_CspSandbox_LookUp",
                table: "tbl_CspSandbox",
                columns: new[] { "AppId", "HostName" });

            migrationBuilder.CreateIndex(
                name: "idx_CustomHeader_LookUp",
                table: "tbl_CspCustomHeader",
                columns: new[] { "HeaderName", "AppId", "HostName" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_PermissionPolicySettings_LookUp",
                table: "tbl_stott_permissionpolicysettings");

            migrationBuilder.DropIndex(
                name: "idx_PermissionPolicy_LookUp",
                table: "tbl_stott_permissionpolicy");

            migrationBuilder.DropIndex(
                name: "idx_CspViolationSummary_LookUp",
                table: "tbl_CspViolationSummary");

            migrationBuilder.DropIndex(
                name: "idx_CspSource_LookUp",
                table: "tbl_CspSource");

            migrationBuilder.DropIndex(
                name: "idx_CspSettings_LookUp",
                table: "tbl_CspSettings");

            migrationBuilder.DropIndex(
                name: "idx_CspSandbox_LookUp",
                table: "tbl_CspSandbox");

            migrationBuilder.DropIndex(
                name: "idx_CustomHeader_LookUp",
                table: "tbl_CspCustomHeader");

            migrationBuilder.DropColumn(
                name: "AppId",
                table: "tbl_stott_permissionpolicysettings");

            migrationBuilder.DropColumn(
                name: "HostName",
                table: "tbl_stott_permissionpolicysettings");

            migrationBuilder.DropColumn(
                name: "AppId",
                table: "tbl_stott_permissionpolicy");

            migrationBuilder.DropColumn(
                name: "HostName",
                table: "tbl_stott_permissionpolicy");

            migrationBuilder.DropColumn(
                name: "AppId",
                table: "tbl_CspViolationSummary");

            migrationBuilder.DropColumn(
                name: "HostName",
                table: "tbl_CspViolationSummary");

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

            migrationBuilder.DropColumn(
                name: "AppId",
                table: "tbl_CspCustomHeader");

            migrationBuilder.DropColumn(
                name: "HostName",
                table: "tbl_CspCustomHeader");

            migrationBuilder.AlterColumn<string>(
                name: "Source",
                table: "tbl_CspSource",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "idx_permissionpolicy_lookUp",
                table: "tbl_stott_permissionpolicy",
                column: "Directive");

            migrationBuilder.CreateIndex(
                name: "idx_CspViolationSummary_LookUp",
                table: "tbl_CspViolationSummary",
                columns: new[] { "BlockedUri", "ViolatedDirective" });

            migrationBuilder.CreateIndex(
                name: "idx_HeaderName_LookUp",
                table: "tbl_CspCustomHeader",
                column: "HeaderName");
        }
    }
}
