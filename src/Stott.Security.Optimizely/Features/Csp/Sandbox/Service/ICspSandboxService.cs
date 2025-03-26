namespace Stott.Security.Optimizely.Features.Csp.Sandbox.Service;

using System.Threading.Tasks;
using Stott.Security.Optimizely.Features.Csp.Sandbox;

public interface ICspSandboxService
{
    Task<SandboxModel> GetAsync();

    Task SaveAsync(SandboxModel model, string? modifiedBy);
}