using System.Collections.Generic;

namespace Stott.Optimizely.Csp.Features.Permissions.List
{
    public class CspPermissionsViewModel
    {
        public List<string> AllowedDirectives { get; internal set; }

        public IList<CspPermissionViewModel> Permissions { get; set; }
    }
}
