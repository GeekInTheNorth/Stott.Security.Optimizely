namespace Stott.Security.Optimizely.Features.Csp.Sandbox.Repository;

using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.Csp.Sandbox;

internal sealed class CspSandboxRepository : ICspSandboxRepository
{
    private readonly Lazy<ICspDataContext> _context;

    public CspSandboxRepository(Lazy<ICspDataContext> context)
    {
        _context = context;
    }

    public async Task<SandboxModel> GetAsync(Guid? siteId, string? hostName)
    {
        var normalisedHost = string.IsNullOrWhiteSpace(hostName) ? null : hostName;
        var hasSiteId = siteId.HasValue && siteId.Value != Guid.Empty;
        var hasHostName = normalisedHost != null;

        var candidates = await _context.Value.CspSandboxes
            .AsNoTracking()
            .Where(x => (x.SiteId == null || x.SiteId == siteId) && (x.HostName == null || x.HostName == normalisedHost))
            .ToListAsync();

        var bestMatch = candidates
            .OrderByDescending(x => hasSiteId && x.SiteId == siteId && hasHostName && string.Equals(x.HostName, normalisedHost, StringComparison.OrdinalIgnoreCase))
            .ThenByDescending(x => hasSiteId && x.SiteId == siteId && string.IsNullOrWhiteSpace(x.HostName))
            .ThenByDescending(x => x.SiteId == null && string.IsNullOrWhiteSpace(x.HostName))
            .FirstOrDefault();

        return CspSandboxMapper.ToModelWithDefault(bestMatch);
    }

    public async Task<SandboxModel?> GetByContextAsync(Guid? siteId, string? hostName)
    {
        var normalisedSite = siteId == Guid.Empty ? null : siteId;
        var normalisedHost = string.IsNullOrWhiteSpace(hostName) ? null : hostName;

        var data = await _context.Value.CspSandboxes
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.SiteId == normalisedSite && x.HostName == normalisedHost);

        return CspSandboxMapper.ToModel(data);
    }

    public async Task SaveAsync(SandboxModel model, string modifiedBy, Guid? siteId, string? hostName)
    {
        var normalisedSite = siteId == Guid.Empty ? null : siteId;
        var normalisedHost = string.IsNullOrWhiteSpace(hostName) ? null : hostName;

        var recordToSave = await _context.Value.CspSandboxes
            .FirstOrDefaultAsync(x => x.SiteId == normalisedSite && x.HostName == normalisedHost);

        if (recordToSave == null)
        {
            recordToSave = new CspSandbox
            {
                SiteId = normalisedSite,
                HostName = normalisedHost
            };
            _context.Value.CspSandboxes.Add(recordToSave);
        }

        CspSandboxMapper.ToEntity(model, recordToSave);
        recordToSave.SiteId = normalisedSite;
        recordToSave.HostName = normalisedHost;
        recordToSave.Modified = DateTime.UtcNow;
        recordToSave.ModifiedBy = modifiedBy;

        await _context.Value.SaveChangesAsync();
    }

    public async Task DeleteByContextAsync(Guid? siteId, string? hostName, string deletedBy)
    {
        var normalisedSite = siteId == Guid.Empty ? null : siteId;
        var normalisedHost = string.IsNullOrWhiteSpace(hostName) ? null : hostName;

        // Refuse to delete Global scope; Global is the root of the inheritance chain.
        if (normalisedSite == null)
        {
            return;
        }

        var record = await _context.Value.CspSandboxes
            .FirstOrDefaultAsync(x => x.SiteId == normalisedSite && x.HostName == normalisedHost);

        if (record != null)
        {
            record.Modified = DateTime.UtcNow;
            record.ModifiedBy = deletedBy;

            _context.Value.CspSandboxes.Remove(record);
            await _context.Value.SaveChangesAsync();
        }
    }
}
