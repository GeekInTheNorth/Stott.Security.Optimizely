using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stott.Security.Optimizely.Migrations
{
    public partial class AddAuditTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tbl_CspAuditHeader",
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
                    table.PrimaryKey("PK_tbl_CspAuditHeader", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_CspAuditProperty",
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
                    table.PrimaryKey("PK_tbl_CspAuditProperty", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tbl_CspAuditProperty_tbl_CspAuditHeader_AuditHeaderId",
                        column: x => x.AuditHeaderId,
                        principalTable: "tbl_CspAuditHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "idx_CspAuditHeader_LookUp",
                table: "tbl_CspAuditHeader",
                columns: new[] { "Actioned", "ActionedBy", "RecordType" });

            migrationBuilder.CreateIndex(
                name: "idx_CspAuditProperty_LookUp",
                table: "tbl_CspAuditProperty",
                column: "AuditHeaderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tbl_CspAuditProperty");

            migrationBuilder.DropTable(
                name: "tbl_CspAuditHeader");
        }
    }
}
