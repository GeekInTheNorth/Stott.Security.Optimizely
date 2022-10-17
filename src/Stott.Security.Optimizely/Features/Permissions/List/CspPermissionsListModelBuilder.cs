namespace Stott.Security.Optimizely.Features.Permissions.List;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.Permissions.Service;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class CspPermissionsListModelBuilder : ICspPermissionsListModelBuilder
{
    private readonly ICspPermissionService _permissionsService;

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

    private async Task<List<CspPermissionListModel>> GetPermissionsAsync()
    {
        var cspSources = await _permissionsService.GetAsync() ?? Enumerable.Empty<CspSource>();
        var permissions = cspSources.Select(x => new CspPermissionListModel
        {
            Id = x.Id,
            Source = x.Source,
            Directives = x.Directives
        }).ToList();

        if (!permissions.Any(x => x.Source.Equals(CspConstants.Sources.Self)))
        {
            permissions.Add(new CspPermissionListModel
            {
                Id = Guid.Empty,
                Source = CspConstants.Sources.Self,
                Directives = string.Join(", ", new[] { CspConstants.Directives.DefaultSource })
            });
        }

        return permissions;
    }
}
