namespace Stott.Security.Optimizely.Features.Csp.Sandbox.Service;

using System.Threading.Tasks;

using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.Csp.Sandbox;

public interface ICspSandboxService
{
    Task<SandboxModel> GetAsync(string? appId, string? hostName);

    Task<CspSandbox?> GetByContextAsync(string? appId, string? hostName);

    Task SaveAsync(SandboxModel model, string? modifiedBy, string? appId, string? hostName);

    Task DeleteByContextAsync(string? appId, string? hostName, string? deletedBy);
}
