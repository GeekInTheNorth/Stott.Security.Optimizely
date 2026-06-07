namespace Stott.Security.Optimizely.Features.Guides;

using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Features.Guides.Service;

/// <summary>
/// API controller that proxies the remote Stott Security guides feed so that consuming
/// sites do not need to allow the remote domain within their Content Security Policy.
/// </summary>
[ApiExplorerSettings(IgnoreApi = true)]
[Authorize(Policy = CspConstants.AuthorizationPolicy)]
public sealed class GuideController(
    IGuideService service,
    ILogger<GuideController> logger) : BaseController
{
    /// <summary>
    /// Gets the list of published guides, newest first.
    /// </summary>
    [HttpGet]
    [Route("/stott.security.optimizely/api/guides/list")]
    public async Task<IActionResult> List()
    {
        try
        {
            var guides = await service.GetGuidesAsync();

            return CreateSuccessJson(guides);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "{LogPrefix} Failed to retrieve guides.", CspConstants.LogPrefix);
            throw;
        }
    }
}
