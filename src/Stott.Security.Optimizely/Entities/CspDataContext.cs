#nullable disable
namespace Stott.Security.Optimizely.Entities;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using EPiServer.Web;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Features.Audit;

public class CspDataContext : DbContext, ICspDataContext
{
    private readonly ISiteDefinitionRepository _siteDefinitionRepository;

    private readonly ILogger<CspDataContext> _logger;

    public CspDataContext(
        DbContextOptions<CspDataContext> options,
        ISiteDefinitionRepository siteDefinitionRepository,
        ILogger<CspDataContext> logger)
        : base(options)
    {
        _siteDefinitionRepository = siteDefinitionRepository;
        _logger = logger;
        Debug.WriteLine($"CspDataContext created: {DateTime.UtcNow}");
    }

    public DbSet<CspSettings> CspSettings { get; set; }

    public DbSet<CspSource> CspSources { get; set; }

    public DbSet<CspViolationSummary> CspViolations { get; set; }

    public DbSet<SecurityHeaderSettings> SecurityHeaderSettings { get; set; }

    public DbSet<CspSandbox> CspSandboxes { get; set; }

    public DbSet<CorsSettings> CorsSettings { get; set; }

    public DbSet<PermissionPolicySettings> PermissionPolicySettings { get; set; }

    public DbSet<PermissionPolicy> PermissionPolicies { get; set; }

    public DbSet<AuditHeader> AuditHeaders { get; set; }

    public DbSet<AuditProperty> AuditProperties { get; set; }

    public DbSet<CustomHeader> CustomHeaders { get; set; }

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
        var sites = _siteDefinitionRepository.List().ToDictionary(x => x.Id, y => y.Name);
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
                Identifier = GetIdentifier(entry.Entity, sites)
            };

            AuditHeaders.Add(parent);

            foreach(var property in entry.Properties)
            {
                if (CanAuditProperty(entry.State, property))
                {
                    var isSiteId = string.Equals(nameof(Entities.CspSettings.SiteId), property.Metadata.Name, StringComparison.InvariantCultureIgnoreCase);
                    AuditProperties.Add(new AuditProperty
                    {
                        Header = parent,
                        Field = property.Metadata.Name,
                        OldValue = GetOriginalValue(entry.State, property, isSiteId, sites),
                        NewValue = GetUpdatedValue(entry.State, property, isSiteId, sites)
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
            PermissionPolicy _ => "Permission Policy Directive",
            PermissionPolicySettings _ => "Permission Policy Settings",
            CustomHeader _ => "Response Header",
            _ => string.Empty,
        };
    }

    private static string GetIdentifier(IAuditableEntity entity, Dictionary<Guid, string> sites)
    {
        return entity switch
        {
            CspSource cspSource => FormatContextIdentifier(cspSource.Source, cspSource.SiteId, cspSource.HostName, sites),
            CspSettings cspSettings => FormatContextIdentifier("CSP Settings", cspSettings.SiteId, cspSettings.HostName, sites),
            CspSandbox cspSandbox => FormatContextIdentifier("CSP Sandbox", cspSandbox.SiteId, cspSandbox.HostName, sites),
            PermissionPolicy permissionPolicy => FormatContextIdentifier(permissionPolicy.Directive, permissionPolicy.SiteId, permissionPolicy.HostName, sites),
            PermissionPolicySettings ppSettings => FormatContextIdentifier("Permission Policy Settings", ppSettings.SiteId, ppSettings.HostName, sites),
            CustomHeader customHeader => FormatContextIdentifier(customHeader.HeaderName, customHeader.SiteId, customHeader.HostName, sites),
            _ => string.Empty
        };
    }

    private static string FormatContextIdentifier(string baseIdentifier, Guid? siteId, string hostName, Dictionary<Guid, string> sites)
    {
        if (siteId is null || Guid.Empty.Equals(siteId))
        {
            return baseIdentifier;
        }

        var siteIdentifier = siteId?.ToString();
        if (sites.TryGetValue(siteId.Value, out var siteName) && !string.IsNullOrWhiteSpace(siteName))
        {
            siteIdentifier = siteName;
        }

        if (string.IsNullOrWhiteSpace(hostName))
        {
            return $"{baseIdentifier} ({siteIdentifier})";
        }

        return $"{baseIdentifier} ({siteIdentifier} - {hostName})";
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

    private static string GetOriginalValue(EntityState state, PropertyEntry property, bool isSiteId, Dictionary<Guid, string> sites)
    {
        if (state == EntityState.Added)
        {
            return string.Empty;
        }

        if (isSiteId && property.OriginalValue is Guid siteId && sites.TryGetValue(siteId, out var siteName))
        {
            return $"{siteName} ({property.OriginalValue})";
        }

        return property.OriginalValue?.ToString();
    }

    private static string GetUpdatedValue(EntityState state, PropertyEntry property, bool isSiteId, Dictionary<Guid, string> sites)
    {
        if (state == EntityState.Deleted)
        {
            return string.Empty;
        }

        if (isSiteId && property.CurrentValue is Guid siteId && sites.TryGetValue(siteId, out var siteName))
        {
            return $"{siteName} ({property.CurrentValue})";
        }

        return property.CurrentValue?.ToString();
    }
}