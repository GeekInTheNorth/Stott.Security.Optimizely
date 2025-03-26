namespace Stott.Security.Optimizely.Features.Csp.Sandbox.Repository;

using System.Threading.Tasks;
using Stott.Security.Optimizely.Features.Csp.Sandbox;

public interface ICspSandboxRepository
{
    Task<SandboxModel> GetAsync();

    Task SaveAsync(SandboxModel model, string modifiedBy);
}