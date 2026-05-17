namespace Stott.Security.Optimizely.Features.Header;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using EPiServer.ServiceLocation;

using Microsoft.AspNetCore.Http;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Features.Caching;
using Stott.Security.Optimizely.Features.Csp;
using Stott.Security.Optimizely.Features.Csp.Nonce;
using Stott.Security.Optimizely.Features.CustomHeaders.Service;
using Stott.Security.Optimizely.Features.PermissionPolicy.Service;
using Stott.Security.Optimizely.Features.Route;

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

    public async Task<List<HeaderDto>> GetSecurityHeadersAsync(SecurityRouteData routeData, HttpRequest? request)
    {
        var cacheKey = GetCacheKey(routeData);
        var headers = _cacheWrapper.Get<List<HeaderDto>>(cacheKey);
        if (headers == null)
        {
            headers = await CompileSecurityHeadersAsync(routeData);

            _cacheWrapper.Add(cacheKey, headers);
        }

        var isHttps = request?.IsHttps ?? false;

        // We do not want to mutate the headers in the cache as this will break functionality.
        return ModifyHeadersForRequest(headers, isHttps).ToList();
    }

    private static string GetCacheKey(SecurityRouteData routeData)
    {
        var contextSuffix = GetContextCacheKeySuffix(routeData);

        return routeData.RouteType switch
        {
            SecurityRouteType.NoNonceOrHash => $"{CspConstants.CacheKeys.CompiledHeadersNoHash}_{contextSuffix}",
            SecurityRouteType.ContentSpecificNoNonceOrHash => $"{CspConstants.CacheKeys.CompiledHeadersNoHash}_{routeData.Content?.ContentLink?.ID}_{contextSuffix}",
            SecurityRouteType.ContentSpecific => $"{CspConstants.CacheKeys.CompiledHeaders}_{routeData.Content?.ContentLink?.ID}_{contextSuffix}",
            _ => $"{CspConstants.CacheKeys.CompiledHeaders}_{contextSuffix}"
        };
    }

    private static string GetContextCacheKeySuffix(SecurityRouteData routeData)
    {
        var sitePart = routeData.SiteId.HasValue && routeData.SiteId.Value != Guid.Empty
            ? routeData.SiteId.Value.ToString("N")
            : "global";
        var hostPart = string.IsNullOrWhiteSpace(routeData.HostName) ? "all" : routeData.HostName.ToLowerInvariant();
        return $"{sitePart}_{hostPart}";
    }

    private static async Task<List<HeaderDto>> CompileSecurityHeadersAsync(SecurityRouteData routeData)
    {
        var securityHeaders = new List<HeaderDto>();

        var cspService = ServiceLocator.Current.GetInstance<ICspService>();
        var cspHeaders = await cspService.GetCompiledHeaders(routeData);
        if (cspHeaders is not null)
        {
            securityHeaders.AddRange(cspHeaders);
        }

        var permissionPolicyService = ServiceLocator.Current.GetInstance<IPermissionPolicyService>();
        var permissionPolicyHeaders = await permissionPolicyService.GetCompiledHeaders(routeData.SiteId, routeData.HostName);
        if (permissionPolicyHeaders is not null)
        {
            securityHeaders.AddRange(permissionPolicyHeaders);
        }

        var customHeaderService = ServiceLocator.Current.GetInstance<ICustomHeaderService>();
        var customHeaders = await customHeaderService.GetCompiledHeaders(routeData.SiteId, routeData.HostName);
        if (customHeaders is not null)
        {
            securityHeaders.AddRange(customHeaders);
        }

        return securityHeaders;
    }

    private IEnumerable<HeaderDto> ModifyHeadersForRequest(List<HeaderDto> headers, bool isHttps)
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

                yield return new HeaderDto { Key = header.Key, Value = newValue, IsRemoval = header.IsRemoval };
            }
            else if (header.Key == CspConstants.HeaderNames.ReportingEndpoints)
            {
                yield return new HeaderDto { Key = header.Key, Value = header.Value?.Replace(CspConstants.InternalReportingPlaceholder, _cspReportUrlResolver.GetReportToPath()), IsRemoval = header.IsRemoval };
            }
            else if (header.Key == CspConstants.HeaderNames.StrictTransportSecurity)
            {
                // HSTS should only be sent over HTTPS
                if (isHttps)
                {
                    yield return new HeaderDto { Key = header.Key, Value = header.Value, IsRemoval = header.IsRemoval };
                }
            }
            else
            {
                yield return new HeaderDto { Key = header.Key, Value = header.Value, IsRemoval = header.IsRemoval };
            }
        }
    }
}
