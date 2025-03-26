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

    private async Task<List<CspPermissionListModel>> GetPermissionsAsync()
    {
        var cspSources = await _permissionsService.GetAsync() ?? Enumerable.Empty<CspSource>();
        var permissions = cspSources.Select(x => new CspPermissionListModel(x)).OrderBy(x => x.SortSource).ToList();

        if (!permissions.Any(x => x.Source.Equals(CspConstants.Sources.Self)))
        {
            permissions.Add(new CspPermissionListModel(CspConstants.Sources.Self, string.Join(", ", new[] { CspConstants.Directives.DefaultSource })));
        }

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