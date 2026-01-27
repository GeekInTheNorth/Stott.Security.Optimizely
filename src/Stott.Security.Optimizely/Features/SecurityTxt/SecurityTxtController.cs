using System;
using System.Threading.Tasks;
using EPiServer.Applications;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Stott.Security.Optimizely.Extensions;
using Stott.Security.Optimizely.Features.SecurityTxt.Service;

namespace Stott.Security.Optimizely.Features.SecurityTxt;

public sealed class SecurityTxtController(
    IApplicationResolver applicationResolver, 
    ISecurityTxtContentService service, 
    ILogger<SecurityTxtController> logger) : Controller
{
    [HttpGet]
    [Route("/.well-known/security.txt")]
    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        try
        {
            var application = await applicationResolver.GetByContextAsync();
            var content = service.GetSecurityTxtContent(application?.Name, Request?.Host.Value);

            if (string.IsNullOrWhiteSpace(content))
            {
                logger.LogWarning("The security.txt content is empty for the current site.");
                return NotFound();
            }

            // Set a low cache duration, but not zero to ensure the CDN protects against DDOS attacks
            Response.Headers.AddOrUpdateHeader("Cache-Control", "public, max-age=300");

            return new ContentResult
            {
                Content = content,
                ContentType = "text/plain",
                StatusCode = 200
            };
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to load the security.txt for the current site.");
            throw;
        }
    }
}