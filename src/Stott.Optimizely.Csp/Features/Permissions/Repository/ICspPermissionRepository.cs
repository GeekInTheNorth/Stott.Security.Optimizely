using System;
using System.Collections.Generic;

using Stott.Optimizely.Csp.Entities;

namespace Stott.Optimizely.Csp.Features.Permissions.Repository
{
    public interface ICspPermissionRepository
    {
        IList<CspSource> Get();

        IList<CspSource> GetCmsRequirements();

        void Delete(Guid id);

        void Save(Guid id, string source, List<string> directives);

        void AppendDirective(string source, string directive);
    }
}
