namespace Stott.Security.Optimizely.Features.Csp.Permissions.List;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.Csp.Permissions.Service;

internal class CspPermissionsListModelBuilder(ICspPermissionService permissionsService) : ICspPermissionsListModelBuilder
{
    private string? _sourceFilter;

    private string? _directiveFilter;

    private string? _appId;

    private string? _hostName;

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

    public ICspPermissionsListModelBuilder WithAppId(string? appId)
    {
        _appId = appId;

        return this;
    }

    public ICspPermissionsListModelBuilder WithHostName(string? hostName)
    {
        _hostName = hostName;

        return this;
    }

    private async Task<List<CspPermissionListModel>> GetPermissionsAsync()
    {
        var cspSources = await permissionsService.GetByContextAsync(_appId, _hostName) ?? Enumerable.Empty<CspSource>();
        var permissions = cspSources.Select(x => new CspPermissionListModel(x)).OrderBy(x => x.SortSource).ToList();

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
