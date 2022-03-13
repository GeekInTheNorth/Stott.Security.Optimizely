using System;
using System.Threading.Tasks;

using EPiServer.Core;
using EPiServer.Logging;
using EPiServer.Web.Templating;

using Microsoft.AspNetCore.Http;

using Stott.Optimizely.Csp.Common;

namespace Stott.Optimizely.Csp.Features.Header
{
    public class SecurityHeaderMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly ILogger _logger = LogManager.GetLogger(typeof(SecurityHeaderMiddleware));

        public SecurityHeaderMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, ISecurityHeaderService securityHeaderService)
        {
            try
            {
                if (IsContentPage(context))
                {
                    var headers = securityHeaderService.GetSecurityHeaders();
                    foreach(var header in headers)
                    {
                        context.Response.Headers.Add(header.Key, header.Value);
                    }
                }
            }
            catch(Exception exception)
            {
                _logger.Error($"{CspConstants.LogPrefix} Error encountered adding security headers.", exception);
            }

            await _next(context);
        }

        private static bool IsContentPage(HttpContext context)
        {
            var renderingContext = context.Items["Epi:ContentRenderingContext"] as ContentRenderingContext;
            
            return renderingContext?.Content is PageData;
        }
    }
}
