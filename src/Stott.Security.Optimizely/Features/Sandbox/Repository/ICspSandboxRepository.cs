namespace Stott.Security.Optimizely.Features.Sandbox.Repository;

using System.Threading.Tasks;

public interface ICspSandboxRepository
{
    Task<SandboxModel> Get();

    Task Save(SandboxModel model);
}