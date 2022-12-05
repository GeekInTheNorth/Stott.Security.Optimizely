using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stott.Security.Optimizely.Migrations
{
    public partial class AddSandboxSupport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tbl_CspSandbox",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsSandboxEnabled = table.Column<bool>(type: "bit", nullable: false),
                    IsAllowDownloadsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    IsAllowDownloadsWithoutGestureEnabled = table.Column<bool>(type: "bit", nullable: false),
                    IsAllowFormsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    IsAllowModalsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    IsAllowOrientationLockEnabled = table.Column<bool>(type: "bit", nullable: false),
                    IsAllowPointerLockEnabled = table.Column<bool>(type: "bit", nullable: false),
                    IsAllowPopupsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    IsAllowPopupsToEscapeTheSandboxEnabled = table.Column<bool>(type: "bit", nullable: false),
                    IsAllowPresentationEnabled = table.Column<bool>(type: "bit", nullable: false),
                    IsAllowSameOriginEnabled = table.Column<bool>(type: "bit", nullable: false),
                    IsAllowScriptsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    IsAllowStorageAccessByUserEnabled = table.Column<bool>(type: "bit", nullable: false),
                    IsAllowTopNavigationEnabled = table.Column<bool>(type: "bit", nullable: false),
                    IsAllowTopNavigationByUserEnabled = table.Column<bool>(type: "bit", nullable: false),
                    IsAllowTopNavigationToCustomProtocolEnabled = table.Column<bool>(type: "bit", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_CspSandbox", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tbl_CspSandbox");
        }
    }
}
