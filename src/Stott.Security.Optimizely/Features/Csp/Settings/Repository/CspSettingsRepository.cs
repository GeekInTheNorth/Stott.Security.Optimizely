namespace Stott.Security.Optimizely.Features.Csp.Settings.Repository;

using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.Csp.Settings;

internal sealed class CspSettingsRepository : ICspSettingsRepository
{
    private readonly Lazy<ICspDataContext> _context;

    public CspSettingsRepository(Lazy<ICspDataContext> context)
    {
        _context = context;
    }

    public async Task<CspSettings> GetAsync(string? appId, string? hostName)
    {
        // Walk inheritance chain: host → app → global
        if (!string.IsNullOrWhiteSpace(appId) && !string.IsNullOrWhiteSpace(hostName))
        {
            var hostSettings = await _context.Value.CspSettings
                .FirstOrDefaultAsync(x => x.AppId == appId && x.HostName == hostName);
            if (hostSettings != null) return hostSettings;
        }

        if (!string.IsNullOrWhiteSpace(appId))
        {
            var appSettings = await _context.Value.CspSettings
                .FirstOrDefaultAsync(x => x.AppId == appId && x.HostName == null);
            if (appSettings != null) return appSettings;
        }

        var globalSettings = await _context.Value.CspSettings
            .Where(x => x.AppId == null && x.HostName == null)
            .OrderByDescending(x => x.Modified)
            .FirstOrDefaultAsync();

        return globalSettings ?? new CspSettings();
    }

    public async Task<CspSettings?> GetByContextAsync(string? appId, string? hostName)
    {
        // Returns exact match only (null if using inherited)
        return await _context.Value.CspSettings
            .FirstOrDefaultAsync(x => x.AppId == appId && x.HostName == hostName);
    }

    public async Task SaveAsync(ICspSettings settings, string modifiedBy, string? appId, string? hostName)
    {
        var recordToSave = await _context.Value.CspSettings
            .FirstOrDefaultAsync(x => x.AppId == appId && x.HostName == hostName);

        if (recordToSave == null)
        {
            recordToSave = new CspSettings
            {
                AppId = appId,
                HostName = hostName
            };
            _context.Value.CspSettings.Add(recordToSave);
        }

        CspSettingsMapper.ToEntity(settings, recordToSave);
        recordToSave.Modified = DateTime.UtcNow;
        recordToSave.ModifiedBy = modifiedBy;

        await _context.Value.SaveChangesAsync();
    }

    public async Task DeleteByContextAsync(string? appId, string? hostName, string deletedBy)
    {
        // Only allow deleting non-global settings (revert to inherited)
        if (string.IsNullOrWhiteSpace(appId))
        {
            return;
        }

        var record = await _context.Value.CspSettings
            .FirstOrDefaultAsync(x => x.AppId == appId && x.HostName == hostName);

        if (record != null)
        {
            record.Modified = DateTime.UtcNow;
            record.ModifiedBy = deletedBy;

            _context.Value.CspSettings.Remove(record);
            await _context.Value.SaveChangesAsync();
        }
    }
}
