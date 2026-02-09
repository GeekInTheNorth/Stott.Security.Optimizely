namespace Stott.Security.Optimizely.Features.Middleware;

using System;
using System.Threading.Tasks;

using EPiServer.Logging;

using Microsoft.AspNetCore.Http;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Features.Header;
using Stott.Security.Optimizely.Features.Route;

public sealed class SecurityHeaderMiddleware
{
    private readonly RequestDelegate _next;

    private readonly ILogger _logger = LogManager.GetLogger(typeof(SecurityHeaderMiddleware));

    public SecurityHeaderMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(
        HttpContext context,
        ISecurityRouteHelper securityRouteHelper,
        IHeaderCompilationService securityHeaderService)
    {
        try
        {
            // pageRouteHelper.Page is only populated for PageData routes
            // pageRouteHelper.Content is populated for PageData and Geta Category routes
            var routeData = securityRouteHelper.GetRouteData();
            var headers = await securityHeaderService.GetSecurityHeadersAsync(routeData, context.Request);
            foreach (var header in headers)
            {
                if (string.IsNullOrWhiteSpace(header.Key))
                {
                    continue;
                }

                if (header.IsRemoval)
                {
                    context.Response.Headers.Remove(header.Key);
                }
                else if (!string.IsNullOrWhiteSpace(header.Value))
                {
                    context.Response.Headers.Append(header.Key, header.Value);
                }
            }
        }
        catch (Exception exception)
        {
            _logger.Error($"{CspConstants.LogPrefix} Error encountered adding security headers.", exception);
        }

        await _next(context);
    }
}