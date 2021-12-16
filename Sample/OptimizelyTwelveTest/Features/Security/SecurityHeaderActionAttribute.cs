namespace OptimizelyTwelveTest.Features.Security
{
    using Microsoft.AspNetCore.Mvc.Filters;

    public class SecurityHeaderActionAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            base.OnResultExecuting(context);

            context.HttpContext.Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");
            context.HttpContext.Response.Headers.Add("X-Xss-Protection", "1; mode=block");
            context.HttpContext.Response.Headers.Add("X-Content-Type-Options", "nosniff");
            context.HttpContext.Response.Headers.Add("Referrer-Policy", "no-referrer");

            context.HttpContext.Response.Headers.Add("Content-Security-Policy", "default-src 'self'; style-src 'self' 'unsafe-inline' https://cdn.jsdelivr.net; script-src 'self' 'unsafe-inline' https://cdn.jsdelivr.net; img-src 'self' data: https:; frame-src 'self' https://www.youtube-nocookie.com/;");
        }
    }
}