namespace Stott.Security.Optimizely.Features.Csp.Sandbox.Repository;

using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.Csp.Sandbox;

/// <inheritdoc cref="ICspSandboxRepository"/>
internal sealed class CspSandboxRepository(Lazy<IStottSecurityDataContext> context) : ICspSandboxRepository
{
    public async Task<SandboxModel> GetAsync(string? appId, string? hostName)
    {
        var hasAppId = !string.IsNullOrWhiteSpace(appId);
        var hasHostName = !string.IsNullOrWhiteSpace(hostName);

        var candidates = await context.Value.CspSandboxes
            .Where(x => (x.AppId == null || x.AppId == appId) && (x.HostName == null || x.HostName == hostName))
            .AsNoTracking()
            .ToListAsync();

        var bestMatch = candidates
            .OrderByDescending(x => hasAppId && string.Equals(x.AppId, appId, StringComparison.OrdinalIgnoreCase) && hasHostName && string.Equals(x.HostName, hostName, StringComparison.OrdinalIgnoreCase))
            .ThenByDescending(x => hasAppId && string.Equals(x.AppId, appId, StringComparison.OrdinalIgnoreCase) && string.IsNullOrWhiteSpace(x.HostName))
            .ThenByDescending(x => string.IsNullOrWhiteSpace(x.AppId) && string.IsNullOrWhiteSpace(x.HostName))
            .FirstOrDefault();

        return CspSandboxMapper.ToModelWithDefault(bestMatch);
    }

    public async Task<SandboxModel?> GetByContextAsync(string? appId, string? hostName)
    {
        // Returns exact match only (null if using inherited)
        var data = await context.Value.CspSandboxes.AsNoTracking().FirstOrDefaultAsync(x => x.AppId == appId && x.HostName == hostName);

        return CspSandboxMapper.ToModel(data);
    }

    public async Task SaveAsync(SandboxModel model, string modifiedBy, string? appId, string? hostName)
    {
        var recordToSave = await context.Value.CspSandboxes
            .FirstOrDefaultAsync(x => x.AppId == appId && x.HostName == hostName);

        if (recordToSave == null)
        {
            recordToSave = new CspSandbox
            {
                AppId = appId,
                HostName = hostName
            };
            context.Value.CspSandboxes.Add(recordToSave);
        }

        CspSandboxMapper.ToEntity(model, recordToSave);

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

        var record = await context.Value.CspSandboxes
            .FirstOrDefaultAsync(x => x.AppId == appId && x.HostName == hostName);

        if (record != null)
        {
            record.Modified = DateTime.UtcNow;
            record.ModifiedBy = deletedBy;

            context.Value.CspSandboxes.Remove(record);
            await context.Value.SaveChangesAsync();
        }
    }
}
