namespace Stott.Security.Optimizely.Features.Middleware;

using System;
using System.Threading.Tasks;

using EPiServer.Logging;
using EPiServer.Web.Routing;

using Microsoft.AspNetCore.Http;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Extensions;
using Stott.Security.Optimizely.Features.Header;

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
        IHeaderCompilationService securityHeaderService,
        IPageRouteHelper pageRouteHelper)
    {
        try
        {
            var pageData = pageRouteHelper.Page;
            var headers = await securityHeaderService.GetSecurityHeadersAsync(pageData);
            foreach (var header in headers)
            {
                context.Response.Headers.Append(header.Key, header.Value);
            }
        }
        catch (Exception exception)
        {
            _logger.Error($"{CspConstants.LogPrefix} Error encountered adding security headers.", exception);
        }

        await _next(context);
    }
}