using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Features.Caching;
using Stott.Security.Optimizely.Features.Header;
using Stott.Security.Optimizely.Features.PermissionPolicy.Models;
using Stott.Security.Optimizely.Features.PermissionPolicy.Repository;

namespace Stott.Security.Optimizely.Features.PermissionPolicy.Service;

public sealed class PermissionPolicyService : IPermissionPolicyService
{
    private readonly ICacheWrapper _cache;

    private readonly IPermissionPolicyRepository _repository;

    private const string CompiledHeaderCacheKey = "stott.sec.pp";

    public PermissionPolicyService(ICacheWrapper cache, IPermissionPolicyRepository repository)
    {
        _cache = cache;
        _repository = repository;
    }

    public async Task<IPermissionPolicySettings> GetPermissionPolicySettingsAsync(Guid? siteId, string? hostName)
    {
        var cacheKey = GetCacheKey(CspConstants.CacheKeys.PermissionsPolicySettings, siteId, hostName);
        var settings = _cache.Get<PermissionPolicySettingsModel>(cacheKey);
        if (settings is null)
        {
            settings = await _repository.GetSettingsAsync(siteId, hostName);
            _cache.Add(cacheKey, settings);
        }

        return settings;
    }

    public async Task<IPermissionPolicySettings?> GetPermissionPolicySettingsByContextAsync(Guid? siteId, string? hostName)
    {
        return await _repository.GetSettingsByContextAsync(siteId, hostName);
    }

    public async Task SaveSettingsAsync(IPermissionPolicySettings? settings, string? modifiedBy, Guid? siteId, string? hostName)
    {
        if (settings is null) throw new ArgumentNullException(nameof(settings));
        if (string.IsNullOrWhiteSpace(modifiedBy)) throw new ArgumentNullException(nameof(modifiedBy));

        await _repository.SaveSettingsAsync(settings, modifiedBy, siteId, hostName);

        // If saving settings at a non-global context, ensure directives are also overridden
        var hasSiteId = siteId.HasValue && siteId.Value != Guid.Empty;
        if (hasSiteId)
        {
            var hasDirectiveOverride = await _repository.ListDirectivesByContextAsync(siteId, hostName);
            if (hasDirectiveOverride is null)
            {
                await CreateOverrideAsync(siteId, hostName, modifiedBy);
            }
        }

        _cache.RemoveAll();
    }

    public async Task<IList<PermissionPolicyDirectiveModel>> ListDirectivesAsync(Guid? siteId, string? hostName, string? sourceFilter, PermissionPolicyEnabledFilter enabledFilter)
    {
        var cacheKey = GetCacheKey(CspConstants.CacheKeys.PermissionsPolicyDirectives, siteId, hostName);
        var directives = _cache.Get<List<PermissionPolicyDirectiveModel>>(cacheKey);
        if (directives is null)
        {
            directives = await _repository.ListDirectivesAsync(siteId, hostName);
            _cache.Add(cacheKey, directives);
        }

        foreach (var directive in PermissionPolicyConstants.AllDirectives)
        {
            if (!directives.Any(x => string.Equals(directive, x.Name)))
            {
                directives.Add(new PermissionPolicyDirectiveModel
                {
                    Name = directive,
                    EnabledState = PermissionPolicyEnabledState.Disabled,
                    Sources = new List<PermissionPolicyUrl>(0)
                });
            }
        }

        return directives.Where(x => IsMatch(x, sourceFilter, enabledFilter)).ToList();
    }

    public async Task SaveDirectiveAsync(SavePermissionPolicyModel? model, string? modifiedBy, Guid? siteId, string? hostName)
    {
        if (model is null) throw new ArgumentNullException(nameof(model));
        if (string.IsNullOrWhiteSpace(modifiedBy)) throw new ArgumentNullException(nameof(modifiedBy));

        await _repository.SaveDirectiveAsync(model, modifiedBy, siteId, hostName);

        _cache.RemoveAll();
    }

    public async Task<bool> ExistsForContextAsync(Guid? siteId, string? hostName)
    {
        var hasSiteId = siteId.HasValue && siteId.Value != Guid.Empty;
        var hasHostName = !string.IsNullOrWhiteSpace(hostName);
        if (!hasSiteId && !hasHostName)
        {
            // The Global scope always exists.
            return true;
        }

        var cacheKey = GetCacheKey(CspConstants.CacheKeys.PermissionsPolicyInheritedSettings, siteId, hostName);
        var ctxState = _cache.Get<ContextStateModel>(cacheKey);
        if (ctxState is null)
        {
            var actualSettings = await _repository.GetSettingsByContextAsync(siteId, hostName);
            ctxState = new ContextStateModel
            {
                Exists = actualSettings is not null
            };

            _cache.Add(cacheKey, ctxState);
        }

        return ctxState.Exists;
    }

    public async Task CreateOverrideAsync(Guid? siteId, string? hostName, string? modifiedBy)
    {
        if (string.IsNullOrWhiteSpace(modifiedBy)) throw new ArgumentNullException(nameof(modifiedBy));

        // Determine the source context to copy from (the parent in the fallback chain)
        // Creating host-level override: source is site-level (or global) with a null source host.
        // For site-level override: source is global (null, null) which is the default
        Guid? sourceSiteId = null;
        string? sourceHostName = null;

        if (!string.IsNullOrWhiteSpace(hostName))
        {
            sourceSiteId = siteId;
            sourceHostName = null;
        }

        await _repository.CreateOverrideAsync(sourceSiteId, sourceHostName, siteId, hostName, modifiedBy);

        _cache.RemoveAll();
    }

    public async Task DeleteByContextAsync(Guid? siteId, string? hostName, string? deletedBy)
    {
        if (!siteId.HasValue || siteId.Value == Guid.Empty) throw new ArgumentNullException(nameof(siteId));
        if (string.IsNullOrWhiteSpace(deletedBy)) throw new ArgumentNullException(nameof(deletedBy));

        await _repository.DeleteByContextAsync(siteId, hostName, deletedBy);

        _cache.RemoveAll();
    }

    public async Task<IEnumerable<HeaderDto>> GetCompiledHeaders(Guid? siteId, string? hostName)
    {
        var compiledHeaders = new List<HeaderDto>();
        var cacheKey = GetCacheKey(CompiledHeaderCacheKey, siteId, hostName);
        var cachedData = _cache.Get<CompiledPermissionPolicy>(cacheKey);
        if (cachedData is null)
        {
            var settings = await _repository.GetSettingsAsync(siteId, hostName);
            cachedData = new CompiledPermissionPolicy
            {
                IsEnabled = settings.IsEnabled
            };

            if (cachedData.IsEnabled)
            {
                cachedData.Directives = await _repository.ListDirectiveFragments(siteId, hostName);
            }

            _cache.Add(cacheKey, cachedData);
        }

        if (cachedData is { IsEnabled: true, Directives.Count: > 0 })
        {
            compiledHeaders.Add(new HeaderDto { Key = PermissionPolicyConstants.PermissionPolicyHeader, Value = string.Join(", ", cachedData.Directives) });
        }

        return compiledHeaders;
    }

    private static string GetCacheKey(string prefix, Guid? siteId, string? hostName)
    {
        var sitePart = siteId.HasValue && siteId.Value != Guid.Empty ? siteId.Value.ToString("N") : "global";
        var hostPart = string.IsNullOrWhiteSpace(hostName) ? string.Empty : hostName.ToLowerInvariant();
        return $"{prefix}.{sitePart}.{hostPart}";
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
