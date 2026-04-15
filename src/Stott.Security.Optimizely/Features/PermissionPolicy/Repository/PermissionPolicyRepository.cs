using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.PermissionPolicy.Models;

namespace Stott.Security.Optimizely.Features.PermissionPolicy.Repository;

internal sealed class PermissionPolicyRepository : IPermissionPolicyRepository
{
    private readonly Lazy<ICspDataContext> _context;

    public PermissionPolicyRepository(Lazy<ICspDataContext> context)
    {
        _context = context;
    }

    public async Task<PermissionPolicySettingsModel> GetSettingsAsync(Guid? siteId, string? hostName)
    {
        var normalisedSite = siteId == Guid.Empty ? null : siteId;
        var normalisedHost = string.IsNullOrWhiteSpace(hostName) ? null : hostName;
        var hasSiteId = normalisedSite.HasValue;
        var hasHostName = normalisedHost != null;

        var candidates = await _context.Value.PermissionPolicySettings
            .Where(x => (x.SiteId == null || x.SiteId == normalisedSite) && (x.HostName == null || x.HostName == normalisedHost))
            .AsNoTracking()
            .ToListAsync();

        var bestMatch = candidates
            .OrderByDescending(x => hasSiteId && x.SiteId == normalisedSite && hasHostName && string.Equals(x.HostName, normalisedHost, StringComparison.OrdinalIgnoreCase))
            .ThenByDescending(x => hasSiteId && x.SiteId == normalisedSite && string.IsNullOrWhiteSpace(x.HostName))
            .ThenByDescending(x => x.SiteId == null && string.IsNullOrWhiteSpace(x.HostName))
            .FirstOrDefault();

        return PermissionPolicyMapper.ToSettingsModel(bestMatch);
    }

    public async Task<PermissionPolicySettingsModel?> GetSettingsByContextAsync(Guid? siteId, string? hostName)
    {
        var normalisedSite = siteId == Guid.Empty ? null : siteId;
        var normalisedHost = string.IsNullOrWhiteSpace(hostName) ? null : hostName;

        var data = await _context.Value.PermissionPolicySettings
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.SiteId == normalisedSite && x.HostName == normalisedHost);

        return data is not null ? PermissionPolicyMapper.ToSettingsModel(data) : null;
    }

    public async Task SaveSettingsAsync(IPermissionPolicySettings settings, string modifiedBy, Guid? siteId, string? hostName)
    {
        if (settings is null) throw new ArgumentNullException(nameof(settings));
        if (string.IsNullOrWhiteSpace(modifiedBy)) throw new ArgumentNullException(nameof(modifiedBy));

        var normalisedSite = siteId == Guid.Empty ? null : siteId;
        var normalisedHost = string.IsNullOrWhiteSpace(hostName) ? null : hostName;

        var data = await _context.Value.PermissionPolicySettings
            .FirstOrDefaultAsync(x => x.SiteId == normalisedSite && x.HostName == normalisedHost);

        if (data is null)
        {
            data = new PermissionPolicySettings
            {
                SiteId = normalisedSite,
                HostName = normalisedHost
            };

            _context.Value.PermissionPolicySettings.Add(data);
        }

        data.IsEnabled = settings.IsEnabled;
        data.Modified = DateTime.UtcNow;
        data.ModifiedBy = modifiedBy;

        await _context.Value.SaveChangesAsync();
    }

    public async Task<List<PermissionPolicyDirectiveModel>> ListDirectivesAsync(Guid? siteId, string? hostName)
    {
        var data = await GetDirectivesInFallBackChainAsync(siteId, hostName);

        return data.Select(PermissionPolicyMapper.ToModel).ToList();
    }

    public async Task<List<PermissionPolicyDirectiveModel>?> ListDirectivesByContextAsync(Guid? siteId, string? hostName)
    {
        var normalisedSite = siteId == Guid.Empty ? null : siteId;
        var normalisedHost = string.IsNullOrWhiteSpace(hostName) ? null : hostName;

        var data = await _context.Value.PermissionPolicies
            .Where(x => x.SiteId == normalisedSite && x.HostName == normalisedHost)
            .AsNoTracking()
            .ToListAsync();

        return data.Count > 0 ? data.Select(PermissionPolicyMapper.ToModel).ToList() : null;
    }

    public async Task<List<string>> ListDirectiveFragments(Guid? siteId, string? hostName)
    {
        var data = await GetDirectivesInFallBackChainAsync(siteId, hostName);

        return data.Select(PermissionPolicyMapper.ToPolicyFragment).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
    }

    public async Task SaveDirectiveAsync(SavePermissionPolicyModel model, string modifiedBy, Guid? siteId, string? hostName)
    {
        if (model is null) throw new ArgumentNullException(nameof(model));
        if (string.IsNullOrWhiteSpace(modifiedBy)) throw new ArgumentNullException(nameof(modifiedBy));

        var normalisedSite = siteId == Guid.Empty ? null : siteId;
        var normalisedHost = string.IsNullOrWhiteSpace(hostName) ? null : hostName;

        var recordToSave = await _context.Value.PermissionPolicies
            .FirstOrDefaultAsync(x => x.Directive == model.Name && x.SiteId == normalisedSite && x.HostName == normalisedHost);

        if (recordToSave is null)
        {
            recordToSave = new Entities.PermissionPolicy
            {
                SiteId = normalisedSite,
                HostName = normalisedHost
            };

            _context.Value.PermissionPolicies.Add(recordToSave);
        }

        PermissionPolicyMapper.ToEntity(model, recordToSave, modifiedBy);

        await _context.Value.SaveChangesAsync();
    }

    public async Task CreateOverrideAsync(Guid? sourceSiteId, string? sourceHostName, Guid? targetSiteId, string? targetHostName, string modifiedBy)
    {
        if (string.IsNullOrWhiteSpace(modifiedBy)) throw new ArgumentNullException(nameof(modifiedBy));

        var normalisedTargetSite = targetSiteId == Guid.Empty ? null : targetSiteId;
        var normalisedTargetHost = string.IsNullOrWhiteSpace(targetHostName) ? null : targetHostName;

        // Check if target already has a configuration, if yes, do not override to prevent unintentional data loss
        var settingsExist = await _context.Value.PermissionPolicySettings
            .AnyAsync(x => x.SiteId == normalisedTargetSite && x.HostName == normalisedTargetHost);
        var directivesExist = await _context.Value.PermissionPolicies
            .AnyAsync(x => x.SiteId == normalisedTargetSite && x.HostName == normalisedTargetHost);

        if (directivesExist || settingsExist)
        {
            return;
        }

        // Duplicate Settings
        var sourceSettings = await GetSettingsAsync(sourceSiteId, sourceHostName);
        _context.Value.PermissionPolicySettings.Add(new PermissionPolicySettings
        {
            SiteId = normalisedTargetSite,
            HostName = normalisedTargetHost,
            IsEnabled = sourceSettings.IsEnabled,
            Modified = DateTime.UtcNow,
            ModifiedBy = modifiedBy
        });

        // Load directives from source context using fallback
        var sourceDirectives = await GetDirectivesInFallBackChainAsync(sourceSiteId, sourceHostName);
        var now = DateTime.UtcNow;

        foreach (var source in sourceDirectives)
        {
            _context.Value.PermissionPolicies.Add(new Entities.PermissionPolicy
            {
                Directive = source.Directive,
                EnabledState = source.EnabledState,
                Origins = source.Origins,
                SiteId = normalisedTargetSite,
                HostName = normalisedTargetHost,
                Modified = now,
                ModifiedBy = modifiedBy
            });
        }

        await _context.Value.SaveChangesAsync();
    }

    public async Task DeleteByContextAsync(Guid? siteId, string? hostName, string deletedBy)
    {
        var normalisedSite = siteId == Guid.Empty ? null : siteId;
        var normalisedHost = string.IsNullOrWhiteSpace(hostName) ? null : hostName;

        // Refuse to delete Global scope
        if (normalisedSite == null)
        {
            return;
        }

        var records = await _context.Value.PermissionPolicies
            .Where(x => x.SiteId == normalisedSite && x.HostName == normalisedHost)
            .ToListAsync();

        if (records is { Count: > 0 })
        {
            var now = DateTime.UtcNow;
            foreach (var record in records)
            {
                record.Modified = now;
                record.ModifiedBy = deletedBy;
            }

            _context.Value.PermissionPolicies.RemoveRange(records);
        }

        var settings = await _context.Value.PermissionPolicySettings
            .FirstOrDefaultAsync(x => x.SiteId == normalisedSite && x.HostName == normalisedHost);

        if (settings != null)
        {
            settings.Modified = DateTime.UtcNow;
            settings.ModifiedBy = deletedBy;

            _context.Value.PermissionPolicySettings.Remove(settings);
        }

        await _context.Value.SaveChangesAsync();
    }

    private async Task<List<Entities.PermissionPolicy>> GetDirectivesInFallBackChainAsync(Guid? siteId, string? hostName)
    {
        var normalisedSite = siteId == Guid.Empty ? null : siteId;
        var normalisedHost = string.IsNullOrWhiteSpace(hostName) ? null : hostName;

        // Check exact match (host level)
        if (normalisedSite.HasValue && normalisedHost != null)
        {
            var hostLevelPermissions = await _context.Value.PermissionPolicies
                .Where(x => x.SiteId == normalisedSite && x.HostName == normalisedHost)
                .AsNoTracking()
                .ToListAsync();

            if (hostLevelPermissions is { Count: > 0 })
            {
                return hostLevelPermissions;
            }
        }

        // Check site level
        if (normalisedSite.HasValue)
        {
            var siteLevelPermissions = await _context.Value.PermissionPolicies
                .Where(x => x.SiteId == normalisedSite && x.HostName == null)
                .AsNoTracking()
                .ToListAsync();

            if (siteLevelPermissions is { Count: > 0 })
            {
                return siteLevelPermissions;
            }
        }

        // Fall back to global
        return await _context.Value.PermissionPolicies
            .Where(x => x.SiteId == null && x.HostName == null)
            .AsNoTracking()
            .ToListAsync();
    }
}
