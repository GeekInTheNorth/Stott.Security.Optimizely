namespace Stott.Security.Optimizely.Features.CustomHeaders.Repository;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Stott.Security.Optimizely.Entities;

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

    public async Task<IEnumerable<CustomHeader>> GetAllAsync(string? appId, string? hostName)
    {
        var data = await GetHeadersInFallbackChainAsync(appId, hostName);

        return data.OrderBy(x => x.HeaderName).ToList();
    }

    public async Task<IEnumerable<CustomHeader>?> GetAllByContextAsync(string? appId, string? hostName)
    {
        var data = await _context.Value.CustomHeaders
            .AsNoTracking()
            .Where(x => x.AppId == appId && x.HostName == hostName)
            .OrderBy(x => x.HeaderName)
            .ToListAsync();

        return data.Count > 0 ? data : null;
    }

    public async Task<CustomHeader?> GetByIdAsync(Guid id)
    {
        return await _context.Value.CustomHeaders.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<CustomHeader?> GetByHeaderNameAsync(string headerName, string? appId, string? hostName)
    {
        if (string.IsNullOrWhiteSpace(headerName))
        {
            return null;
        }

        return await _context.Value.CustomHeaders
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.HeaderName == headerName && x.AppId == appId && x.HostName == hostName);
    }

    public async Task SaveAsync(ICustomHeader model, string modifiedBy, string? appId, string? hostName)
    {
        CustomHeader? recordToSave = null;

        if (!Guid.Empty.Equals(model.Id))
        {
            recordToSave = await _context.Value.CustomHeaders.FirstOrDefaultAsync(x => x.Id == model.Id);
        }

        if (recordToSave is null && !string.IsNullOrWhiteSpace(model.HeaderName))
        {
            recordToSave = await _context.Value.CustomHeaders.FirstOrDefaultAsync(x => x.HeaderName == model.HeaderName && x.AppId == appId && x.HostName == hostName);
        }

        if (recordToSave is null)
        {
            recordToSave = new CustomHeader
            {
                Id = Guid.NewGuid(),
                AppId = appId,
                HostName = hostName
            };
            _context.Value.CustomHeaders.Add(recordToSave);
        }

        CustomHeaderMapper.ToEntity(model, recordToSave);
        recordToSave.Modified = DateTime.UtcNow;
        recordToSave.ModifiedBy = modifiedBy;

        await _context.Value.SaveChangesAsync();
    }

    public async Task CreateOverrideAsync(string? sourceAppId, string? sourceHostName, string? targetAppId, string? targetHostName, string modifiedBy)
    {
        if (string.IsNullOrWhiteSpace(modifiedBy)) throw new ArgumentNullException(nameof(modifiedBy));

        // Check if target already has headers
        var existingTarget = await _context.Value.CustomHeaders.AnyAsync(x => x.AppId == targetAppId && x.HostName == targetHostName);
        if (existingTarget)
        {
            return;
        }

        // Load headers from source context using fallback
        var sourceHeaders = await GetHeadersInFallbackChainAsync(sourceAppId, sourceHostName);
        var now = DateTime.UtcNow;

        foreach (var source in sourceHeaders)
        {
            var copy = new CustomHeader
            {
                Id = Guid.NewGuid(),
                HeaderName = source.HeaderName,
                Behavior = source.Behavior,
                HeaderValue = source.HeaderValue,
                AppId = targetAppId,
                HostName = targetHostName,
                Modified = now,
                ModifiedBy = modifiedBy
            };

            _context.Value.CustomHeaders.Add(copy);
        }

        if (sourceHeaders.Count > 0)
        {
            await _context.Value.SaveChangesAsync();
        }
    }

    public async Task DeleteByContextAsync(string? appId, string? hostName, string deletedBy)
    {
        // Delete by context only supports app level and host level records, global records should not be deleted as they serve as fallback
        if (string.IsNullOrWhiteSpace(appId))
        {
            return;
        }

        var records = await _context.Value.CustomHeaders.Where(x => x.AppId == appId && x.HostName == hostName).ToListAsync();
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

    private async Task<List<CustomHeader>> GetHeadersInFallbackChainAsync(string? appId, string? hostName)
    {
        // Check exact match (host level)
        if (!string.IsNullOrWhiteSpace(appId) && !string.IsNullOrWhiteSpace(hostName))
        {
            var hostLevelHeaders = await _context.Value.CustomHeaders
                .AsNoTracking()
                .Where(x => x.AppId == appId && x.HostName == hostName)
                .ToListAsync();
            if (hostLevelHeaders is { Count: > 0 })
            {
                return hostLevelHeaders;
            }
        }

        // Check app level
        if (!string.IsNullOrWhiteSpace(appId))
        {
            var appLevelHeaders = await _context.Value.CustomHeaders
                .AsNoTracking()
                .Where(x => x.AppId == appId && x.HostName == null)
                .ToListAsync();
            if (appLevelHeaders is { Count: > 0 })
            {
                return appLevelHeaders;
            }
        }

        // Fall back to global
        return await _context.Value.CustomHeaders
            .AsNoTracking()
            .Where(x => x.AppId == null && x.HostName == null)
            .ToListAsync();
    }
}
