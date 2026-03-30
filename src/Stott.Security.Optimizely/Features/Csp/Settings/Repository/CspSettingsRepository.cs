namespace Stott.Security.Optimizely.Features.Csp.Settings.Repository;

using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.Csp.Settings;

/// <inheritdoc cref="ICspSettingsRepository"/>
internal sealed class CspSettingsRepository(Lazy<IStottSecurityDataContext> context) : ICspSettingsRepository
{
    public async Task<CspSettings> GetAsync(string? appId, string? hostName)
    {
        var hasAppId = !string.IsNullOrWhiteSpace(appId);
        var hasHostName = !string.IsNullOrWhiteSpace(hostName);

        var candidates = await context.Value.CspSettings
            .Where(x => (x.AppId == null || x.AppId == appId) && (x.HostName == null || x.HostName == hostName))
            .AsNoTracking()
            .ToListAsync();

        var bestMatch = candidates
            .OrderByDescending(x => hasAppId && string.Equals(x.AppId, appId, StringComparison.OrdinalIgnoreCase) && hasHostName && string.Equals(x.HostName, hostName, StringComparison.OrdinalIgnoreCase))
            .ThenByDescending(x => hasAppId && string.Equals(x.AppId, appId, StringComparison.OrdinalIgnoreCase) && string.IsNullOrWhiteSpace(x.HostName))
            .ThenByDescending(x => string.IsNullOrWhiteSpace(x.AppId) && string.IsNullOrWhiteSpace(x.HostName))
            .FirstOrDefault();

        return bestMatch ?? new CspSettings();
    }

    public async Task<CspSettings?> GetByContextAsync(string? appId, string? hostName)
    {
        // Returns exact match only (null if using inherited)
        return await context.Value.CspSettings
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.AppId == appId && x.HostName == hostName);
    }

    public async Task SaveAsync(ICspSettings settings, string modifiedBy, string? appId, string? hostName)
    {
        var recordToSave = await context.Value.CspSettings
            .FirstOrDefaultAsync(x => x.AppId == appId && x.HostName == hostName);

        if (recordToSave == null)
        {
            recordToSave = new CspSettings
            {
                AppId = appId,
                HostName = hostName
            };
            context.Value.CspSettings.Add(recordToSave);
        }

        CspSettingsMapper.ToEntity(settings, recordToSave);
        recordToSave.Modified = DateTime.UtcNow;
        recordToSave.ModifiedBy = modifiedBy;

        await context.Value.SaveChangesAsync();
    }

    public async Task DeleteByContextAsync(string? appId, string? hostName, string deletedBy)
    {
        // Only allow deleting non-global settings (revert to inherited)
        if (string.IsNullOrWhiteSpace(appId))
        {
            return;
        }

        var record = await context.Value.CspSettings
            .FirstOrDefaultAsync(x => x.AppId == appId && x.HostName == hostName);

        if (record != null)
        {
            record.Modified = DateTime.UtcNow;
            record.ModifiedBy = deletedBy;

            context.Value.CspSettings.Remove(record);
            await context.Value.SaveChangesAsync();
        }
    }
}
