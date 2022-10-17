namespace Stott.Security.Optimizely.Features.Permissions.Service;

using Stott.Security.Optimizely.Entities;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface ICspPermissionService
{
    Task<IList<CspSource>> GetAsync();

    IList<CspSource> GetCmsRequirements();

    Task DeleteAsync(Guid id);

    Task SaveAsync(Guid id, string source, List<string> directives);

    Task AppendDirectiveAsync(string source, string directive);
}
