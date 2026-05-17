using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stott.Security.Optimizely.Migrations
{
    /// <inheritdoc />
    public partial class AddMultiSiteAndHostConfig : Migration
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
                name: "HostName",
                table: "tbl_stott_permissionpolicysettings",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SiteId",
                table: "tbl_stott_permissionpolicysettings",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HostName",
                table: "tbl_stott_permissionpolicy",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SiteId",
                table: "tbl_stott_permissionpolicy",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HostName",
                table: "tbl_CspViolationSummary",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SiteId",
                table: "tbl_CspViolationSummary",
                type: "uniqueidentifier",
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
                name: "HostName",
                table: "tbl_CspSource",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SiteId",
                table: "tbl_CspSource",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HostName",
                table: "tbl_CspSettings",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SiteId",
                table: "tbl_CspSettings",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HostName",
                table: "tbl_CspSandbox",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SiteId",
                table: "tbl_CspSandbox",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HostName",
                table: "tbl_CspCustomHeader",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SiteId",
                table: "tbl_CspCustomHeader",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "idx_PermissionPolicySettings_LookUp",
                table: "tbl_stott_permissionpolicysettings",
                columns: new[] { "SiteId", "HostName" });

            migrationBuilder.CreateIndex(
                name: "idx_PermissionPolicy_LookUp",
                table: "tbl_stott_permissionpolicy",
                columns: new[] { "Directive", "SiteId", "HostName" });

            migrationBuilder.CreateIndex(
                name: "idx_CspViolationSummary_LookUp",
                table: "tbl_CspViolationSummary",
                columns: new[] { "BlockedUri", "ViolatedDirective", "SiteId", "HostName" });

            migrationBuilder.CreateIndex(
                name: "idx_CspSource_LookUp",
                table: "tbl_CspSource",
                columns: new[] { "Source", "SiteId", "HostName" });

            migrationBuilder.CreateIndex(
                name: "idx_CspSettings_LookUp",
                table: "tbl_CspSettings",
                columns: new[] { "SiteId", "HostName" });

            migrationBuilder.CreateIndex(
                name: "idx_CspSandbox_LookUp",
                table: "tbl_CspSandbox",
                columns: new[] { "SiteId", "HostName" });

            migrationBuilder.CreateIndex(
                name: "idx_CspCustomHeader_LookUp",
                table: "tbl_CspCustomHeader",
                columns: new[] { "HeaderName", "SiteId", "HostName" });
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
                name: "idx_CspCustomHeader_LookUp",
                table: "tbl_CspCustomHeader");

            migrationBuilder.DropColumn(
                name: "HostName",
                table: "tbl_stott_permissionpolicysettings");

            migrationBuilder.DropColumn(
                name: "SiteId",
                table: "tbl_stott_permissionpolicysettings");

            migrationBuilder.DropColumn(
                name: "HostName",
                table: "tbl_stott_permissionpolicy");

            migrationBuilder.DropColumn(
                name: "SiteId",
                table: "tbl_stott_permissionpolicy");

            migrationBuilder.DropColumn(
                name: "HostName",
                table: "tbl_CspViolationSummary");

            migrationBuilder.DropColumn(
                name: "SiteId",
                table: "tbl_CspViolationSummary");

            migrationBuilder.DropColumn(
                name: "HostName",
                table: "tbl_CspSource");

            migrationBuilder.DropColumn(
                name: "SiteId",
                table: "tbl_CspSource");

            migrationBuilder.DropColumn(
                name: "HostName",
                table: "tbl_CspSettings");

            migrationBuilder.DropColumn(
                name: "SiteId",
                table: "tbl_CspSettings");

            migrationBuilder.DropColumn(
                name: "HostName",
                table: "tbl_CspSandbox");

            migrationBuilder.DropColumn(
                name: "SiteId",
                table: "tbl_CspSandbox");

            migrationBuilder.DropColumn(
                name: "HostName",
                table: "tbl_CspCustomHeader");

            migrationBuilder.DropColumn(
                name: "SiteId",
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
