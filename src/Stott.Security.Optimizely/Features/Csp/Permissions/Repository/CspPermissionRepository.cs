namespace Stott.Security.Optimizely.Features.Csp.Permissions.Repository;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Entities.Exceptions;
using Stott.Security.Optimizely.Extensions;

internal sealed class CspPermissionRepository : ICspPermissionRepository
{
    private readonly Lazy<ICspDataContext> _context;

    public CspPermissionRepository(Lazy<ICspDataContext> context)
    {
        _context = context;
    }

    public async Task<IList<CspSource>> GetAllAsync()
    {
        var sources = await _context.Value.CspSources.ToListAsync();
        return sources ?? new List<CspSource>(0);
    }

    public async Task<IList<CspSource>> GetAsync(Guid? siteId, string? hostName)
    {
        var normalisedSite = siteId == Guid.Empty ? null : siteId;
        var normalisedHost = string.IsNullOrWhiteSpace(hostName) ? null : hostName;

        // Load from DB: global sources (no SiteId) + sources matching the given SiteId
        var sources = await _context.Value.CspSources
            .Where(x => x.SiteId == null || x.SiteId == normalisedSite)
            .AsNoTracking()
            .ToListAsync();

        // Filter in-memory using host comparison for HostName matching
        return sources.Where(x => IsGlobalSource(x) || IsSiteSource(x, normalisedSite) || IsHostSource(x, normalisedSite, normalisedHost)).ToList();
    }

    public async Task<CspSource?> GetBySourceAsync(string? source, Guid? siteId, string? hostName)
    {
        if (string.IsNullOrWhiteSpace(source))
        {
            return null;
        }

        var normalisedSite = siteId == Guid.Empty ? null : siteId;
        var normalisedHost = string.IsNullOrWhiteSpace(hostName) ? null : hostName;

        var candidates = await _context.Value.CspSources
            .Where(x => x.Source == source && (x.SiteId == null || x.SiteId == normalisedSite))
            .AsNoTracking()
            .ToListAsync();

        return candidates.FirstOrDefault(x => IsGlobalSource(x) || IsSiteSource(x, normalisedSite) || IsHostSource(x, normalisedSite, normalisedHost));
    }

    public async Task DeleteAsync(Guid id, string deletedBy)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException($"{nameof(id)} should not be empty", nameof(id));
        }

        if (string.IsNullOrWhiteSpace(deletedBy))
        {
            throw new ArgumentNullException(nameof(deletedBy));
        }

        var existingRecord = await _context.Value.CspSources.FirstOrDefaultAsync(x => x.Id == id);
        if (existingRecord != null)
        {
            existingRecord.Modified = DateTime.UtcNow;
            existingRecord.ModifiedBy = deletedBy;

            _context.Value.CspSources.Remove(existingRecord);
            await _context.Value.SaveChangesAsync();
        }
    }

    public async Task SaveAsync(Guid id, string source, List<string> directives, string modifiedBy, Guid? siteId, string? hostName)
    {
        if (string.IsNullOrWhiteSpace(source))
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (directives == null || !directives.Any())
        {
            throw new ArgumentException($"{directives} must not be null or empty.", nameof(directives));
        }

        if (string.IsNullOrWhiteSpace(modifiedBy))
        {
            throw new ArgumentNullException(nameof(modifiedBy));
        }

        var combinedDirectives = string.Join(",", directives);
        await SaveAsync(id, source, combinedDirectives, modifiedBy, siteId, hostName);
    }

    public async Task AppendDirectiveAsync(string source, string directive, string modifiedBy, Guid? siteId, string? hostName)
    {
        if (string.IsNullOrWhiteSpace(source))
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (string.IsNullOrWhiteSpace(directive))
        {
            throw new ArgumentNullException(nameof(directive));
        }

        if (string.IsNullOrWhiteSpace(modifiedBy))
        {
            throw new ArgumentNullException(nameof(modifiedBy));
        }

        var normalisedSite = siteId == Guid.Empty ? null : siteId;
        var normalisedHost = string.IsNullOrWhiteSpace(hostName) ? null : hostName;

        var matchingSource = await _context.Value.CspSources
            .FirstOrDefaultAsync(x => x.Source == source && x.SiteId == normalisedSite && x.HostName == normalisedHost);

        if (matchingSource == null)
        {
            _context.Value.CspSources.Add(new CspSource
            {
                Source = source,
                Directives = directive,
                SiteId = normalisedSite,
                HostName = normalisedHost,
                Modified = DateTime.UtcNow,
                ModifiedBy = modifiedBy
            });
        }
        else if (!HasDirective(matchingSource.Directives, directive))
        {
            matchingSource.Directives = $"{matchingSource.Directives},{directive}";
            matchingSource.Modified = DateTime.UtcNow;
            matchingSource.ModifiedBy = modifiedBy;
        }

        await _context.Value.SaveChangesAsync();
    }

    private async Task SaveAsync(Guid id, string source, string directives, string modifiedBy, Guid? siteId, string? hostName)
    {
        var normalisedSite = siteId == Guid.Empty ? null : siteId;
        var normalisedHost = string.IsNullOrWhiteSpace(hostName) ? null : hostName;

        var matchingSource = await _context.Value.CspSources
            .FirstOrDefaultAsync(x => x.Source == source && x.SiteId == normalisedSite && x.HostName == normalisedHost);
        if (matchingSource != null && !matchingSource.Id.Equals(id))
        {
            throw new EntityExistsException($"{CspConstants.LogPrefix} An entry already exists for the source of '{source}'.");
        }

        var recordToSave = await _context.Value.CspSources.FirstOrDefaultAsync(x => x.Id.Equals(id));
        if (recordToSave == null)
        {
            recordToSave = new CspSource
            {
                Source = source,
                Directives = directives,
                SiteId = normalisedSite,
                HostName = normalisedHost
            };

            _context.Value.CspSources.Add(recordToSave);
        }

        recordToSave.Source = source;
        recordToSave.Directives = directives;
        recordToSave.Modified = DateTime.UtcNow;
        recordToSave.ModifiedBy = modifiedBy;

        await _context.Value.SaveChangesAsync();
    }

    private static bool HasDirective(string? currentDirectives, string? directive)
    {
        if (string.IsNullOrWhiteSpace(currentDirectives)) return false;

        if (string.IsNullOrWhiteSpace(directive)) return true;

        return currentDirectives.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                                .Contains(directive);
    }

    private static bool IsGlobalSource(CspSource source)
    {
        return source.SiteId == null && string.IsNullOrWhiteSpace(source.HostName);
    }

    private static bool IsSiteSource(CspSource source, Guid? siteId)
    {
        if (!siteId.HasValue)
        {
            return false;
        }

        return source.SiteId == siteId && string.IsNullOrWhiteSpace(source.HostName);
    }

    private static bool IsHostSource(CspSource source, Guid? siteId, string? hostName)
    {
        if (!siteId.HasValue || string.IsNullOrWhiteSpace(hostName))
        {
            return false;
        }

        return source.SiteId == siteId && HostsMatch(source.HostName, hostName);
    }

    private static bool HostsMatch(string? storedHostName, string? requestedHostName)
    {
        var storedEmpty = string.IsNullOrWhiteSpace(storedHostName);
        var requestedEmpty = string.IsNullOrWhiteSpace(requestedHostName);

        if (storedEmpty && requestedEmpty)
        {
            return true;
        }

        if (storedEmpty || requestedEmpty)
        {
            return false;
        }

        var storedHost = storedHostName.GetSanitizedHostDomain();
        var requestedHost = requestedHostName.GetSanitizedHostDomain();

        return string.Equals(storedHost, requestedHost, StringComparison.OrdinalIgnoreCase);
    }
}
