namespace Stott.Security.Optimizely.Features.Header;

using System.Collections.Generic;
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

internal sealed class HeaderCompilationService(
    ICspReportUrlResolver cspReportUrlResolver,
    INonceProvider nonceProvider,
    ICacheWrapper cacheWrapper) : IHeaderCompilationService
{
    public async Task<List<HeaderDto>> GetSecurityHeadersAsync(SecurityRouteData routeData, HttpRequest? request)
    {
        var cacheKey = GetCacheKey(routeData);
        var headers = cacheWrapper.Get<List<HeaderDto>>(cacheKey);
        if (headers == null)
        {
            headers = await CompileSecurityHeadersAsync(routeData);

            cacheWrapper.Add(cacheKey, headers);
        }

        var isHttps = request?.IsHttps ?? false;

        // We do not want to mutate the headers in the cache as this will break functionality.
        return await ModifyHeadersForRequest(headers, isHttps);
    }

    private static string GetCacheKey(SecurityRouteData routeData)
    {
        var appSuffix = GetCacheKeySuffix(routeData);

        return routeData.RouteType switch
        {
            SecurityRouteType.NoNonceOrHash => $"{CspConstants.CacheKeys.CompiledHeadersNoHash}_{appSuffix}",
            SecurityRouteType.ContentSpecificNoNonceOrHash => $"{CspConstants.CacheKeys.CompiledHeadersNoHash}_{routeData.Content?.ContentLink?.ID}_{appSuffix}",
            SecurityRouteType.ContentSpecific => $"{CspConstants.CacheKeys.CompiledHeaders}_{routeData.Content?.ContentLink?.ID}_{appSuffix}",
            _ => $"{CspConstants.CacheKeys.CompiledHeaders}_{appSuffix}"
        };
    }

    private static string GetCacheKeySuffix(SecurityRouteData routeData)
    {
        return $"{routeData.AppId ?? "global"}_{routeData.HostName ?? "all"}";
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
        var permissionPolicyHeaders = await permissionPolicyService.GetCompiledHeaders();
        if (permissionPolicyHeaders is not null)
        {
            securityHeaders.AddRange(permissionPolicyHeaders);
        }

        var customHeaderService = ServiceLocator.Current.GetInstance<ICustomHeaderService>();
        var customHeaders = await customHeaderService.GetCompiledHeaders();
        if (customHeaders is not null)
        {
            securityHeaders.AddRange(customHeaders);
        }

        return securityHeaders;
    }

    private async Task<List<HeaderDto>> ModifyHeadersForRequest(List<HeaderDto> headers, bool isHttps)
    {
        var updatedHeaders = new List<HeaderDto>();
        var nonceValue = await nonceProvider.GetCspValueAsync();
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

                updatedHeaders.Add(new HeaderDto { Key = header.Key, Value = newValue, IsRemoval = header.IsRemoval });
            }
            else if (header.Key == CspConstants.HeaderNames.ReportingEndpoints)
            {
                updatedHeaders.Add(new HeaderDto { Key = header.Key, Value = header.Value?.Replace(CspConstants.InternalReportingPlaceholder, cspReportUrlResolver.GetReportToPath()), IsRemoval = header.IsRemoval });
            }
            else if (header.Key == CspConstants.HeaderNames.StrictTransportSecurity)
            {
                // HSTS should only be sent over HTTPS
                if (isHttps)
                {
                    updatedHeaders.Add(new HeaderDto { Key = header.Key, Value = header.Value, IsRemoval = header.IsRemoval });
                }
            }
            else
            {
                updatedHeaders.Add(new HeaderDto { Key = header.Key, Value = header.Value, IsRemoval = header.IsRemoval });
            }
        }

        return updatedHeaders;
    }
}
