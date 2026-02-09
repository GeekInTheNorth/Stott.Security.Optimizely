namespace Stott.Security.Optimizely.Test;

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.Audit;

public class TestDataContext : DbContext, ICspDataContext
{
    public TestDataContext(DbContextOptions<TestDataContext> options) : base(options)
    {
        RecordsUpdated = [];
    }

    private int _numberOfRecords = 0;

    public List<string> RecordsUpdated { get; set; }

    public DbSet<CspSettings> CspSettings { get; set; }

    public DbSet<CspSource> CspSources { get; set; }

    public DbSet<CspViolationSummary> CspViolations { get; set; }

    public DbSet<CspSandbox> CspSandboxes { get; set; }

    public DbSet<CorsSettings> CorsSettings { get; set; }

    public DbSet<SecurityHeaderSettings> SecurityHeaderSettings { get; set; }

    public DbSet<CustomHeader> CustomHeaders { get; set; }

    public DbSet<PermissionPolicySettings> PermissionPolicySettings { get; set; }

    public DbSet<PermissionPolicy> PermissionPolicies { get; set; }

    public DbSet<AuditHeader> AuditHeaders { get; set; }

    public DbSet<AuditProperty> AuditProperties { get; set; }

    public void SetExecuteSqlAsyncResult(int numberOfRecords)
    {
        _numberOfRecords = numberOfRecords;
    }

    public Task<int> ExecuteSqlAsync(string sqlCommand, params SqlParameter[] sqlParameters)
    {
        return Task.FromResult(_numberOfRecords);
    }

    public async Task Reset()
    {
        _numberOfRecords = 0;
        RecordsUpdated.Clear();

        var allSettings = await CspSettings.ToListAsync();
        CspSettings.RemoveRange(allSettings);

        var allSources = await CspSources.ToListAsync();
        CspSources.RemoveRange(allSources);

        var allViolations = await CspViolations.ToListAsync();
        CspViolations.RemoveRange(allViolations);

        var allSandboxes = await CspSandboxes.ToListAsync();
        CspSandboxes.RemoveRange(allSandboxes);

        var allCors = await CorsSettings.ToListAsync();
        CorsSettings.RemoveRange(allCors);

        var allHeaders = await SecurityHeaderSettings.ToListAsync();
        SecurityHeaderSettings.RemoveRange(allHeaders);

        var allCustomHeaders = await CustomHeaders.ToListAsync();
        CustomHeaders.RemoveRange(allCustomHeaders);

        var allPolicySettings = await PermissionPolicySettings.ToListAsync();
        PermissionPolicySettings.RemoveRange(allPolicySettings);

        var allPermissionPolicies = await PermissionPolicies.ToListAsync();
        PermissionPolicies.RemoveRange(allPermissionPolicies);

        var allAuditHeaders = await AuditHeaders.ToListAsync();
        AuditHeaders.RemoveRange(allAuditHeaders);

        var allAuditProperties = await AuditProperties.ToListAsync();
        AuditProperties.RemoveRange(allAuditProperties);

        SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        RecordsUpdated = ChangeTracker.Entries<IAuditableEntity>().Select(x => x.GetType().Name).ToList();

        return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    public void ClearTracking()
    {
        RecordsUpdated.Clear();
        ChangeTracker.Clear();
    }
}
