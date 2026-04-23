namespace Stott.Security.Optimizely.Features.Csp.Settings.Repository;

using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Extensions;
using Stott.Security.Optimizely.Features.Csp.Settings;

internal sealed class CspSettingsRepository : ICspSettingsRepository
{
    private readonly Lazy<ICspDataContext> _context;

    public CspSettingsRepository(Lazy<ICspDataContext> context)
    {
        _context = context;
    }

    public async Task<CspSettings> GetAsync(Guid? siteId, string? hostName)
    {
        var normalisedSiteId = siteId.GetSanitizedSiteId();
        var normalisedHost = hostName.GetSanitizedHostDomain();
        var hasSiteId = normalisedSiteId.HasValue;
        var hasHostName = normalisedHost != null;

        var candidates = await _context.Value.CspSettings
            .AsNoTracking()
            .Where(x => (x.SiteId == null || x.SiteId == normalisedSiteId) && (x.HostName == null || x.HostName == normalisedHost))
            .ToListAsync();

        var bestMatch = candidates
            .OrderByDescending(x => hasSiteId && x.SiteId == normalisedSiteId && hasHostName && string.Equals(x.HostName, normalisedHost, StringComparison.OrdinalIgnoreCase))
            .ThenByDescending(x => hasSiteId && x.SiteId == normalisedSiteId && string.IsNullOrWhiteSpace(x.HostName))
            .ThenByDescending(x => x.SiteId == null && string.IsNullOrWhiteSpace(x.HostName))
            .FirstOrDefault();

        return bestMatch ?? new CspSettings();
    }

    public async Task<CspSettings?> GetByContextAsync(Guid? siteId, string? hostName)
    {
        var normalisedSiteId = siteId.GetSanitizedSiteId();
        var normalisedHost = hostName.GetSanitizedHostDomain();

        return await _context.Value.CspSettings
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.SiteId == normalisedSiteId && x.HostName == normalisedHost);
    }

    public async Task SaveAsync(ICspSettings settings, Guid? siteId, string? hostName, string modifiedBy)
    {
        var normalisedSiteId = siteId.GetSanitizedSiteId();
        var normalisedHost = hostName.GetSanitizedHostDomain();

        var recordToSave = await _context.Value.CspSettings.FirstOrDefaultAsync(x => x.SiteId == normalisedSiteId && x.HostName == normalisedHost);

        if (recordToSave == null)
        {
            recordToSave = new CspSettings
            {
                SiteId = normalisedSiteId,
                HostName = normalisedHost
            };
            _context.Value.CspSettings.Add(recordToSave);
        }

        CspSettingsMapper.ToEntity(settings, recordToSave);
        recordToSave.SiteId = normalisedSiteId;
        recordToSave.HostName = normalisedHost;
        recordToSave.Modified = DateTime.UtcNow;
        recordToSave.ModifiedBy = modifiedBy;

        await _context.Value.SaveChangesAsync();
    }

    public async Task DeleteByContextAsync(Guid? siteId, string? hostName, string deletedBy)
    {
        var normalisedSiteId = siteId.GetSanitizedSiteId();
        var normalisedHost = hostName.GetSanitizedHostDomain();

        // Refuse to delete Global scope; Global is the root of the inheritance chain.
        if (normalisedSiteId == null)
        {
            return;
        }

        var record = await _context.Value.CspSettings.FirstOrDefaultAsync(x => x.SiteId == normalisedSiteId && x.HostName == normalisedHost);

        if (record != null)
        {
            record.Modified = DateTime.UtcNow;
            record.ModifiedBy = deletedBy;

            _context.Value.CspSettings.Remove(record);
            await _context.Value.SaveChangesAsync();
        }
    }
}
