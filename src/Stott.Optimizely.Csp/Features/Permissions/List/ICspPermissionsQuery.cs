using System.Collections.Generic;

using Stott.Optimizely.Csp.Entities;

namespace Stott.Optimizely.Csp.Features.Permissions.List
{
    public interface ICspPermissionsQuery
    {
        IList<CspSource> Get();
    }
}
