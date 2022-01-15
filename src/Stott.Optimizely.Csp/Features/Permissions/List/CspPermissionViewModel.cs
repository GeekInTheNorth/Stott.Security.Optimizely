using System;

namespace Stott.Optimizely.Csp.Features.Permissions.List
{
    public class CspPermissionViewModel
    {
        public Guid Id { get; set; }

        public string Source { get; set; }

        public string Directives { get; set; }
    }
}
