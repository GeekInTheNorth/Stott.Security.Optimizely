namespace Stott.Security.Optimizely.Features.Csp.Sandbox.Repository;

using System;
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

    public async Task<SandboxModel> GetAsync(string? appId, string? hostName)
    {
        // Walk inheritance chain: host → app → global
        if (!string.IsNullOrWhiteSpace(appId) && !string.IsNullOrWhiteSpace(hostName))
        {
            var hostSandbox = await _context.Value.CspSandboxes
                .FirstOrDefaultAsync(x => x.AppId == appId && x.HostName == hostName);
            if (hostSandbox != null) return CspSandboxMapper.ToModel(hostSandbox);
        }

        if (!string.IsNullOrWhiteSpace(appId))
        {
            var appSandbox = await _context.Value.CspSandboxes
                .FirstOrDefaultAsync(x => x.AppId == appId && x.HostName == null);
            if (appSandbox != null) return CspSandboxMapper.ToModel(appSandbox);
        }

        var globalSandbox = await _context.Value.CspSandboxes
            .FirstOrDefaultAsync(x => x.AppId == null && x.HostName == null);

        return CspSandboxMapper.ToModel(globalSandbox);
    }

    public async Task<CspSandbox?> GetByContextAsync(string? appId, string? hostName)
    {
        // Returns exact match only (null if using inherited)
        return await _context.Value.CspSandboxes
            .FirstOrDefaultAsync(x => x.AppId == appId && x.HostName == hostName);
    }

    public async Task SaveAsync(SandboxModel model, string modifiedBy, string? appId, string? hostName)
    {
        var recordToSave = await _context.Value.CspSandboxes
            .FirstOrDefaultAsync(x => x.AppId == appId && x.HostName == hostName);

        if (recordToSave == null)
        {
            recordToSave = new CspSandbox
            {
                AppId = appId,
                HostName = hostName
            };
            _context.Value.CspSandboxes.Add(recordToSave);
        }

        CspSandboxMapper.ToEntity(model, recordToSave);

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

        var record = await _context.Value.CspSandboxes
            .FirstOrDefaultAsync(x => x.AppId == appId && x.HostName == hostName);

        if (record != null)
        {
            record.Modified = DateTime.UtcNow;
            record.ModifiedBy = deletedBy;

            _context.Value.CspSandboxes.Remove(record);
            await _context.Value.SaveChangesAsync();
        }
    }
}
