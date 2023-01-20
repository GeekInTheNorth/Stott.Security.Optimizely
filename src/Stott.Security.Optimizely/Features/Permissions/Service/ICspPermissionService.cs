namespace Stott.Security.Optimizely.Features.Permissions.Service;

using Stott.Security.Optimizely.Entities;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface ICspPermissionService
{
    Task<IList<CspSource>> GetAsync();

    Task DeleteAsync(Guid id, string? deletedBy);

    Task SaveAsync(Guid id, string? source, List<string>? directives, string? modifiedBy);

    Task AppendDirectiveAsync(string? source, string? directive, string? modifiedBy);
}