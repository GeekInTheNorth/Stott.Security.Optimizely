namespace Stott.Security.Optimizely.Features.Cors.Provider;

using System;
using System.Threading.Tasks;

using EPiServer.ServiceLocation;

using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Features.Caching;
using Stott.Security.Optimizely.Features.Cors.Service;

public sealed class CustomCorsPolicyProvider : DefaultCorsPolicyProvider, ICorsPolicyProvider
{
    private readonly ICacheWrapper _cache;

    private const string CacheKey = "stott.security.cors.config";

    public CustomCorsPolicyProvider(
        ICacheWrapper cache, 
        IOptions<CorsOptions> corsOptions)
        : base(corsOptions)
    {
        _cache = cache;
    }

    public new async Task<CorsPolicy?> GetPolicyAsync(HttpContext context, string? policyName)
    {
        CorsPolicy? configuredPolicy = null;
        
        // If policy name has been defined and it's not the policy defined by this module
        // then attempt to retrieve the configuration from code.
        if (!string.IsNullOrWhiteSpace(policyName) && !CspConstants.CorsPolicy.Equals(policyName))
        {
            configuredPolicy = await base.GetPolicyAsync(context, policyName);
        }
        
        // If the policy configured in code exists, then exit here.
        if (configuredPolicy != null)
        {
            return configuredPolicy;
        }

        // Use the CORS Policy configured within this module
        var policy = _cache.Get<CorsPolicy>(CacheKey);
        if (policy == null)
        {
            policy = await LoadPolicy();
            _cache.Add(CacheKey, policy);
        }

        return policy;
    }

    private static async Task<CorsPolicy> LoadPolicy()
    {
        var service = ServiceLocator.Current.GetInstance<ICorsSettingsService>();
        var configuration = await service.GetAsync();
        var policy = new CorsPolicy();

        if (!configuration.IsEnabled)
        {
            return policy;
        }
        
        if (configuration.AllowHeaders.Count > 0)
        {
            foreach (var allowHeader in configuration.AllowHeaders)
            {
                policy.Headers.Add(allowHeader.Value);
            }
        }
        else
        {
            policy.Headers.Add("*");
        }

        if (configuration.AllowOrigins.Count > 0)
        {
            foreach (var allowOrigin in configuration.AllowOrigins)
            {
                policy.Origins.Add(allowOrigin.Value);
            }
        }
        else
        {
            policy.Origins.Add("*");
        }

        if (configuration.AllowMethods.IsAllowAllMethods)
        {
            policy.Methods.Add("*");
        }
        else
        {
            if (configuration.AllowMethods.IsAllowConnectMethods) policy.Methods.Add(HttpMethods.Connect);
            if (configuration.AllowMethods.IsAllowDeleteMethods) policy.Methods.Add(HttpMethods.Delete);
            if (configuration.AllowMethods.IsAllowGetMethods) policy.Methods.Add(HttpMethods.Get);
            if (configuration.AllowMethods.IsAllowHeadMethods) policy.Methods.Add(HttpMethods.Head);
            if (configuration.AllowMethods.IsAllowOptionsMethods) policy.Methods.Add(HttpMethods.Options);
            if (configuration.AllowMethods.IsAllowPatchMethods) policy.Methods.Add(HttpMethods.Patch);
            if (configuration.AllowMethods.IsAllowPostMethods) policy.Methods.Add(HttpMethods.Post);
            if (configuration.AllowMethods.IsAllowPutMethods) policy.Methods.Add(HttpMethods.Put);
            if (configuration.AllowMethods.IsAllowTraceMethods) policy.Methods.Add(HttpMethods.Trace);
        }

        foreach (var exposeHeader in configuration.ExposeHeaders)
        {
            policy.ExposedHeaders.Add(exposeHeader.Value);
        }

        policy.SupportsCredentials = configuration.AllowCredentials && !policy.Origins.Contains("*");
        policy.PreflightMaxAge = TimeSpan.FromSeconds(configuration.MaxAge);

        return policy;
    }
}