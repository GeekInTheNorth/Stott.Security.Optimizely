using System;
using System.Collections.Generic;

namespace Stott.Optimizely.Csp.Features.Permissions.Save
{
    public interface ISaveCspPermissionsCommand
    {
        void Save(Guid id, string source, List<string> directives);
    }
}
