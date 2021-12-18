using System;
using System.Collections.Generic;

namespace Stott.Optimizely.Csp.Features.Permissions.List
{
    public class CspPermissionViewModel
    {
        public Guid Id { get; set; }

        public string Source { get; set; }

        public IList<string> Directives { get; set; }

        public string DirectivesList => string.Join(", ", Directives);
    }
}
