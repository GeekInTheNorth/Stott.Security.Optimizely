using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Stott.Security.Core.Entities;

namespace Stott.Security.Core.Features.Permissions.Repository
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
