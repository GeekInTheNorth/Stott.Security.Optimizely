namespace Stott.Security.Optimizely.Features.Sandbox.Repository;

using System.Threading.Tasks;

public interface ICspSandboxRepository
{
    Task<SandboxModel> GetAsync();

    Task SaveAsync(SandboxModel model, string modifiedBy);
}