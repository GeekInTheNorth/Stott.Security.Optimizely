namespace Stott.Security.Optimizely.Entities;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Features.Audit;

public class CspDataContext : DbContext, ICspDataContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    private readonly ILogger<CspDataContext> _logger;

    public CspDataContext(
        DbContextOptions<CspDataContext> options,
        IHttpContextAccessor httpContextAccessor,
        ILogger<CspDataContext> logger)
        : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public DbSet<CspSettings> CspSettings { get; set; }

    public DbSet<CspSource> CspSources { get; set; }

    public DbSet<CspViolationSummary> CspViolations { get; set; }

    public DbSet<SecurityHeaderSettings> SecurityHeaderSettings { get; set; }

    public DbSet<AuditHeader> AuditHeaders { get; set; }

    public DbSet<AuditProperty> AuditProperties { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuditHeader>()
                    .HasMany(x => x.AuditProperties)
                    .WithOne(x => x.Header)
                    .HasForeignKey(x => x.AuditHeaderId);
    }

    public async Task<int> ExecuteSqlAsync(string sqlCommand, params SqlParameter[] sqlParameters)
    {
        return await Database.ExecuteSqlRawAsync(sqlCommand, sqlParameters);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (ChangeTracker.HasChanges())
        {
            try
            {
                AuditRecords();
            }
            catch(Exception exception)
            {
                _logger.LogError(exception, $"{CspConstants.LogPrefix} Failed to create audit entry records");
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }

    public void AuditRecords()
    {
        var user = _httpContextAccessor.HttpContext.User.Identity.Name ?? "System";
        var auditTime = DateTime.UtcNow;
        var entries = ChangeTracker.Entries<IAuditableEntity>().ToList();
        
        foreach (var entry in entries)
        {
            var parent = new AuditHeader
            {
                RecordType = GetRecordType(entry.Entity),
                OperationType = entry.State.ToString(),
                Actioned = auditTime,
                ActionedBy = user,
                Identifier = GetIdentifier(entry.Entity)
            };

            AuditHeaders.Add(parent);

            foreach(var property in entry.Properties)
            {
                if (property.IsModified && !string.Equals("Id", property.Metadata.Name, StringComparison.InvariantCultureIgnoreCase))
                {
                    AuditProperties.Add(new AuditProperty
                    {
                        Header = parent,
                        Field = property.Metadata.Name,
                        OldValue = property.OriginalValue?.ToString(),
                        NewValue = property.CurrentValue?.ToString()
                    });
                }
            }
        }
    }

    private static string GetRecordType(IAuditableEntity entity)
    {
        return entity switch
        {
            CspSettings _ => "CSP Settings",
            CspSource _ => "CSP Source",
            SecurityHeaderSettings _ => "Security Header Settings",
            _ => string.Empty,
        };
    }

    private static string GetIdentifier(IAuditableEntity entity)
    {
        return entity switch
        {
            CspSource cspSource => cspSource.Source,
            _ => string.Empty
        };
    }
}