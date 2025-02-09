namespace Stott.Security.Optimizely.Features.Header;

using System.Collections.Generic;
using System.Threading.Tasks;

using EPiServer.Core;
using EPiServer.ServiceLocation;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Features.Caching;
using Stott.Security.Optimizely.Features.Csp;
using Stott.Security.Optimizely.Features.Csp.Nonce;
using Stott.Security.Optimizely.Features.Pages;
using Stott.Security.Optimizely.Features.PermissionPolicy.Service;
using Stott.Security.Optimizely.Features.SecurityHeaders.Service;

internal sealed class HeaderCompilationService : IHeaderCompilationService
{
    private readonly ICspReportUrlResolver _cspReportUrlResolver;

    private readonly INonceProvider _nonceProvider;

    private readonly ICacheWrapper _cacheWrapper;

    public HeaderCompilationService(
        ICspReportUrlResolver cspReportUrlResolver,
        INonceProvider nonceProvider,
        ICacheWrapper cacheWrapper)
    {
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

        var cspService = ServiceLocator.Current.GetInstance<ICspService>();
        var cspHeaders = await cspService.GetCompiledHeaders(cspPage);
        if (cspHeaders is not null)
        {
            foreach (var header in cspHeaders)
            {
                securityHeaders.Add(header.Key, header.Value);
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

    private void SetNonceValue(Dictionary<string, string> headers)
    {
        var nonceValue = _nonceProvider.GetCspValue();

        SetNonceValue(headers, CspConstants.HeaderNames.ContentSecurityPolicy, nonceValue);
        SetNonceValue(headers, CspConstants.HeaderNames.ReportOnlyContentSecurityPolicy, nonceValue);
    }

    private static void SetNonceValue(Dictionary<string, string> headers, string headerName, string? nonceValue)
    {
        if (headers.TryGetValue(headerName, out var headerValue))
        {
            headers[headerName] = headerValue.Replace(CspConstants.NoncePlaceholder, nonceValue);
        }
    }
}