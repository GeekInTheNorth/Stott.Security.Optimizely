namespace Stott.Security.Optimizely.Features.Csp.Permissions.Repository;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Entities.Exceptions;

internal sealed class CspPermissionRepository(Lazy<ICspDataContext> context) : ICspPermissionRepository
{
    public async Task<IList<CspSource>> GetAllAsync()
    {
        var sources = await context.Value.CspSources.ToListAsync();
        return sources ?? [];
    }

    public async Task<IList<CspSource>> GetAsync(string? appId, string? hostName)
    {
        // Load from DB: global sources (no AppId) + sources matching the given AppId
        var sources = await context.Value.CspSources
            .Where(x => x.AppId == null || x.AppId == string.Empty || x.AppId == appId)
            .ToListAsync();

        // Filter in-memory using URI host comparison for HostName matching
        return [.. sources.Where(x => IsGlobalSource(x) || IsAppSource(x, appId) || IsHostSource(x, appId, hostName))];
    }

    public async Task<IList<CspSource>> GetByContextAsync(string? appId, string? hostName)
    {
        // Returns only sources at the exact context level (not inherited)
        var sources = await context.Value.CspSources
            .Where(x => x.AppId == appId && x.HostName == hostName)
            .ToListAsync();

        return sources ?? [];
    }

    public async Task<CspSource?> GetBySourceAsync(string? source, string? appId, string? hostName)
    {
        if (string.IsNullOrWhiteSpace(source))
        {
            return null;
        }

        var candidates = await context.Value.CspSources
            .Where(x => x.Source == source && (x.AppId == null || x.AppId == string.Empty || x.AppId == appId))
            .ToListAsync();

        return candidates.FirstOrDefault(x => IsGlobalSource(x) || IsAppSource(x, appId) || IsHostSource(x, appId, hostName));
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

        var existingRecord = await context.Value.CspSources.FirstOrDefaultAsync(x => x.Id == id);
        if (existingRecord != null)
        {
            existingRecord.Modified = DateTime.UtcNow;
            existingRecord.ModifiedBy = deletedBy;

            context.Value.CspSources.Remove(existingRecord);
            await context.Value.SaveChangesAsync();
        }
    }

    public async Task SaveAsync(Guid id, string source, List<string> directives, string modifiedBy, string? appId, string? hostName)
    {
        if (string.IsNullOrWhiteSpace(source))
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (directives is not { Count: >0 })
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

        var matchingSource = await context.Value.CspSources
            .FirstOrDefaultAsync(x => x.Source == source && x.AppId == appId && x.HostName == hostName);

        if (matchingSource == null)
        {
            context.Value.CspSources.Add(new CspSource
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

        await context.Value.SaveChangesAsync();
    }

    private async Task SaveAsync(Guid id, string source, string directives, string modifiedBy, string? appId, string? hostName)
    {
        var matchingSource = await context.Value.CspSources
            .FirstOrDefaultAsync(x => x.Source == source && x.AppId == appId && x.HostName == hostName);
        if (matchingSource != null && !matchingSource.Id.Equals(id))
        {
            throw new EntityExistsException($"{CspConstants.LogPrefix} An entry already exists for the source of '{source}'.");
        }

        var recordToSave = await context.Value.CspSources.FirstOrDefaultAsync(x => x.Id.Equals(id));
        if (recordToSave == null)
        {
            recordToSave = new CspSource
            {
                Source = source,
                Directives = directives,
                AppId = appId,
                HostName = hostName
            };

            context.Value.CspSources.Add(recordToSave);
        }

        recordToSave.Source = source;
        recordToSave.Directives = directives;
        recordToSave.Modified = DateTime.UtcNow;
        recordToSave.ModifiedBy = modifiedBy;

        await context.Value.SaveChangesAsync();
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
        return string.IsNullOrWhiteSpace(source.AppId) && string.IsNullOrWhiteSpace(source.HostName);
    }

    private static bool IsAppSource(CspSource source, string? appId)
    {
        if (string.IsNullOrWhiteSpace(appId))
        {
            return false;
        }

        return source.AppId == appId && string.IsNullOrWhiteSpace(source.HostName);
    }

    private static bool IsHostSource(CspSource source, string? appId, string? hostName)
    {
        if (string.IsNullOrWhiteSpace(appId) || string.IsNullOrWhiteSpace(hostName))
        {
            return false;
        }

        return source.AppId == appId && HostsMatch(source.HostName, hostName);
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

        var storedHost = GetHost(storedHostName!);
        var requestedHost = GetHost(requestedHostName!);

        return string.Equals(storedHost, requestedHost, StringComparison.OrdinalIgnoreCase);
    }

    private static string? GetHost(string value)
    {
        // Ensure we have a scheme so Uri parses host correctly
        var normalized = value.Contains("://") ? value : $"https://{value}";
        if (Uri.TryCreate(normalized, UriKind.Absolute, out var uri))
        {
            return uri.Host + (uri.IsDefaultPort ? string.Empty : ":" + uri.Port);
        }

        return value.TrimEnd('/');
    }
}
