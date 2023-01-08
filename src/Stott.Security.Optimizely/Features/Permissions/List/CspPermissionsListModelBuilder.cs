namespace Stott.Security.Optimizely.Features.Permissions.List;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.Permissions.Service;

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
        var permissions = cspSources.Select(x => new CspPermissionListModel(x)).ToList();

        if (!permissions.Any(x => x.Source.Equals(CspConstants.Sources.Self)))
        {
            permissions.Add(new CspPermissionListModel(CspConstants.Sources.Self, string.Join(", ", new[] { CspConstants.Directives.DefaultSource })));
        }

        return permissions;
    }
}