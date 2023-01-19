namespace Stott.Security.Optimizely.Features.LandingPage;

using System.Reflection;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Features.StaticFile;

[ApiExplorerSettings(IgnoreApi = true)]
[Authorize(Policy = CspConstants.AuthorizationPolicy)]
public sealed class SettingsLandingPageController : Controller
{
    private readonly IStaticFileResolver _staticFileProvider;

    public SettingsLandingPageController(IStaticFileResolver staticFileProvider)
    {
        _staticFileProvider = staticFileProvider;
    }

    [HttpGet]
    [Route("/stott.security.optimizely/administration/")]
    public IActionResult Index()
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
        var assembly = Assembly.GetAssembly(typeof(SettingsLandingPageViewModel));
        var assemblyName = assembly?.GetName();

        return new SettingsLandingPageViewModel
        {
            Title = assemblyName?.Name ?? string.Empty,
            Version = $"v{assemblyName?.Version}",
            JavaScriptPath = $"/stott.security.optimizely/static/{_staticFileProvider.GetJavaScriptPath()}",
            CssPath = $"/stott.security.optimizely/static/{_staticFileProvider.GetStyleSheetPath()}"
        };
    }
}