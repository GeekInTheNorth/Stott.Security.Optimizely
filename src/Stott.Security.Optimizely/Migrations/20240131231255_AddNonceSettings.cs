using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stott.Security.Optimizely.Migrations
{
    public partial class AddNonceSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsNonceEnabled",
                table: "tbl_CspSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsStrictDynamicEnabled",
                table: "tbl_CspSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsNonceEnabled",
                table: "tbl_CspSettings");

            migrationBuilder.DropColumn(
                name: "IsStrictDynamicEnabled",
                table: "tbl_CspSettings");
        }
    }
}
