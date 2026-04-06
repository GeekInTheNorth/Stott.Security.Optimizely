namespace Stott.Security.Optimizely.Features.Csp.Permissions.Service;

using Stott.Security.Optimizely.Entities;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface ICspPermissionService
{
    Task<IList<CspSource>> GetAsync(string? appId, string? hostName);

    Task<IList<CspSource>> GetAllAsync();

    Task DeleteAsync(Guid id, string? deletedBy);

    Task SaveAsync(Guid id, string? source, List<string>? directives, string? modifiedBy, string? appId, string? hostName);

    Task AppendDirectiveAsync(string? source, string? directive, string? modifiedBy, string? appId, string? hostName);
}
