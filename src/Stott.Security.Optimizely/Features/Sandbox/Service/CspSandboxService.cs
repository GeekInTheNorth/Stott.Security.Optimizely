namespace Stott.Security.Optimizely.Features.Sandbox.Service;

using System.Threading.Tasks;

using Stott.Security.Optimizely.Features.Caching;
using Stott.Security.Optimizely.Features.Sandbox.Repository;

internal sealed class CspSandboxService : ICspSandboxService
{
    private readonly ICspSandboxRepository _repository;

    private readonly ICacheWrapper _cacheWrapper;

    public CspSandboxService(
        ICspSandboxRepository repository,
        ICacheWrapper cacheWrapper)
    {
        _repository = repository;
        _cacheWrapper = cacheWrapper;
    }

    public async Task<SandboxModel> GetAsync()
    {
        return await _repository.GetAsync();
    }

    public async Task SaveAsync(SandboxModel model, string? modifiedBy)
    {
        if (string.IsNullOrWhiteSpace(modifiedBy))
        {
            return;
        }

        await _repository.SaveAsync(model, modifiedBy);

        _cacheWrapper.RemoveAll();
    }
}