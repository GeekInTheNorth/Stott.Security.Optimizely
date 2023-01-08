namespace Stott.Security.Optimizely.Features.Sandbox.Service;

using System.Threading.Tasks;

public interface ICspSandboxService
{
    Task<SandboxModel> GetAsync();

    Task SaveAsync(SandboxModel model, string? modifiedBy);
}