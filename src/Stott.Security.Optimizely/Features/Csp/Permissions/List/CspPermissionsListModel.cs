namespace Stott.Security.Optimizely.Features.Csp.Permissions.List;

using System.Collections.Generic;

public sealed class CspPermissionsListModel
{
    public List<string>? AllowedDirectives { get; set; }

    public IList<CspPermissionListModel>? Permissions { get; set; }
}