using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stott.Security.Core.Migrations
{
    public partial class AddAdditionalHeaders : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsXXssProtectionEnabled",
                table: "tbl_CspSecurityHeaderSettings",
                newName: "IsStrictTransportSecuritySubDomainsEnabled");

            migrationBuilder.RenameColumn(
                name: "IsXContentTypeOptionsEnabled",
                table: "tbl_CspSecurityHeaderSettings",
                newName: "IsStrictTransportSecurityEnabled");

            migrationBuilder.AddColumn<int>(
                name: "CrossOriginEmbedderPolicy",
                table: "tbl_CspSecurityHeaderSettings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CrossOriginOpenerPolicy",
                table: "tbl_CspSecurityHeaderSettings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CrossOriginResourcePolicy",
                table: "tbl_CspSecurityHeaderSettings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "ForceHttpRedirect",
                table: "tbl_CspSecurityHeaderSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "StrictTransportSecurityMaxAge",
                table: "tbl_CspSecurityHeaderSettings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "XContentTypeOptions",
                table: "tbl_CspSecurityHeaderSettings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "XssProtection",
                table: "tbl_CspSecurityHeaderSettings",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CrossOriginEmbedderPolicy",
                table: "tbl_CspSecurityHeaderSettings");

            migrationBuilder.DropColumn(
                name: "CrossOriginOpenerPolicy",
                table: "tbl_CspSecurityHeaderSettings");

            migrationBuilder.DropColumn(
                name: "CrossOriginResourcePolicy",
                table: "tbl_CspSecurityHeaderSettings");

            migrationBuilder.DropColumn(
                name: "ForceHttpRedirect",
                table: "tbl_CspSecurityHeaderSettings");

            migrationBuilder.DropColumn(
                name: "StrictTransportSecurityMaxAge",
                table: "tbl_CspSecurityHeaderSettings");

            migrationBuilder.DropColumn(
                name: "XContentTypeOptions",
                table: "tbl_CspSecurityHeaderSettings");

            migrationBuilder.DropColumn(
                name: "XssProtection",
                table: "tbl_CspSecurityHeaderSettings");

            migrationBuilder.RenameColumn(
                name: "IsStrictTransportSecuritySubDomainsEnabled",
                table: "tbl_CspSecurityHeaderSettings",
                newName: "IsXXssProtectionEnabled");

            migrationBuilder.RenameColumn(
                name: "IsStrictTransportSecurityEnabled",
                table: "tbl_CspSecurityHeaderSettings",
                newName: "IsXContentTypeOptionsEnabled");
        }
    }
}
