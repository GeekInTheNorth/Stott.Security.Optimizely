namespace Stott.Security.Optimizely.Features.LandingPage;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Features.StaticFile;

[ApiExplorerSettings(IgnoreApi = true)]
[Authorize(Policy = CspConstants.AuthorizationPolicy)]
public class SettingsLandingPageController : Controller
{
    private readonly IStaticFileResolver _staticFileProvider;

    public SettingsLandingPageController(IStaticFileResolver staticFileProvider)
    {
        _staticFileProvider = staticFileProvider;
    }

    [HttpGet]
    [Route("/stott.security.optimizely/settings/content-security-policy")]
    public IActionResult ContentSecurityPolicy()
    {
        return View(GetModel());
    }

    [HttpGet]
    [Route("/stott.security.optimizely/settings/headers")]
    public IActionResult Headers()
    {
        return View(GetModel());
    }

    [HttpGet]
    [Route("/stott.security.optimizely/settings/audit-history")]
    public IActionResult AuditHistory()
    {
        return View(GetModel());
    }

    [HttpGet]
    [Route("/stott.security.optimizely/static/{staticFileName}")]
    public IActionResult ApplicationStaticFile(string staticFileName)
    {
        var fileBytes = _staticFileProvider.GetFileContent(staticFileName);
        var mimeType = _staticFileProvider.GetFileMimeType(staticFileName);

        if (fileBytes.Length == 0)
        {
            return NotFound("The requested file does not exist.");
        }

        Response.Headers.Add("cache-control", "public, max-age=31557600");

        return File(fileBytes, mimeType);
    }

    private SettingsLandingPageViewModel GetModel()
    {
        return new SettingsLandingPageViewModel
        {
            JavaScriptPath = $"/stott.security.optimizely/static/{_staticFileProvider.GetJavaScriptPath()}",
            CssPath = $"/stott.security.optimizely/static/{_staticFileProvider.GetStyleSheetPath()}"
        };
    }
}
