namespace Stott.Security.Optimizely.Features.Header;

using System.Collections.Generic;
using System.Linq;
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

    public async Task<List<HeaderDto>> GetSecurityHeadersAsync(PageData? pageData)
    {
        var host = _cspReportUrlResolver.GetHost();
        var cacheKey = GetCacheKey(pageData, host);
        var headers = _cacheWrapper.Get<List<HeaderDto>>(cacheKey);
        if (headers == null)
        {
            headers = await CompileSecurityHeadersAsync(pageData as IContentSecurityPolicyPage);

            _cacheWrapper.Add(cacheKey, headers);
        }

        // We do not want to mutate the headers in the cache as this will break functionality.
        return CloneAndUpdatePlaceholders(headers).ToList();
    }

    private static string GetCacheKey(PageData? pageData, string host)
    {
        var shouldCacheForPage = pageData is IContentSecurityPolicyPage { ContentSecurityPolicySources.Count: > 0 };

        return shouldCacheForPage ? $"{CspConstants.CacheKeys.CompiledHeaders}_{host}_{pageData?.ContentLink}_{pageData?.Changed.Ticks}" : CspConstants.CacheKeys.CompiledHeaders;
    }

    private static async Task<List<HeaderDto>> CompileSecurityHeadersAsync(IContentSecurityPolicyPage? cspPage)
    {
        var securityHeaders = new List<HeaderDto>();

        var cspService = ServiceLocator.Current.GetInstance<ICspService>();
        var cspHeaders = await cspService.GetCompiledHeaders(cspPage);
        if (cspHeaders is not null)
        {
            securityHeaders.AddRange(cspHeaders);
        }

        var securityHeaderService = ServiceLocator.Current.GetInstance<ISecurityHeaderService>();
        var responseHeaders = await securityHeaderService.GetCompiledHeaders();
        if (responseHeaders is not null)
        {
            securityHeaders.AddRange(responseHeaders);
        }

        var permissionPolicyService = ServiceLocator.Current.GetInstance<IPermissionPolicyService>();
        var permissionPolicyHeaders = await permissionPolicyService.GetCompiledHeaders();
        if (permissionPolicyHeaders is not null)
        {
            securityHeaders.AddRange(permissionPolicyHeaders);
        }

        return securityHeaders;
    }

    private IEnumerable<HeaderDto> CloneAndUpdatePlaceholders(List<HeaderDto> headers)
    {
        var nonceValue = _nonceProvider.GetCspValue();
        var removeNonce = string.IsNullOrWhiteSpace(nonceValue);
        foreach (var header in headers)
        {
            if (header.Key == CspConstants.HeaderNames.ContentSecurityPolicy ||
                header.Key == CspConstants.HeaderNames.ReportOnlyContentSecurityPolicy)
            {
                var newValue = header.Value ?? string.Empty;
                if (removeNonce)
                {
                    newValue = newValue.Replace(CspConstants.Sources.Nonce, string.Empty)
                                       .Replace(CspConstants.Sources.StrictDynamic, string.Empty);
                }
                else
                {
                    newValue = newValue.Replace(CspConstants.Sources.Nonce, nonceValue);
                }

                yield return new HeaderDto { Key = header.Key, Value = newValue };
            }
            else if (header.Key == CspConstants.HeaderNames.ReportingEndpoints)
            {
                yield return new HeaderDto { Key = header.Key, Value = header.Value?.Replace(CspConstants.InternalReportingPlaceholder, _cspReportUrlResolver.GetReportToPath()) };
            }
            else
            {
                yield return new HeaderDto { Key = header.Key, Value = header.Value };
            }
        }
    }
}