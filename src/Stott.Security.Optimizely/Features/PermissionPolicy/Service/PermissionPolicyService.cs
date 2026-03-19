using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Stott.Security.Optimizely.Features.Caching;
using Stott.Security.Optimizely.Features.Header;
using Stott.Security.Optimizely.Features.PermissionPolicy.Models;
using Stott.Security.Optimizely.Features.PermissionPolicy.Repository;

namespace Stott.Security.Optimizely.Features.PermissionPolicy.Service;

public sealed class PermissionPolicyService(ICacheWrapper cache, IPermissionPolicyRepository repository) : IPermissionPolicyService
{
    private const string SettingsCacheKeyPrefix = "stott.security.permissionpolicy.settings";

    private const string DirectivesCacheKeyPrefix = "stott.security.permissionpolicy.directives";

    private const string CompiledHeaderCacheKeyPrefix = "stott.security.permissionpolicy.compiled";

    public async Task<IPermissionPolicySettings> GetPermissionPolicySettingsAsync(string? appId, string? hostName)
    {
        var cacheKey = GetCacheKey(SettingsCacheKeyPrefix, appId, hostName);
        var settings = cache.Get<PermissionPolicySettingsModel>(cacheKey);
        if (settings is null)
        {
            settings = await repository.GetSettingsAsync(appId, hostName);
            cache.Add(cacheKey, settings);
        }

        return settings;
    }

    public async Task<IPermissionPolicySettings?> GetPermissionPolicySettingsByContextAsync(string? appId, string? hostName)
    {
        return await repository.GetSettingsByContextAsync(appId, hostName);
    }

    public async Task SaveSettingsAsync(IPermissionPolicySettings? settings, string? modifiedBy, string? appId, string? hostName)
    {
        if (settings is null) throw new ArgumentNullException(nameof(settings));
        if (string.IsNullOrWhiteSpace(modifiedBy)) throw new ArgumentNullException(nameof(modifiedBy));

        await repository.SaveSettingsAsync(settings, modifiedBy, appId, hostName);

        // If saving settings at a non-global context, ensure directives are also overridden
        if (!string.IsNullOrWhiteSpace(appId))
        {
            var hasDirectiveOverride = await repository.ListDirectivesByContextAsync(appId, hostName);
            if (hasDirectiveOverride is null)
            {
                await CreateDirectiveOverrideAsync(appId, hostName, modifiedBy);
            }
        }

        cache.RemoveAll();
    }

    public async Task DeleteSettingsByContextAsync(string? appId, string? hostName, string? deletedBy)
    {
        if (string.IsNullOrWhiteSpace(deletedBy)) throw new ArgumentNullException(nameof(deletedBy));

        await repository.DeleteSettingsByContextAsync(appId, hostName, deletedBy);

        cache.RemoveAll();
    }

    public async Task<IList<PermissionPolicyDirectiveModel>> ListDirectivesAsync(string? appId, string? hostName, string? sourceFilter, PermissionPolicyEnabledFilter enabledFilter)
    {
        var cacheKey = GetCacheKey(DirectivesCacheKeyPrefix, appId, hostName);
        var directives = cache.Get<List<PermissionPolicyDirectiveModel>>(cacheKey);
        if (directives is null)
        {
            directives = await repository.ListDirectivesAsync(appId, hostName);
            cache.Add(cacheKey, directives);
        }

        foreach (var directive in PermissionPolicyConstants.AllDirectives)
        {
            if (!directives.Any(x => string.Equals(directive, x.Name)))
            {
                directives.Add(new PermissionPolicyDirectiveModel
                {
                    Name = directive,
                    EnabledState = PermissionPolicyEnabledState.Disabled,
                    Sources = []
                });
            }
        }

        return directives.Where(x => IsMatch(x, sourceFilter, enabledFilter)).ToList();
    }

    public async Task<bool> HasDirectiveOverrideAsync(string? appId, string? hostName)
    {
        var directives = await repository.ListDirectivesByContextAsync(appId, hostName);
        return directives is not null;
    }

    public async Task SaveDirectiveAsync(SavePermissionPolicyModel? model, string? modifiedBy, string? appId, string? hostName)
    {
        if (model is null) throw new ArgumentNullException(nameof(model));
        if (string.IsNullOrWhiteSpace(modifiedBy)) throw new ArgumentNullException(nameof(modifiedBy));

        await repository.SaveDirectiveAsync(model, modifiedBy, appId, hostName);

        cache.RemoveAll();
    }

    public async Task CreateDirectiveOverrideAsync(string? appId, string? hostName, string? modifiedBy)
    {
        if (string.IsNullOrWhiteSpace(modifiedBy)) throw new ArgumentNullException(nameof(modifiedBy));

        // Determine the source context to copy from (the parent in the fallback chain)
        string? sourceAppId = null;
        string? sourceHostName = null;

        if (!string.IsNullOrWhiteSpace(hostName))
        {
            // Creating host-level override: source is app-level (or global)
            sourceAppId = appId;
            sourceHostName = null;
        }

        // For app-level override: source is global (null, null) which is the default

        await repository.CreateDirectiveOverrideAsync(sourceAppId, sourceHostName, appId, hostName, modifiedBy);

        // Also copy inherited settings to the target context
        var inheritedSettings = await repository.GetSettingsAsync(appId, hostName);
        await repository.SaveSettingsAsync(inheritedSettings, modifiedBy, appId, hostName);

        cache.RemoveAll();
    }

    public async Task DeleteDirectivesByContextAsync(string? appId, string? hostName, string? deletedBy)
    {
        if (string.IsNullOrWhiteSpace(deletedBy)) throw new ArgumentNullException(nameof(deletedBy));

        await repository.DeleteDirectivesByContextAsync(appId, hostName, deletedBy);
        await repository.DeleteSettingsByContextAsync(appId, hostName, deletedBy);

        cache.RemoveAll();
    }

    public async Task<IEnumerable<HeaderDto>> GetCompiledHeaders(string? appId, string? hostName)
    {
        var compiledHeaders = new List<HeaderDto>();
        var cacheKey = GetCacheKey(CompiledHeaderCacheKeyPrefix, appId, hostName);
        var cachedData = cache.Get<CompiledPermissionPolicy>(cacheKey);
        if (cachedData is null)
        {
            var settings = await repository.GetSettingsAsync(appId, hostName);
            cachedData = new CompiledPermissionPolicy
            {
                IsEnabled = settings.IsEnabled
            };

            if (cachedData.IsEnabled)
            {
                cachedData.Directives = await repository.ListDirectiveFragments(appId, hostName);
            }

            cache.Add(cacheKey, cachedData);
        }

        if (cachedData is { IsEnabled: true, Directives.Count: > 0 })
        {
            compiledHeaders.Add(new HeaderDto { Key = PermissionPolicyConstants.PermissionPolicyHeader, Value = string.Join(", ", cachedData.Directives) });
        }

        return compiledHeaders;
    }

    private static string GetCacheKey(string prefix, string? appId, string? hostName)
    {
        return $"{prefix}.{appId ?? "global"}.{hostName ?? "all"}";
    }

    private static bool IsMatch(PermissionPolicyDirectiveModel model, string? sourceFilter, PermissionPolicyEnabledFilter enabledFilter)
    {
        if (!string.IsNullOrWhiteSpace(sourceFilter))
        {
            if (model.Sources is null || !model.Sources.Any(x => x.Url?.Contains(sourceFilter) ?? false))
            {
                return false;
            }
        }

        return enabledFilter switch
        {
            PermissionPolicyEnabledFilter.AllEnabled => model.EnabledState != PermissionPolicyEnabledState.Disabled,
            PermissionPolicyEnabledFilter.AllDisabled => model.EnabledState == PermissionPolicyEnabledState.Disabled,
            PermissionPolicyEnabledFilter.None => model.EnabledState == PermissionPolicyEnabledState.None,
            PermissionPolicyEnabledFilter.AllSites => model.EnabledState == PermissionPolicyEnabledState.All,
            PermissionPolicyEnabledFilter.ThisSite => model.EnabledState == PermissionPolicyEnabledState.ThisSite || model.EnabledState == PermissionPolicyEnabledState.ThisAndSpecificSites,
            PermissionPolicyEnabledFilter.SpecificSites => model.EnabledState == PermissionPolicyEnabledState.SpecificSites || model.EnabledState == PermissionPolicyEnabledState.ThisAndSpecificSites,
            _ => true,
        };
    }
}
