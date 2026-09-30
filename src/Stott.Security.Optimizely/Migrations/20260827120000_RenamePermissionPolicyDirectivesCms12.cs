using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stott.Security.Optimizely.Migrations
{
    /// <inheritdoc />
    public partial class RenamePermissionPolicyDirectivesCms12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            RenameDirective(migrationBuilder, "identity-credentials", "identity-credentials-get");
            RenameDirective(migrationBuilder, "opt-credentials", "otp-credentials");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            RenameDirective(migrationBuilder, "identity-credentials-get", "identity-credentials");
            RenameDirective(migrationBuilder, "otp-credentials", "opt-credentials");
        }

        /// <summary>
        /// Renames a directive across every configuration context.
        /// </summary>
        private static void RenameDirective(MigrationBuilder migrationBuilder, string oldName, string newName)
        {
            migrationBuilder.Sql($"UPDATE tbl_stott_permissionpolicy SET Directive = '{newName}' WHERE Directive = '{oldName}'");
        }
    }
}
