using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.PermissionPolicy.Models;

namespace Stott.Security.Optimizely.Features.PermissionPolicy.Repository;

internal sealed class PermissionPolicyRepository(Lazy<ICspDataContext> context) : IPermissionPolicyRepository
{
    public async Task<PermissionPolicySettingsModel> GetSettingsAsync(string? appId, string? hostName)
    {
        var hasAppId = !string.IsNullOrWhiteSpace(appId);
        var hasHostName = !string.IsNullOrWhiteSpace(hostName);

        var candidates = await context.Value.PermissionPolicySettings
            .Where(x => (x.AppId == null || x.AppId == appId) && (x.HostName == null || x.HostName == hostName))
            .ToListAsync();

        var bestMatch = candidates
            .OrderByDescending(x => hasAppId && string.Equals(x.AppId, appId, StringComparison.OrdinalIgnoreCase) && hasHostName && string.Equals(x.HostName, hostName, StringComparison.OrdinalIgnoreCase))
            .ThenByDescending(x => hasAppId && string.Equals(x.AppId, appId, StringComparison.OrdinalIgnoreCase) && string.IsNullOrWhiteSpace(x.HostName))
            .ThenByDescending(x => string.IsNullOrWhiteSpace(x.AppId) && string.IsNullOrWhiteSpace(x.HostName))
            .FirstOrDefault();

        return PermissionPolicyMapper.ToModel(bestMatch);
    }

    public async Task<PermissionPolicySettingsModel?> GetSettingsByContextAsync(string? appId, string? hostName)
    {
        var data = await context.Value.PermissionPolicySettings.FirstOrDefaultAsync(x => x.AppId == appId && x.HostName == hostName);

        return data is not null ? PermissionPolicyMapper.ToModel(data) : null;
    }

    public async Task SaveSettingsAsync(IPermissionPolicySettings settings, string modifiedBy, string? appId, string? hostName)
    {
        if (settings is null) throw new ArgumentNullException(nameof(settings));
        if (string.IsNullOrWhiteSpace(modifiedBy)) throw new ArgumentNullException(nameof(modifiedBy));

        var data = await context.Value.PermissionPolicySettings.FirstOrDefaultAsync(x => x.AppId == appId && x.HostName == hostName);

        if (data is null)
        {
            data = new PermissionPolicySettings
            {
                AppId = appId,
                HostName = hostName
            };

            context.Value.PermissionPolicySettings.Add(data);
        }

        data.IsEnabled = settings.IsEnabled;
        data.Modified = DateTime.UtcNow;
        data.ModifiedBy = modifiedBy;

        await context.Value.SaveChangesAsync();
    }

    public async Task DeleteSettingsByContextAsync(string? appId, string? hostName, string deletedBy)
    {
        // Delete by context only supports app level and host level records, global records should not be deleted as they serve as fallback
        if (string.IsNullOrWhiteSpace(appId))
        {
            return;
        }

        var record = await context.Value.PermissionPolicySettings.FirstOrDefaultAsync(x => x.AppId == appId && x.HostName == hostName);
        if (record != null)
        {
            record.Modified = DateTime.UtcNow;
            record.ModifiedBy = deletedBy;

            context.Value.PermissionPolicySettings.Remove(record);
            await context.Value.SaveChangesAsync();
        }
    }

    public async Task<List<PermissionPolicyDirectiveModel>> ListDirectivesAsync(string? appId, string? hostName)
    {
        var data = await GetDirectivesInFallBackChainAsync(appId, hostName);

        return data.Select(PermissionPolicyMapper.ToModel).ToList();
    }

    public async Task<List<PermissionPolicyDirectiveModel>?> ListDirectivesByContextAsync(string? appId, string? hostName)
    {
        var data = await context.Value.PermissionPolicies.Where(x => x.AppId == appId && x.HostName == hostName).ToListAsync();

        return data.Count > 0 ? data.Select(PermissionPolicyMapper.ToModel).ToList() : null;
    }

    public async Task<List<string>> ListDirectiveFragments(string? appId, string? hostName)
    {
        var data = await GetDirectivesInFallBackChainAsync(appId, hostName);

        return data.Select(PermissionPolicyMapper.ToPolicyFragment).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
    }

    public async Task SaveDirectiveAsync(SavePermissionPolicyModel model, string modifiedBy, string? appId, string? hostName)
    {
        if (model is null) throw new ArgumentNullException(nameof(model));
        if (string.IsNullOrWhiteSpace(modifiedBy)) throw new ArgumentNullException(nameof(modifiedBy));

        var recordToSave = await context.Value.PermissionPolicies.FirstOrDefaultAsync(x => x.Directive == model.Name && x.AppId == appId && x.HostName == hostName);

        if (recordToSave is null)
        {
            recordToSave = new Entities.PermissionPolicy
            {
                AppId = appId,
                HostName = hostName
            };

            context.Value.PermissionPolicies.Add(recordToSave);
        }

        PermissionPolicyMapper.ToEntity(model, recordToSave, modifiedBy);

        await context.Value.SaveChangesAsync();
    }

    public async Task CreateDirectiveOverrideAsync(string? sourceAppId, string? sourceHostName, string? targetAppId, string? targetHostName, string modifiedBy)
    {
        if (string.IsNullOrWhiteSpace(modifiedBy)) throw new ArgumentNullException(nameof(modifiedBy));

        // Check if target already has directives
        var existingTarget = await context.Value.PermissionPolicies.AnyAsync(x => x.AppId == targetAppId && x.HostName == targetHostName);
        if (existingTarget)
        {
            return;
        }

        // Load directives from source context using fallback
        var sourceDirectives = await GetDirectivesInFallBackChainAsync(sourceAppId, sourceHostName);
        var now = DateTime.UtcNow;

        foreach (var source in sourceDirectives)
        {
            var copy = new Entities.PermissionPolicy
            {
                Directive = source.Directive,
                EnabledState = source.EnabledState,
                Origins = source.Origins,
                AppId = targetAppId,
                HostName = targetHostName,
                Modified = now,
                ModifiedBy = modifiedBy
            };

            context.Value.PermissionPolicies.Add(copy);
        }

        if (sourceDirectives.Count > 0)
        {
            await context.Value.SaveChangesAsync();
        }
    }

    public async Task DeleteDirectivesByContextAsync(string? appId, string? hostName, string deletedBy)
    {
        if (string.IsNullOrWhiteSpace(appId))
        {
            return;
        }

        var records = await context.Value.PermissionPolicies.Where(x => x.AppId == appId && x.HostName == hostName).ToListAsync();
        if (records.Count > 0)
        {
            var now = DateTime.UtcNow;
            foreach (var record in records)
            {
                record.Modified = now;
                record.ModifiedBy = deletedBy;
            }

            context.Value.PermissionPolicies.RemoveRange(records);
            await context.Value.SaveChangesAsync();
        }
    }

    private async Task<List<Entities.PermissionPolicy>> GetDirectivesInFallBackChainAsync(string? appId, string? hostName)
    {
        // Check exact match (host level)
        if (!string.IsNullOrWhiteSpace(appId) && !string.IsNullOrWhiteSpace(hostName))
        {
            var hostLevelPermissions = await context.Value.PermissionPolicies.Where(x => x.AppId == appId && x.HostName == hostName).ToListAsync() ;
            if (hostLevelPermissions is { Count: >0 })
            {
                return hostLevelPermissions;
            }
        }

        // Check app level
        if (!string.IsNullOrWhiteSpace(appId))
        {
            var appLevelPermissions = await context.Value.PermissionPolicies.Where(x => x.AppId == appId && x.HostName == null).ToListAsync();
            if (appLevelPermissions is { Count: >0 })
            {
                return appLevelPermissions;
            }
        }

        // Fall back to global
        return await context.Value.PermissionPolicies.Where(x => x.AppId == null && x.HostName == null).ToListAsync();
    }
}
