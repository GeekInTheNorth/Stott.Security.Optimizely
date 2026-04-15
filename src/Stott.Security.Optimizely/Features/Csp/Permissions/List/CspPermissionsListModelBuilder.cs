namespace Stott.Security.Optimizely.Features.Csp.Permissions.List;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.Csp.Permissions.Service;

internal class CspPermissionsListModelBuilder : ICspPermissionsListModelBuilder
{
    private readonly ICspPermissionService _permissionsService;

    private string? _sourceFilter;

    private string? _directiveFilter;

    private Guid? _siteId;

    private string? _hostName;

    public CspPermissionsListModelBuilder(ICspPermissionService permissionsService)
    {
        _permissionsService = permissionsService;
    }

    public async Task<CspPermissionsListModel> BuildAsync()
    {
        return new CspPermissionsListModel
        {
            AllowedDirectives = CspConstants.AllDirectives,
            Permissions = await GetPermissionsAsync()
        };
    }

    public ICspPermissionsListModelBuilder WithDirectiveFilter(string? directive)
    {
        _directiveFilter = CspConstants.AllDirectives.FirstOrDefault(x => x.Equals(directive, StringComparison.OrdinalIgnoreCase));

        return this;
    }

    public ICspPermissionsListModelBuilder WithSourceFilter(string? source)
    {
        _sourceFilter = source;

        return this;
    }

    public ICspPermissionsListModelBuilder WithSiteId(Guid? siteId)
    {
        _siteId = siteId;

        return this;
    }

    public ICspPermissionsListModelBuilder WithHostName(string? hostName)
    {
        _hostName = hostName;

        return this;
    }

    private async Task<List<CspPermissionListModel>> GetPermissionsAsync()
    {
        var cspSources = await _permissionsService.GetAllAsync() ?? Enumerable.Empty<CspSource>().ToList();

        var hasSiteId = _siteId.HasValue && _siteId.Value != Guid.Empty;

        if (hasSiteId)
        {
            cspSources = cspSources.Where(x => x.SiteId == null || x.SiteId == _siteId).ToList();
        }

        if (hasSiteId && !string.IsNullOrWhiteSpace(_hostName))
        {
            cspSources = cspSources.Where(x => string.IsNullOrWhiteSpace(x.HostName) || string.Equals(x.HostName, _hostName, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        var permissions = cspSources.Select(x => new CspPermissionListModel(x, _siteId, _hostName)).OrderBy(x => x.InheritanceLevel).ThenBy(x => x.SortSource).ToList();

        if (!string.IsNullOrWhiteSpace(_sourceFilter))
        {
            permissions = permissions.Where(x => x.Source.Contains(_sourceFilter, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        if (!string.IsNullOrWhiteSpace(_directiveFilter))
        {
            permissions = permissions.Where(x => x.DirectiveList.Contains(_directiveFilter)).ToList();
        }

        return permissions;
    }
}
