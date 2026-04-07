namespace Stott.Security.Optimizely.Features.Csp.Permissions.Repository;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Stott.Security.Optimizely.Entities;

public interface ICspPermissionRepository
{
    Task<IList<CspSource>> GetAllAsync();

    Task<IList<CspSource>> GetAsync(string? appId, string? hostName);

    Task<CspSource?> GetBySourceAsync(string? source, string? appId, string? hostName);

    Task DeleteAsync(Guid id, string deletedBy);

    Task SaveAsync(Guid id, string source, List<string> directives, string modifiedBy, string? appId, string? hostName);

    Task AppendDirectiveAsync(string source, string directive, string modifiedBy, string? appId, string? hostName);
}
