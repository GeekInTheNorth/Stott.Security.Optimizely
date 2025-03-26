﻿#nullable disable
namespace Stott.Security.Optimizely.Entities;

using System.Threading;
using System.Threading.Tasks;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

public interface ICspDataContext
{
    DbSet<CspSettings> CspSettings { get; set; }

    DbSet<CspSource> CspSources { get; set; }

    DbSet<CspViolationSummary> CspViolations { get; set; }

    DbSet<SecurityHeaderSettings> SecurityHeaderSettings { get; set; }

    DbSet<CspSandbox> CspSandboxes { get; set; }

    DbSet<CorsSettings> CorsSettings { get; set; }

    DbSet<PermissionPolicySettings> PermissionPolicySettings { get; set; }

    DbSet<PermissionPolicy> PermissionPolicies { get; set; }

    DbSet<AuditHeader> AuditHeaders { get; set; }

    DbSet<AuditProperty> AuditProperties { get; set; }

    Task<int> ExecuteSqlAsync(string sqlCommand, params SqlParameter[] sqlParameters);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    int SaveChanges();
}