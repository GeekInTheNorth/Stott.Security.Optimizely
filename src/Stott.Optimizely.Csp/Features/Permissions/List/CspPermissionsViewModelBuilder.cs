using System;
using System.Collections.Generic;

using Stott.Optimizely.Csp.Common;

namespace Stott.Optimizely.Csp.Features.Permissions.List
{
    public class CspPermissionsViewModelBuilder : ICspPermissionsViewModelBuilder
    {
        public CspPermissionsViewModel Build()
        {
            return new CspPermissionsViewModel
            {
                AllowedDirectives = CspConstants.Directives,
                Permissions = new List<CspPermissionViewModel> {
                    new CspPermissionViewModel
                    {
                        Id = Guid.NewGuid(),
                        Source = "'self'",
                        Directives = new List<string> { "default-src" }
                    }
                }
            };
        }
    }
}
