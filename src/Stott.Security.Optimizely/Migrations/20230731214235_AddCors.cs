using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stott.Security.Optimizely.Migrations
{
    public partial class AddCors : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tbl_CorsSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AllowCredentials = table.Column<bool>(type: "bit", nullable: false),
                    AllowHeaders = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AllowMethods = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AllowOrigins = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaxAge = table.Column<int>(type: "int", nullable: false),
                    ExposeHeaders = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_CorsSettings", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tbl_CorsSettings");
        }
    }
}
