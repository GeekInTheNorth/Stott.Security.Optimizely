using System.Collections.Generic;

using Stott.Optimizely.Csp.Entities;

namespace Stott.Optimizely.Csp.Features.Permissions.Repository
{
    public interface ICspPermissionsRepository
    {
        IEnumerable<CspSource> List();
    }
}
