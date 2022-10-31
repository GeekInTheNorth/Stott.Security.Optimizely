using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stott.Security.Optimizely.Migrations
{
    public partial class AddModifiedFieldsToAuditableTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Modified",
                table: "tbl_CspSource",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "tbl_CspSource",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Modified",
                table: "tbl_CspSettings",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "tbl_CspSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Modified",
                table: "tbl_CspSecurityHeaderSettings",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "tbl_CspSecurityHeaderSettings",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Modified",
                table: "tbl_CspSource");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "tbl_CspSource");

            migrationBuilder.DropColumn(
                name: "Modified",
                table: "tbl_CspSettings");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "tbl_CspSettings");

            migrationBuilder.DropColumn(
                name: "Modified",
                table: "tbl_CspSecurityHeaderSettings");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "tbl_CspSecurityHeaderSettings");
        }
    }
}
