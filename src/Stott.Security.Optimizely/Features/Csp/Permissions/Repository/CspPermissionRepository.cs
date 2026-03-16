namespace Stott.Security.Optimizely.Features.Csp.Permissions.Repository;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Entities.Exceptions;

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

    public async Task<IList<CspSource>> GetAsync(string? appId, string? hostName)
    {
        // Returns merged sources across the inheritance chain:
        // Global (AppId=null, HostName=null) always included
        // + Application level (AppId=appId, HostName=null) if appId is set
        // + Host level (AppId=appId, HostName=hostName) if both are set
        var query = _context.Value.CspSources.AsQueryable();

        if (!string.IsNullOrWhiteSpace(appId) && !string.IsNullOrWhiteSpace(hostName))
        {
            query = query.Where(x =>
                (x.AppId == null && x.HostName == null) ||
                (x.AppId == appId && x.HostName == null) ||
                (x.AppId == appId && x.HostName == hostName));
        }
        else if (!string.IsNullOrWhiteSpace(appId))
        {
            query = query.Where(x =>
                (x.AppId == null && x.HostName == null) ||
                (x.AppId == appId && x.HostName == null));
        }
        else
        {
            query = query.Where(x => x.AppId == null && x.HostName == null);
        }

        var sources = await query.ToListAsync();

        return sources ?? new List<CspSource>(0);
    }

    public async Task<IList<CspSource>> GetByContextAsync(string? appId, string? hostName)
    {
        // Returns only sources at the exact context level (not inherited)
        var sources = await _context.Value.CspSources
            .Where(x => x.AppId == appId && x.HostName == hostName)
            .ToListAsync();

        return sources ?? new List<CspSource>(0);
    }

    public async Task<CspSource?> GetBySourceAsync(string? source, string? appId, string? hostName)
    {
        if (string.IsNullOrWhiteSpace(source))
        {
            return null;
        }

        return await _context.Value.CspSources
            .FirstOrDefaultAsync(x => x.Source == source && x.AppId == appId && x.HostName == hostName);
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

    public async Task SaveAsync(Guid id, string source, List<string> directives, string modifiedBy, string? appId, string? hostName)
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
        await SaveAsync(id, source, combinedDirectives, modifiedBy, appId, hostName);
    }

    public async Task AppendDirectiveAsync(string source, string directive, string modifiedBy, string? appId, string? hostName)
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

        var matchingSource = await _context.Value.CspSources
            .FirstOrDefaultAsync(x => x.Source == source && x.AppId == appId && x.HostName == hostName);

        if (matchingSource == null)
        {
            _context.Value.CspSources.Add(new CspSource
            {
                Source = source,
                Directives = directive,
                AppId = appId,
                HostName = hostName,
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

    private async Task SaveAsync(Guid id, string source, string directives, string modifiedBy, string? appId, string? hostName)
    {
        var matchingSource = await _context.Value.CspSources
            .FirstOrDefaultAsync(x => x.Source == source && x.AppId == appId && x.HostName == hostName);
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
                AppId = appId,
                HostName = hostName
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
}
