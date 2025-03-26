using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stott.Security.Optimizely.Migrations
{
    /// <inheritdoc />
    public partial class AddPermissionPolicy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tbl_stott_permissionpolicy",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Directive = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EnabledState = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Origins = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_stott_permissionpolicy", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_stott_permissionpolicysettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_stott_permissionpolicysettings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "idx_permissionpolicy_lookUp",
                table: "tbl_stott_permissionpolicy",
                column: "Directive");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tbl_stott_permissionpolicy");

            migrationBuilder.DropTable(
                name: "tbl_stott_permissionpolicysettings");
        }
    }
}
