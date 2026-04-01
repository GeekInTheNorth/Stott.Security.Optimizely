namespace Stott.Security.Optimizely.Features.Csp.Sandbox.Service;

using System.Threading.Tasks;
using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Features.Caching;
using Stott.Security.Optimizely.Features.Csp.Sandbox;
using Stott.Security.Optimizely.Features.Csp.Sandbox.Repository;

/// <inheritdoc cref="ICspSandboxService"/>
internal sealed class CspSandboxService(
    ICspSandboxRepository repository,
    ICacheWrapper cacheWrapper) : ICspSandboxService
{
    public async Task<SandboxModel> GetAsync(string? appId, string? hostName)
    {
        var cacheKey = GetCacheKey(CspConstants.CacheKeys.CspSandbox, appId, hostName);
        var settings = cacheWrapper.Get<SandboxModel>(cacheKey);
        if (settings is null)
        {
            settings = await repository.GetAsync(appId, hostName);

            cacheWrapper.Add(cacheKey, settings);
        }

        return settings;
    }

    public async Task SaveAsync(SandboxModel model, string? modifiedBy, string? appId, string? hostName)
    {
        if (string.IsNullOrWhiteSpace(modifiedBy))
        {
            return;
        }

        await repository.SaveAsync(model, modifiedBy, appId, hostName);

        cacheWrapper.RemoveAll();
    }

    public async Task DeleteByContextAsync(string? appId, string? hostName, string? deletedBy)
    {
        if (string.IsNullOrWhiteSpace(appId) || string.IsNullOrWhiteSpace(deletedBy))
        {
            return;
        }

        await repository.DeleteByContextAsync(appId, hostName, deletedBy);

        cacheWrapper.RemoveAll();
    }

    public async Task<bool> ExistsForContextAsync(string? appId, string? hostName)
    {
        if (string.IsNullOrWhiteSpace(appId) && string.IsNullOrWhiteSpace(hostName))
        {
            return true;
        }

        var cacheKey = GetCacheKey(CspConstants.CacheKeys.CspInheritedSandbox, appId, hostName);
        var ctxState = cacheWrapper.Get<ContextStateModel>(cacheKey);
        if (ctxState is null)
        {
            var actualSettings = await repository.GetByContextAsync(appId, hostName);
            ctxState = new ContextStateModel
            {
                Exists = actualSettings is not null
            };

            cacheWrapper.Add(cacheKey, ctxState);
        }

        return ctxState.Exists;
    }

    private static string GetCacheKey(string prefix, string? appId, string? hostName)
    {
        return $"{prefix}.{appId}.{hostName}";
    }
}
