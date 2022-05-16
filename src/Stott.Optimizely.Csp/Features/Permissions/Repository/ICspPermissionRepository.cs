using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Stott.Optimizely.Csp.Entities;

namespace Stott.Optimizely.Csp.Features.Permissions.Repository
{
    public interface ICspPermissionRepository
    {
        Task<IList<CspSource>> GetAsync();

        IList<CspSource> GetCmsRequirements();

        Task DeleteAsync(Guid id);

        Task SaveAsync(Guid id, string source, List<string> directives);

        Task AppendDirectiveAsync(string source, string directive);
    }
}
