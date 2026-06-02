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
            var routeData = await securityRouteHelper.GetRouteDataAsync();
            var headers = await securityHeaderService.GetSecurityHeadersAsync(routeData, context.Request);
            foreach (var header in headers)
            {
                if (header.IsRemoval)
                {
                    HandleRemoval(context, header);
                }
                else if (header.IsReplacement)
                {
                    HandleReplacement(context, header);
                }
                else 
                {
                    HandleAppend(context, header);
                }
            }
        }
        catch (Exception exception)
        {
            _logger.Error($"{CspConstants.LogPrefix} Error encountered adding security headers.", exception);
        }

        await _next(context);
    }

    private static void HandleAppend(HttpContext context, HeaderDto header)
    {
        if (string.IsNullOrWhiteSpace(header?.Key) || string.IsNullOrWhiteSpace(header?.Value))
        {
            return;
        }

        context.Response.Headers.Append(header.Key, header.Value);
    }

    private static void HandleReplacement(HttpContext context, HeaderDto header)
    {
        if (string.IsNullOrWhiteSpace(header?.Key) || string.IsNullOrWhiteSpace(header?.Value))
        { 
            return;
        }

        if (context.Response.Headers.ContainsKey(header.Key))
        {
            context.Response.Headers[header.Key] = header.Value;
        }
        else
        {   
            context.Response.Headers.Append(header.Key, header.Value);
        }
    }

    private static void HandleRemoval(HttpContext context, HeaderDto header)
    {
        if (string.IsNullOrWhiteSpace(header?.Key))
        {
            return;
        }

        context.Response.Headers.Remove(header.Key!);
    }
}
