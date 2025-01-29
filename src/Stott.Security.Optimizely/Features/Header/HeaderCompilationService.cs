namespace Stott.Security.Optimizely.Features.Header;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using EPiServer.Core;
using EPiServer.ServiceLocation;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.Caching;
using Stott.Security.Optimizely.Features.Nonce;
using Stott.Security.Optimizely.Features.Pages;
using Stott.Security.Optimizely.Features.PermissionPolicy.Service;
using Stott.Security.Optimizely.Features.Permissions.Repository;
using Stott.Security.Optimizely.Features.Sandbox;
using Stott.Security.Optimizely.Features.Sandbox.Repository;
using Stott.Security.Optimizely.Features.SecurityHeaders.Service;
using Stott.Security.Optimizely.Features.Settings;
using Stott.Security.Optimizely.Features.Settings.Repository;

internal sealed class HeaderCompilationService : IHeaderCompilationService
{
    private readonly ICspContentBuilder _cspContentBuilder;

    private readonly ICspReportUrlResolver _cspReportUrlResolver;

    private readonly INonceProvider _nonceProvider;

    private readonly ICacheWrapper _cacheWrapper;

    public HeaderCompilationService(
        ICspContentBuilder cspContentBuilder,
        ICspReportUrlResolver cspReportUrlResolver,
        INonceProvider nonceProvider,
        ICacheWrapper cacheWrapper)
    {
        _cspContentBuilder = cspContentBuilder;
        _cspReportUrlResolver = cspReportUrlResolver;
        _nonceProvider = nonceProvider;
        _cacheWrapper = cacheWrapper;
    }

    public async Task<Dictionary<string, string>> GetSecurityHeadersAsync(PageData? pageData)
    {
        var host = _cspReportUrlResolver.GetHost();
        var cacheKey = GetCacheKey(pageData, host);
        var headers = _cacheWrapper.Get<Dictionary<string, string>>(cacheKey);
        if (headers == null)
        {
            headers = await CompileSecurityHeadersAsync(pageData as IContentSecurityPolicyPage);

            _cacheWrapper.Add(cacheKey, headers);
        }

        // We do not want to mutate the headers in the cache as this will break functionality.
        var clonedHeaders = new Dictionary<string, string>(headers);

        SetNonceValue(clonedHeaders);

        return clonedHeaders;
    }

    private static string GetCacheKey(PageData? pageData, string host)
    {
        var shouldCacheForPage = pageData is IContentSecurityPolicyPage { ContentSecurityPolicySources.Count: > 0 };

        return shouldCacheForPage ? $"{CspConstants.CacheKeys.CompiledHeaders}_{host}_{pageData?.ContentLink}_{pageData?.Changed.Ticks}" : CspConstants.CacheKeys.CompiledHeaders;
    }

    private async Task<Dictionary<string, string>> CompileSecurityHeadersAsync(IContentSecurityPolicyPage? cspPage)
    {
        var securityHeaders = new Dictionary<string, string>();

        var cspSettingsService = ServiceLocator.Current.GetInstance<ICspSettingsRepository>();
        var cspSettings = await cspSettingsService.GetAsync();
        if (cspSettings?.IsEnabled ?? false)
        {
            var cspContent = await GetCspContentAsync(cspSettings, cspPage);

            var reportingEndPoints = GetReportingEndPoints(cspSettings).ToList();
            if (reportingEndPoints.Any())
            {
                securityHeaders.Add(CspConstants.HeaderNames.ReportingEndpoints, string.Join(", ", reportingEndPoints));
            }

            if (cspSettings.IsReportOnly)
            {
                securityHeaders.Add(CspConstants.HeaderNames.ReportOnlyContentSecurityPolicy, cspContent);
            }
            else
            {
                securityHeaders.Add(CspConstants.HeaderNames.ContentSecurityPolicy, cspContent);
            }
        }

        var securityHeaderService = ServiceLocator.Current.GetInstance<ISecurityHeaderService>();
        var responseHeaders = await securityHeaderService.GetCompiledHeaders();
        if (responseHeaders is not null)
        {
            foreach (var header in responseHeaders)
            {
                securityHeaders.Add(header.Key, header.Value);
            }
        }

        var permissionPolicyService = ServiceLocator.Current.GetInstance<IPermissionPolicyService>();
        var permissionPolicyHeaders = await permissionPolicyService.GetCompiledHeaders();
        if (permissionPolicyHeaders is not null)
        {
            foreach (var header in permissionPolicyHeaders)
            {
                securityHeaders.Add(header.Key, header.Value);
            }
        }

        return securityHeaders;
    }

    private async Task<string> GetCspContentAsync(CspSettings cspSettings, IContentSecurityPolicyPage? cspPage)
    {
        var cspSandboxRepository = ServiceLocator.Current.GetInstance<ICspSandboxRepository>();
        var cspPermissionRepository = ServiceLocator.Current.GetInstance<ICspPermissionRepository>();

        var cspSandbox = await cspSandboxRepository.GetAsync() ?? new SandboxModel();
        var cspSources = await cspPermissionRepository.GetAsync() ?? new List<CspSource>(0);
        var pageSources = cspPage?.ContentSecurityPolicySources ?? new List<PageCspSourceMapping>(0);

        var allSources = new List<ICspSourceMapping>();
        allSources.AddRange(cspSources);
        allSources.AddRange(pageSources);
        
        return _cspContentBuilder.WithSources(allSources)
                                 .WithSettings(cspSettings)
                                 .WithSandbox(cspSandbox)
                                 .BuildAsync();
    }

    private static string GetStrictTransportSecurityValue(SecurityHeaderSettings headerSettings)
    {
        return headerSettings.IsStrictTransportSecuritySubDomainsEnabled ?
            $"max-age={headerSettings.StrictTransportSecurityMaxAge}; includeSubDomains" :
            $"max-age={headerSettings.StrictTransportSecurityMaxAge}";
    }

    private void SetNonceValue(Dictionary<string, string> headers)
    {
        var nonceValue = _nonceProvider.GetCspValue();

        SetNonceValue(headers, CspConstants.HeaderNames.ContentSecurityPolicy, nonceValue);
        SetNonceValue(headers, CspConstants.HeaderNames.ReportOnlyContentSecurityPolicy, nonceValue);
    }

    private static void SetNonceValue(Dictionary<string, string> headers, string headerName, string? nonceValue)
    {
        if (!headers.ContainsKey(headerName) || headers[headerName] is null)
        {
            return;
        }

        headers[headerName] = headers[headerName].Replace(CspConstants.NoncePlaceholder, nonceValue);
    }

    private IEnumerable<string> GetReportingEndPoints(ICspSettings? settings)
    {
        if (settings is { IsEnabled: true, UseInternalReporting: true })
        {
            yield return $"stott-security-endpoint=\"{_cspReportUrlResolver.GetReportToPath()}\"";
        }

        if (settings is { IsEnabled: true, UseExternalReporting: true, ExternalReportToUrl.Length: >0 })
        {
            yield return $"stott-security-external-endpoint=\"{settings.ExternalReportToUrl}\"";
        }
    }
}