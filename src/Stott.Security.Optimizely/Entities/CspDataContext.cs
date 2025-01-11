#nullable disable
namespace Stott.Security.Optimizely.Entities;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Features.Audit;

public class CspDataContext : DbContext, ICspDataContext
{
    private readonly ILogger<CspDataContext> _logger;

    public CspDataContext(
        DbContextOptions<CspDataContext> options,
        ILogger<CspDataContext> logger)
        : base(options)
    {
        _logger = logger;
    }

    public DbSet<CspSettings> CspSettings { get; set; }

    public DbSet<CspSource> CspSources { get; set; }

    public DbSet<CspViolationSummary> CspViolations { get; set; }

    public DbSet<SecurityHeaderSettings> SecurityHeaderSettings { get; set; }

    public DbSet<CspSandbox> CspSandboxes { get; set; }

    public DbSet<CorsSettings> CorsSettings { get; set; }

    public DbSet<PermissionPolicy> PermissionPolicies { get; set; }

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
        var entries = ChangeTracker.Entries<IAuditableEntity>().ToList();
        
        foreach (var entry in entries)
        {
            if (!entry.Properties.Any(x => CanAuditProperty(entry.State, x)))
            {
                continue;
            }

            var parent = new AuditHeader
            {
                RecordType = GetRecordType(entry.Entity),
                OperationType = entry.State.ToString(),
                Actioned = entry.Entity.Modified,
                ActionedBy = entry.Entity.ModifiedBy,
                Identifier = GetIdentifier(entry.Entity)
            };

            AuditHeaders.Add(parent);

            foreach(var property in entry.Properties)
            {
                if (CanAuditProperty(entry.State, property))
                {
                    AuditProperties.Add(new AuditProperty
                    {
                        Header = parent,
                        Field = property.Metadata.Name,
                        OldValue = GetOriginalValue(entry.State, property),
                        NewValue = GetUpdatedValue(entry.State, property)
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
            CspSandbox _ => "CSP Sandbox",
            CorsSettings _ => "CORS Settings",
            SecurityHeaderSettings _ => "Security Header Settings",
            PermissionPolicy _ => "Permission Policy",
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

    private static bool CanAuditProperty(EntityState state, PropertyEntry property)
    {
        if (string.Equals(nameof(IAuditableEntity.Id), property.Metadata.Name, StringComparison.InvariantCultureIgnoreCase) ||
            string.Equals(nameof(IAuditableEntity.Modified), property.Metadata.Name, StringComparison.InvariantCultureIgnoreCase) ||
            string.Equals(nameof(IAuditableEntity.ModifiedBy), property.Metadata.Name, StringComparison.InvariantCultureIgnoreCase))
        {
            return false;
        }

        return state == EntityState.Added ||
               state == EntityState.Deleted ||
               (state == EntityState.Modified && property.IsModified);
    }

    private static string GetOriginalValue(EntityState state, PropertyEntry property)
    {
        if (state == EntityState.Added)
        {
            return string.Empty;
        }

        return property.OriginalValue?.ToString();
    }

    private static string GetUpdatedValue(EntityState state, PropertyEntry property)
    {
        if (state == EntityState.Deleted)
        {
            return string.Empty;
        }

        return property.CurrentValue?.ToString();
    }
}