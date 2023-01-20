namespace Stott.Security.Optimizely.Features.Permissions.Repository;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Stott.Security.Optimizely.Entities;

public interface ICspPermissionRepository
{
    Task<IList<CspSource>> GetAsync();

    Task DeleteAsync(Guid id, string deletedBy);

    Task SaveAsync(Guid id, string source, List<string> directives, string modifiedBy);

    Task AppendDirectiveAsync(string source, string directive, string modifiedBy);
}
