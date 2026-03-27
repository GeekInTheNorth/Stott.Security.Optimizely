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
                await CreateOverrideAsync(appId, hostName, modifiedBy);
            }
        }

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

    public async Task SaveDirectiveAsync(SavePermissionPolicyModel? model, string? modifiedBy, string? appId, string? hostName)
    {
        if (model is null) throw new ArgumentNullException(nameof(model));
        if (string.IsNullOrWhiteSpace(modifiedBy)) throw new ArgumentNullException(nameof(modifiedBy));

        await repository.SaveDirectiveAsync(model, modifiedBy, appId, hostName);

        cache.RemoveAll();
    }

    public async Task<bool> ExistsForContextAsync(string? appId, string? hostName)
    {
        if (string.IsNullOrWhiteSpace(appId) && string.IsNullOrWhiteSpace(hostName))
        {
            return true;
        }

        var actualSettings = await repository.GetSettingsByContextAsync(appId, hostName);
        return actualSettings is not null;
    }

    public async Task CreateOverrideAsync(string? appId, string? hostName, string? modifiedBy)
    {
        if (string.IsNullOrWhiteSpace(modifiedBy)) throw new ArgumentNullException(nameof(modifiedBy));

        // Determine the source context to copy from (the parent in the fallback chain)
        // Creating host-level override: source is app-level (or global) with a null source host.
        // For app-level override: source is global (null, null) which is the default
        var sourceAppId = !string.IsNullOrWhiteSpace(hostName) ? appId : null;

        await repository.CreateOverrideAsync(sourceAppId, null, appId, hostName, modifiedBy);

        cache.RemoveAll();
    }

    public async Task DeleteByContextAsync(string? appId, string? hostName, string? deletedBy)
    {
        if (string.IsNullOrWhiteSpace(appId)) throw new ArgumentNullException(nameof(appId));
        if (string.IsNullOrWhiteSpace(deletedBy)) throw new ArgumentNullException(nameof(deletedBy));

        await repository.DeleteByContextAsync(appId, hostName, deletedBy);

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
