namespace Stott.Security.Optimizely.Test;

using System.Threading.Tasks;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

using Stott.Security.Optimizely.Entities;

public class TestDataContext : DbContext, ICspDataContext
{
    public TestDataContext(DbContextOptions<TestDataContext> options) : base(options)
    {
    }

    private int _numberOfRecords = 0;

    public DbSet<CspSettings> CspSettings { get; set; }

    public DbSet<CspSource> CspSources { get; set; }

    public DbSet<CspViolationSummary> CspViolations { get; set; }

    public DbSet<CspSandbox> CspSandboxes { get; set; }

    public DbSet<SecurityHeaderSettings> SecurityHeaderSettings { get; set; }

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

        var allSettings = await CspSettings.ToListAsync();
        CspSettings.RemoveRange(allSettings);

        var allSources = await CspSources.ToListAsync();
        CspSources.RemoveRange(allSources);

        var allViolations = await CspViolations.ToListAsync();
        CspViolations.RemoveRange(allViolations);

        var allSandboxes = await CspSandboxes.ToListAsync();
        CspSandboxes.RemoveRange(allSandboxes);

        var allHeaders = await SecurityHeaderSettings.ToListAsync();
        SecurityHeaderSettings.RemoveRange(allHeaders);

        var allAuditHeaders = await AuditHeaders.ToListAsync();
        AuditHeaders.RemoveRange(allAuditHeaders);

        var allAuditProperties = await AuditProperties.ToListAsync();
        AuditProperties.RemoveRange(allAuditProperties);

        SaveChanges();
    }
}
