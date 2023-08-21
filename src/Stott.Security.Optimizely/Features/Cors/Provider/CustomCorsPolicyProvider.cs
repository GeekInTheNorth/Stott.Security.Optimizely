namespace Stott.Security.Optimizely.Features.Cors.Provider;

using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;

using Stott.Security.Optimizely.Features.Caching;
using Stott.Security.Optimizely.Features.Cors.Repository;

public sealed class CustomCorsPolicyProvider : ICorsPolicyProvider
{
    private readonly ICacheWrapper _cache;

    private readonly ICorsSettingsRepository _repository;

    private const string CacheKey = "stott.security.cors.config";

    public CustomCorsPolicyProvider(ICacheWrapper cache, ICorsSettingsRepository repository)
    {
        _cache = cache;
        _repository = repository;
    }

    public async Task<CorsPolicy?> GetPolicyAsync(HttpContext context, string? policyName)
    {
        var policy = _cache.Get<CorsPolicy>(CacheKey);
        if (policy == null)
        {
            policy = await LoadPolicy();
            _cache.Add(CacheKey, policy);
        }

        return policy;
    }

    private async Task<CorsPolicy> LoadPolicy()
    {
        var configutation = await _repository.GetAsync();

        var policy = new CorsPolicy();

        if (!configutation.IsEnabled)
        {
            return policy;
        }
        
        if (configutation.AllowHeaders.Count > 0)
        {
            foreach (var allowHeader in configutation.AllowHeaders)
            {
                policy.Headers.Add(allowHeader.Value);
            }
        }
        else
        {
            policy.Headers.Add("*");
        }

        if (configutation.AllowOrigins.Count > 0)
        {
            foreach (var allowOrigin in configutation.AllowOrigins)
            {
                policy.Origins.Add(allowOrigin.Value);
            }
        }
        else
        {
            policy.Origins.Add("*");
        }

        if (configutation.AllowMethods.IsAllowAllMethods)
        {
            policy.Methods.Add("*");
        }
        else
        {
            if (configutation.AllowMethods.IsAllowConnectMethods) policy.Methods.Add(HttpMethods.Connect);
            if (configutation.AllowMethods.IsAllowDeleteMethods) policy.Methods.Add(HttpMethods.Delete);
            if (configutation.AllowMethods.IsAllowGetMethods) policy.Methods.Add(HttpMethods.Get);
            if (configutation.AllowMethods.IsAllowHeadMethods) policy.Methods.Add(HttpMethods.Head);
            if (configutation.AllowMethods.IsAllowOptionsMethods) policy.Methods.Add(HttpMethods.Options);
            if (configutation.AllowMethods.IsAllowPatchMethods) policy.Methods.Add(HttpMethods.Patch);
            if (configutation.AllowMethods.IsAllowPostMethods) policy.Methods.Add(HttpMethods.Post);
            if (configutation.AllowMethods.IsAllowPutMethods) policy.Methods.Add(HttpMethods.Put);
            if (configutation.AllowMethods.IsAllowTraceMethods) policy.Methods.Add(HttpMethods.Trace);
        }

        foreach (var exposeHeader in configutation.ExposeHeaders)
        {
            policy.ExposedHeaders.Add(exposeHeader.Value);
        }

        policy.SupportsCredentials = configutation.AllowCredentials;
        policy.PreflightMaxAge = TimeSpan.FromSeconds(configutation.MaxAge);

        return policy;
    }
}