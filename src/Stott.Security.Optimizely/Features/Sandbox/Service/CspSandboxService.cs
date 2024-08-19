namespace Stott.Security.Optimizely.Features.Sandbox.Service;

using System.Threading.Tasks;

using Stott.Security.Optimizely.Features.Caching;
using Stott.Security.Optimizely.Features.Sandbox.Repository;

internal sealed class CspSandboxService : ICspSandboxService
{
    private readonly ICspSandboxRepository _repository;

    private readonly ICacheWrapper _cacheWrapper;

    private const string CacheKey = "stott.security.csp.sandbox";

    public CspSandboxService(
        ICspSandboxRepository repository,
        ICacheWrapper cacheWrapper)
    {
        _repository = repository;
        _cacheWrapper = cacheWrapper;
    }

    public async Task<SandboxModel> GetAsync()
    {
        var settings = _cacheWrapper.Get<SandboxModel>(CacheKey);
        if (settings is null)
        {
            settings = await _repository.GetAsync();

            _cacheWrapper.Add(CacheKey, settings);
        }

        return settings;
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