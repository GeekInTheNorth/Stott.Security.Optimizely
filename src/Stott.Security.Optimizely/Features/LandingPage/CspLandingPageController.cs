namespace Stott.Security.Optimizely.Features.LandingPage;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Features.StaticFile;

[ApiExplorerSettings(IgnoreApi = true)]
[Authorize(Policy = CspConstants.AuthorizationPolicy)]
[Route("[controller]/[action]")]
public class CspLandingPageController : Controller
{
    private readonly IStaticFileResolver _staticFileProvider;

    public CspLandingPageController(IStaticFileResolver staticFileProvider)
    {
        _staticFileProvider = staticFileProvider;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(GetModel());
    }

    [HttpGet]
    public IActionResult Headers()
    {
        return View(GetModel());
    }

    [HttpGet]
    public IActionResult AuditHistory()
    {
        return View(GetModel());
    }

    [HttpGet]
    [Route("/stott.optimizely.csp/static/{staticFileName}")]
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

    private CspLandingPageViewModel GetModel()
    {
        return new CspLandingPageViewModel
        {
            JavaScriptPath = $"/stott.optimizely.csp/static/{_staticFileProvider.GetJavaScriptPath()}",
            CssPath = $"/stott.optimizely.csp/static/{_staticFileProvider.GetStyleSheetPath()}"
        };
    }
}
