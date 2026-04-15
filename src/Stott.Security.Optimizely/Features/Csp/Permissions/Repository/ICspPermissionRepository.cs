namespace Stott.Security.Optimizely.Features.Csp.Permissions.Repository;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Stott.Security.Optimizely.Entities;

public interface ICspPermissionRepository
{
    Task<IList<CspSource>> GetAllAsync();

    Task<IList<CspSource>> GetAsync(Guid? siteId, string? hostName);

    Task<CspSource?> GetBySourceAsync(string? source, Guid? siteId, string? hostName);

    Task DeleteAsync(Guid id, string deletedBy);

    Task SaveAsync(Guid id, string source, List<string> directives, string modifiedBy, Guid? siteId, string? hostName);

    Task AppendDirectiveAsync(string source, string directive, string modifiedBy, Guid? siteId, string? hostName);
}
