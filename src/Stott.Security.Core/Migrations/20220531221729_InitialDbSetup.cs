using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Stott.Security.Core.Migrations
{
    public partial class InitialDbSetup : Migration
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
                name: "tbl_CspViolationSummary",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BlockedUri = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ViolatedDirective = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LastReported = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Instances = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_CspViolationSummary", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "idx_CspViolationSummary_LookUp",
                table: "tbl_CspViolationSummary",
                columns: new[] { "BlockedUri", "ViolatedDirective" });
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
                name: "tbl_CspViolationSummary");
        }
    }
}
