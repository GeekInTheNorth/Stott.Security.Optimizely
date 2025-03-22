using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Stott.Security.Optimizely.Features.Caching;
using Stott.Security.Optimizely.Features.PermissionPolicy.Models;
using Stott.Security.Optimizely.Features.PermissionPolicy.Repository;

namespace Stott.Security.Optimizely.Features.PermissionPolicy.Service;

public sealed class PermissionPolicyService : IPermissionPolicyService
{
    private readonly ICacheWrapper _cache;

    private readonly IPermissionPolicyRepository _repository;

    private const string SettingsCacheKey = "stott.security.permissionpolicy.settings";

    private const string DirectivesCacheKey = "stott.security.permissionpolicy.directives";

    private const string CompiledHeaderCacheKey = "stott.security.permissionpolicy.compiled";

    public PermissionPolicyService(ICacheWrapper cache, IPermissionPolicyRepository repository)
    {
        _cache = cache;
        _repository = repository;
    }

    public async Task<IPermissionPolicySettings> GetPermissionPolicySettingsAsync()
    {
        var settings = _cache.Get<PermissionPolicySettingsModel>(SettingsCacheKey);
        if (settings is null)
        {
            settings = await _repository.GetSettingsAsync();
            _cache.Add(SettingsCacheKey, settings);
        }

        return settings;
    }

    public async Task<IList<PermissionPolicyDirectiveModel>> ListDirectivesAsync(string? sourceFilter, PermissionPolicyEnabledFilter enabledFilter)
    {
        var directives = _cache.Get<List<PermissionPolicyDirectiveModel>>(DirectivesCacheKey);
        if (directives is null)
        {
            directives = await _repository.ListDirectivesAsync();
            _cache.Add(DirectivesCacheKey, directives);
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

    public async Task<IEnumerable<KeyValuePair<string, string>>> GetCompiledHeaders()
    {
        var compiledHeaders = new List<KeyValuePair<string, string>>();
        var cachedData = _cache.Get<CompiledPermissionPolicy>(CompiledHeaderCacheKey);
        if (cachedData is null)
        {
            var settings = await _repository.GetSettingsAsync();
            cachedData = new CompiledPermissionPolicy
            {
                IsEnabled = settings.IsEnabled
            };

            if (cachedData.IsEnabled)
            {
                cachedData.Directives = await _repository.ListDirectiveFragments();
            }

            _cache.Add(CompiledHeaderCacheKey, cachedData);
        }

        if (cachedData is { IsEnabled: true, Directives.Count: > 0 })
        {
            compiledHeaders.Add(new KeyValuePair<string, string>(PermissionPolicyConstants.PermissionPolicyHeader, string.Join(", ", cachedData.Directives)));
        }

        return compiledHeaders;
    }

    public async Task SaveDirectiveAsync(SavePermissionPolicyModel? model, string? modifiedBy)
    {
        if (model is null) throw new ArgumentNullException(nameof(model));
        if (string.IsNullOrWhiteSpace(modifiedBy)) throw new ArgumentNullException(nameof(modifiedBy));

        await _repository.SaveDirectiveAsync(model, modifiedBy);

        _cache.RemoveAll();
    }

    public async Task SaveSettingsAsync(IPermissionPolicySettings? settings, string? modifiedBy)
    {
        if (settings is null) throw new ArgumentNullException(nameof(settings));
        if (string.IsNullOrWhiteSpace(modifiedBy)) throw new ArgumentNullException(nameof(modifiedBy));

        await _repository.SaveSettingsAsync(settings, modifiedBy);

        _cache.RemoveAll();
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