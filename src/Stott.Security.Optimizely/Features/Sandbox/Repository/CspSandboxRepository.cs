namespace Stott.Security.Optimizely.Features.Sandbox.Repository;

using System.Threading.Tasks;

public class CspSandboxRepository : ICspSandboxRepository
{
    public Task<SandboxModel> Get()
    {
        return Task.FromResult(new SandboxModel());
    }

    public Task Save(SandboxModel model)
    {
        return Task.CompletedTask;
    }
}