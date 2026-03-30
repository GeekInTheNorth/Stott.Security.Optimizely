using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stott.Security.Optimizely.Migrations
{
    /// <inheritdoc />
    public partial class AddV7Schema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tbl_StottV7_AuditHeader",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Actioned = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActionedBy = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    OperationType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecordType = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Identifier = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_StottV7_AuditHeader", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_StottV7_CorsSettings",
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
                    table.PrimaryKey("PK_tbl_StottV7_CorsSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_StottV7_CspSandbox",
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
                    AppId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    HostName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_StottV7_CspSandbox", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_StottV7_CspSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    IsReportOnly = table.Column<bool>(type: "bit", nullable: false),
                    IsWhitelistEnabled = table.Column<bool>(type: "bit", nullable: false),
                    WhitelistUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsUpgradeInsecureRequestsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    IsNonceEnabled = table.Column<bool>(type: "bit", nullable: false),
                    IsStrictDynamicEnabled = table.Column<bool>(type: "bit", nullable: false),
                    UseInternalReporting = table.Column<bool>(type: "bit", nullable: false),
                    UseExternalReporting = table.Column<bool>(type: "bit", nullable: false),
                    ExternalReportToUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExternalReportUriUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AppId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    HostName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_StottV7_CspSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_StottV7_CspSource",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Source = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Directives = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AppId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    HostName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_StottV7_CspSource", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_StottV7_CspViolationSummary",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BlockedUri = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ViolatedDirective = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AppId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    HostName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    LastReported = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Instances = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_StottV7_CspViolationSummary", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_StottV7_CustomHeader",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HeaderName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Behavior = table.Column<int>(type: "int", nullable: false),
                    HeaderValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AppId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    HostName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_StottV7_CustomHeader", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_StottV7_PermissionPolicy",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Directive = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EnabledState = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Origins = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AppId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    HostName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_StottV7_PermissionPolicy", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_StottV7_PermissionPolicySettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AppId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    HostName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_StottV7_PermissionPolicySettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_StottV7_AuditProperty",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AuditHeaderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Field = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OldValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_StottV7_AuditProperty", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tbl_StottV7_AuditProperty_tbl_StottV7_AuditHeader_AuditHeaderId",
                        column: x => x.AuditHeaderId,
                        principalTable: "tbl_StottV7_AuditHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "idx_StottV7_AuditHeader_LookUp",
                table: "tbl_StottV7_AuditHeader",
                columns: new[] { "Actioned", "ActionedBy", "RecordType" });

            migrationBuilder.CreateIndex(
                name: "idx_StottV7_AuditProperty_LookUp",
                table: "tbl_StottV7_AuditProperty",
                column: "AuditHeaderId");

            migrationBuilder.CreateIndex(
                name: "idx_StottV7_CspSandbox_LookUp",
                table: "tbl_StottV7_CspSandbox",
                columns: new[] { "AppId", "HostName" });

            migrationBuilder.CreateIndex(
                name: "idx_StottV7_CspSettings_LookUp",
                table: "tbl_StottV7_CspSettings",
                columns: new[] { "AppId", "HostName" });

            migrationBuilder.CreateIndex(
                name: "idx_StottV7_CspSource_LookUp",
                table: "tbl_StottV7_CspSource",
                columns: new[] { "Source", "AppId", "HostName" });

            migrationBuilder.CreateIndex(
                name: "idx_StottV7_CspViolationSummary_LookUp",
                table: "tbl_StottV7_CspViolationSummary",
                columns: new[] { "BlockedUri", "ViolatedDirective", "AppId", "HostName" });

            migrationBuilder.CreateIndex(
                name: "idx_StottV7_CustomHeader_LookUp",
                table: "tbl_StottV7_CustomHeader",
                columns: new[] { "HeaderName", "AppId", "HostName" });

            migrationBuilder.CreateIndex(
                name: "idx_StottV7_PermissionPolicy_LookUp",
                table: "tbl_StottV7_PermissionPolicy",
                columns: new[] { "Directive", "AppId", "HostName" });

            migrationBuilder.CreateIndex(
                name: "idx_StottV7_PermissionPolicySettings_LookUp",
                table: "tbl_StottV7_PermissionPolicySettings",
                columns: new[] { "AppId", "HostName" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tbl_StottV7_AuditProperty");

            migrationBuilder.DropTable(
                name: "tbl_StottV7_CorsSettings");

            migrationBuilder.DropTable(
                name: "tbl_StottV7_CspSandbox");

            migrationBuilder.DropTable(
                name: "tbl_StottV7_CspSettings");

            migrationBuilder.DropTable(
                name: "tbl_StottV7_CspSource");

            migrationBuilder.DropTable(
                name: "tbl_StottV7_CspViolationSummary");

            migrationBuilder.DropTable(
                name: "tbl_StottV7_CustomHeader");

            migrationBuilder.DropTable(
                name: "tbl_StottV7_PermissionPolicy");

            migrationBuilder.DropTable(
                name: "tbl_StottV7_PermissionPolicySettings");

            migrationBuilder.DropTable(
                name: "tbl_StottV7_AuditHeader");
        }
    }
}
