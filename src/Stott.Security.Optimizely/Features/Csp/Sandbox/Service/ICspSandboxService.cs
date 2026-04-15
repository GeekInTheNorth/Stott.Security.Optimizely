namespace Stott.Security.Optimizely.Features.Csp.Sandbox.Service;

using System;
using System.Threading.Tasks;

using Stott.Security.Optimizely.Features.Csp.Sandbox;

public interface ICspSandboxService
{
    Task<SandboxModel> GetAsync(Guid? siteId, string? hostName);

    Task SaveAsync(SandboxModel model, string? modifiedBy, Guid? siteId, string? hostName);

    Task DeleteByContextAsync(Guid? siteId, string? hostName, string? deletedBy);

    Task<bool> ExistsForContextAsync(Guid? siteId, string? hostName);
}
