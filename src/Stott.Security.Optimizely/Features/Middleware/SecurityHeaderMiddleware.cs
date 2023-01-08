namespace Stott.Security.Optimizely.Features.Middleware;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

using EPiServer.Core;
using EPiServer.Logging;
using EPiServer.Web.Templating;

using Microsoft.AspNetCore.Http;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Features.Header;

public class SecurityHeaderMiddleware
{
    private readonly RequestDelegate _next;

    private readonly ILogger _logger = LogManager.GetLogger(typeof(SecurityHeaderMiddleware));

    public SecurityHeaderMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, IHeaderCompilationService securityHeaderService)
    {
        try
        {
            if (IsContentPage(context, out var pageData))
            {
                var headers = await securityHeaderService.GetSecurityHeadersAsync(pageData);
                foreach (var header in headers)
                {
                    context.Response.Headers.Add(header.Key, header.Value);
                }
            }
        }
        catch (Exception exception)
        {
            _logger.Error($"{CspConstants.LogPrefix} Error encountered adding security headers.", exception);
        }

        await _next(context);
    }

    private static bool IsContentPage(HttpContext context, [NotNullWhen(true)] out PageData? contentPage)
    {
        var renderingContext = context.Items["Epi:ContentRenderingContext"] as ContentRenderingContext;

        contentPage = renderingContext?.Content as PageData;

        return contentPage != null;
    }
}