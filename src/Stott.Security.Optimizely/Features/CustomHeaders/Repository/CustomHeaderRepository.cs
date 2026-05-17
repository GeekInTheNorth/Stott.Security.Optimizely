namespace Stott.Security.Optimizely.Features.CustomHeaders.Repository;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Extensions;

/// <summary>
/// Repository implementation for custom header data access.
/// </summary>
internal sealed class CustomHeaderRepository : ICustomHeaderRepository
{
    private readonly Lazy<ICspDataContext> _context;

    public CustomHeaderRepository(Lazy<ICspDataContext> context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CustomHeader>> GetAllAsync(Guid? siteId, string? hostName)
    {
        var normalisedSite = siteId == Guid.Empty ? null : siteId;
        var normalisedHost = string.IsNullOrWhiteSpace(hostName) ? null : hostName;

        var data = await GetHeadersInFallbackChainAsync(normalisedSite, normalisedHost);

        return data.OrderBy(x => x.HeaderName).ToList();
    }

    public async Task<IEnumerable<CustomHeader>?> GetAllByContextAsync(Guid? siteId, string? hostName)
    {
        var normalisedSite = siteId == Guid.Empty ? null : siteId;
        var normalisedHost = string.IsNullOrWhiteSpace(hostName) ? null : hostName;

        var data = await _context.Value.CustomHeaders
            .AsNoTracking()
            .Where(x => x.SiteId == normalisedSite && x.HostName == normalisedHost)
            .OrderBy(x => x.HeaderName)
            .ToListAsync();

        return data.Count > 0 ? data : null;
    }

    public async Task<CustomHeader?> GetByIdAsync(Guid id)
    {
        return await _context.Value.CustomHeaders.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<CustomHeader?> GetByHeaderNameAsync(string headerName, Guid? siteId, string? hostName)
    {
        if (string.IsNullOrWhiteSpace(headerName))
        {
            return null;
        }

        var normalisedSite = siteId == Guid.Empty ? null : siteId;
        var normalisedHost = string.IsNullOrWhiteSpace(hostName) ? null : hostName;

        return await _context.Value.CustomHeaders
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.HeaderName == headerName && x.SiteId == normalisedSite && x.HostName == normalisedHost);
    }

    public async Task SaveAsync(ICustomHeader model, string modifiedBy, Guid? siteId, string? hostName)
    {
        var normalisedSiteId = siteId.GetSanitizedSiteId();
        var normalisedHost = hostName.GetSanitizedHostDomain();

        CustomHeader? recordToSave = null;

        if (!Guid.Empty.Equals(model.Id))
        {
            recordToSave = await _context.Value.CustomHeaders.FirstOrDefaultAsync(x => x.Id == model.Id);
        }

        if (recordToSave is null && !string.IsNullOrWhiteSpace(model.HeaderName))
        {
            recordToSave = await _context.Value.CustomHeaders.FirstOrDefaultAsync(x => x.HeaderName == model.HeaderName && x.SiteId == normalisedSiteId && x.HostName == normalisedHost);
        }

        if (recordToSave is null)
        {
            recordToSave = new CustomHeader
            {
                Id = Guid.NewGuid(),
                SiteId = normalisedSiteId,
                HostName = normalisedHost
            };
            _context.Value.CustomHeaders.Add(recordToSave);
        }

        CustomHeaderMapper.ToEntity(model, recordToSave);
        recordToSave.Modified = DateTime.UtcNow;
        recordToSave.ModifiedBy = modifiedBy;

        await _context.Value.SaveChangesAsync();
    }

    public async Task CreateOverrideAsync(Guid? sourceSiteId, string? sourceHostName, Guid? targetSiteId, string? targetHostName, string modifiedBy)
    {
        if (string.IsNullOrWhiteSpace(modifiedBy)) throw new ArgumentNullException(nameof(modifiedBy));

        var normalisedTargetSite = targetSiteId.GetSanitizedSiteId();
        var normalisedTargetHost = targetHostName.GetSanitizedHostDomain();

        // Check if target already has headers
        var existingTarget = await _context.Value.CustomHeaders.AnyAsync(x => x.SiteId == normalisedTargetSite && x.HostName == normalisedTargetHost);
        if (existingTarget)
        {
            return;
        }

        // Load headers from source context using fallback
        var normalisedSourceSite = sourceSiteId.GetSanitizedSiteId();
        var normalisedSourceHost = sourceHostName.GetSanitizedHostDomain();

        var now = DateTime.UtcNow;
        var sourceHeaders = await GetHeadersInFallbackChainAsync(normalisedSourceSite, normalisedSourceHost);
        if (sourceHeaders is not { Count: > 0 })
        {
            return;
        }

        foreach (var source in sourceHeaders)
        {
            _context.Value.CustomHeaders.Add(new CustomHeader
            {
                Id = Guid.NewGuid(),
                HeaderName = source.HeaderName,
                Behavior = source.Behavior,
                HeaderValue = source.HeaderValue,
                SiteId = normalisedTargetSite,
                HostName = normalisedTargetHost,
                Modified = now,
                ModifiedBy = modifiedBy
            });
        }

        await _context.Value.SaveChangesAsync();
    }

    public async Task DeleteByContextAsync(Guid? siteId, string? hostName, string deletedBy)
    {
        var normalisedSite = siteId.GetSanitizedSiteId();
        var normalisedHost = hostName.GetSanitizedHostDomain();

        // Delete by context only supports site level and host level records, global records should not be deleted as they serve as fallback
        if (normalisedSite == null)
        {
            return;
        }

        var records = await _context.Value.CustomHeaders.Where(x => x.SiteId == normalisedSite && x.HostName == normalisedHost).ToListAsync();
        if (records.Count > 0)
        {
            var now = DateTime.UtcNow;
            foreach (var record in records)
            {
                record.Modified = now;
                record.ModifiedBy = deletedBy;
            }

            _context.Value.CustomHeaders.RemoveRange(records);
            await _context.Value.SaveChangesAsync();
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        var recordToDelete = await _context.Value.CustomHeaders.FirstOrDefaultAsync(x => x.Id == id);
        if (recordToDelete is not null)
        {
            _context.Value.CustomHeaders.Remove(recordToDelete);
            await _context.Value.SaveChangesAsync();
        }
    }

    private async Task<List<CustomHeader>> GetHeadersInFallbackChainAsync(Guid? siteId, string? hostName)
    {
        var normalisedSite = siteId.GetSanitizedSiteId();
        var normalisedHost = hostName.GetSanitizedHostDomain();

        // Check exact match (host level)
        if (normalisedSite.HasValue && !string.IsNullOrWhiteSpace(normalisedHost))
        {
            var hostLevelHeaders = await _context.Value.CustomHeaders
                .AsNoTracking()
                .Where(x => x.SiteId == normalisedSite && x.HostName == normalisedHost)
                .ToListAsync();
            if (hostLevelHeaders is { Count: > 0 })
            {
                return hostLevelHeaders;
            }
        }

        // Check site level
        if (normalisedSite.HasValue)
        {
            var siteLevelHeaders = await _context.Value.CustomHeaders
                .AsNoTracking()
                .Where(x => x.SiteId == normalisedSite && x.HostName == null)
                .ToListAsync();
            if (siteLevelHeaders is { Count: > 0 })
            {
                return siteLevelHeaders;
            }
        }

        // Fall back to global
        return await _context.Value.CustomHeaders
            .AsNoTracking()
            .Where(x => x.SiteId == null && x.HostName == null)
            .ToListAsync();
    }
}
