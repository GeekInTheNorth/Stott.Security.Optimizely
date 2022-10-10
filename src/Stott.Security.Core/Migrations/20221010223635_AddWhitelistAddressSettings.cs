using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stott.Security.Core.Migrations
{
    public partial class AddWhitelistAddressSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsWhitelistEnabled",
                table: "tbl_CspSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "WhitelistUrl",
                table: "tbl_CspSettings",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsWhitelistEnabled",
                table: "tbl_CspSettings");

            migrationBuilder.DropColumn(
                name: "WhitelistUrl",
                table: "tbl_CspSettings");
        }
    }
}
