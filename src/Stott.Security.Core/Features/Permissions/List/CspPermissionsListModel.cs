using System.Collections.Generic;

namespace Stott.Security.Core.Features.Permissions.List
{
    public class CspPermissionsListModel
    {
        public List<string> AllowedDirectives { get; internal set; }

        public IList<CspPermissionListModel> Permissions { get; set; }
    }
}
