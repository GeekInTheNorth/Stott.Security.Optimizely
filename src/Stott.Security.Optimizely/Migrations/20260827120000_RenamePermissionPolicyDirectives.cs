using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stott.Security.Optimizely.Migrations
{
    /// <inheritdoc />
    public partial class RenamePermissionPolicyDirectives : Migration
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
        /// Renames a directive across every configuration context, skipping any record where the target
        /// name is already configured for the same context in order to avoid creating duplicates.
        /// </summary>
        private static void RenameDirective(MigrationBuilder migrationBuilder, string oldName, string newName)
        {
            migrationBuilder.Sql($@"
UPDATE p
SET p.Directive = '{newName}'
FROM tbl_StottV7_PermissionPolicy p
WHERE p.Directive = '{oldName}'
AND NOT EXISTS (
    SELECT 1
    FROM tbl_StottV7_PermissionPolicy x
    WHERE x.Directive = '{newName}'
    AND ((x.AppId IS NULL AND p.AppId IS NULL) OR x.AppId = p.AppId)
    AND ((x.HostName IS NULL AND p.HostName IS NULL) OR x.HostName = p.HostName));");
        }
    }
}
