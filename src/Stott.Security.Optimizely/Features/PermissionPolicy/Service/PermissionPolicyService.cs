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

    private const string CacheKey = "stott.security.permissionpolicy.data";

    private const string FragmentCacheKey = "stott.security.permissionpolicy.fragments";

    public PermissionPolicyService(ICacheWrapper cache, IPermissionPolicyRepository repository)
    {
        _cache = cache;
        _repository = repository;
    }

    public async Task<IList<PermissionPolicyDirectiveModel>> List(string? sourceFilter, PermissionPolicyEnabledFilter enabledFilter)
    {
        var directives = _cache.Get<List<PermissionPolicyDirectiveModel>>(CacheKey);
        if (directives is null)
        {
            directives = await _repository.List();
            _cache.Add(CacheKey, directives);
        }

        foreach (var directive in PermissionPolicyConstants.AllDirectives)
        {
            if (!directives.Any(x => string.Equals(directive, x.Name)))
            {
                directives.Add(new PermissionPolicyDirectiveModel
                {
                    Name = directive,
                    EnabledState = PermissionPolicyEnabledState.None,
                    Sources = new List<PermissionPolicyUrl>(0)
                });
            }
        }

        return directives.Where(x => IsMatch(x, sourceFilter, enabledFilter)).ToList();
    }

    public async Task<IEnumerable<KeyValuePair<string, string>>> GetCompiledHeaders()
    {
        var fragments = _cache.Get<List<string>>(FragmentCacheKey);
        if (fragments is null)
        {
            fragments = await _repository.ListFragments();
            _cache.Add(FragmentCacheKey, fragments);
        }

        var headerValue = string.Join(", ", fragments);

        return new List<KeyValuePair<string, string>>
        {
            new(PermissionPolicyConstants.PermissionPolicyHeader, headerValue)
        };
    }

    public async Task Save(SavePermissionPolicyModel model, string? modifiedBy)
    {
        if (model is null) throw new ArgumentNullException(nameof(model));
        if (string.IsNullOrWhiteSpace(modifiedBy)) throw new ArgumentNullException(nameof(modifiedBy));

        await _repository.Save(model, modifiedBy);

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
            PermissionPolicyEnabledFilter.AllEnabled => model.EnabledState != PermissionPolicyEnabledState.None,
            PermissionPolicyEnabledFilter.Disabled => model.EnabledState == PermissionPolicyEnabledState.None,
            PermissionPolicyEnabledFilter.AllSites => model.EnabledState == PermissionPolicyEnabledState.All,
            PermissionPolicyEnabledFilter.ThisSite => model.EnabledState == PermissionPolicyEnabledState.ThisSite,
            PermissionPolicyEnabledFilter.ThisAndSpecificSites => model.EnabledState == PermissionPolicyEnabledState.ThisAndSpecificSites,
            PermissionPolicyEnabledFilter.SpecificSites => model.EnabledState == PermissionPolicyEnabledState.SpecificSites,
            _ => true,
        };
    }
}
