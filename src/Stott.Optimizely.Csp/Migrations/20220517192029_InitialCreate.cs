using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Stott.Optimizely.Csp.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tbl_CspSecurityHeaderSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsXContentTypeOptionsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    IsXXssProtectionEnabled = table.Column<bool>(type: "bit", nullable: false),
                    ReferrerPolicy = table.Column<int>(type: "int", nullable: false),
                    FrameOptions = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_CspSecurityHeaderSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_CspSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    IsReportOnly = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_CspSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_CspSource",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Source = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Directives = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_CspSource", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_CspViolationReport",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Reported = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BlockedUri = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Disposition = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DocumentUri = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EffectiveDirective = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OriginalPolicy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Referrer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ScriptSample = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SourceFile = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ViolatedDirective = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_CspViolationReport", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tbl_CspSecurityHeaderSettings");

            migrationBuilder.DropTable(
                name: "tbl_CspSettings");

            migrationBuilder.DropTable(
                name: "tbl_CspSource");

            migrationBuilder.DropTable(
                name: "tbl_CspViolationReport");
        }
    }
}
