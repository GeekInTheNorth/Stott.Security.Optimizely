namespace Stott.Security.Optimizely.Features.Csp.Sandbox.Repository;

using System.Threading.Tasks;
using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.Csp.Sandbox;

public interface ICspSandboxRepository
{
    Task<SandboxModel> GetAsync(string? appId, string? hostName);

    Task<CspSandbox?> GetByContextAsync(string? appId, string? hostName);

    Task SaveAsync(SandboxModel model, string modifiedBy, string? appId, string? hostName);

    Task DeleteByContextAsync(string? appId, string? hostName, string deletedBy);
}
