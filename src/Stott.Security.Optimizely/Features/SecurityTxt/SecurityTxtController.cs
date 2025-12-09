using System;

using EPiServer.Web;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Stott.Security.Optimizely.Extensions;

namespace Stott.Security.Optimizely.Features.SecurityTxt;

public sealed class SecurityTxtController : Controller
{
    private readonly ISecurityTxtContentService _service;

    private readonly ILogger<SecurityTxtController> _logger;

    public SecurityTxtController(ISecurityTxtContentService service, ILogger<SecurityTxtController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet]
    [Route("/.well-known/security.txt")]
    [AllowAnonymous]
    public IActionResult Index()
    {
        try
        {
            var content = _service.GetSecurityTxtContent(SiteDefinition.Current.Id, Request?.Host.Value);

            if (string.IsNullOrWhiteSpace(content))
            {
                _logger.LogWarning("The security.txt content is empty for the current site.");
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
            _logger.LogError(exception, "Failed to load the security.txt for the current site.");
            throw;
        }
    }
}