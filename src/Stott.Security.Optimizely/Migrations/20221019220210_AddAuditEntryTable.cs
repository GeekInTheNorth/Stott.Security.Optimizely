using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stott.Security.Optimizely.Migrations
{
    public partial class AddAuditEntryTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tbl_CspAuditEntry",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Actioned = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActionedBy = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    OperationType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecordType = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Field = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OldValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_CspAuditEntry", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "idx_CspAuditEntry_LookUp",
                table: "tbl_CspAuditEntry",
                columns: new[] { "Actioned", "ActionedBy", "RecordType" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tbl_CspAuditEntry");
        }
    }
}
