using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stott.Security.Optimizely.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomHeaders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tbl_CspCustomHeader",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HeaderName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Behavior = table.Column<int>(type: "int", nullable: false),
                    HeaderValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_CspCustomHeader", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "idx_HeaderName_LookUp",
                table: "tbl_CspCustomHeader",
                column: "HeaderName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tbl_CspCustomHeader");
        }
    }
}
