namespace Stott.Security.Optimizely.Features.LandingPage;

using System.Reflection;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Extensions;
using Stott.Security.Optimizely.Features.StaticFile;

[ApiExplorerSettings(IgnoreApi = true)]
[Authorize(Policy = CspConstants.AuthorizationPolicy)]
public sealed class SettingsLandingPageController : Controller
{
    private readonly IStaticFileResolver _staticFileResolver;

    public SettingsLandingPageController(IStaticFileResolver staticFileProvider)
    {
        _staticFileResolver = staticFileProvider;
    }

    [HttpGet]
    [Route("/stott.security.optimizely/administration/")]
    public IActionResult Index()
    {
        return View("~/Views/StottSecurity/SettingsLandingPage/Index.cshtml", GetModel());
    }

    [HttpGet]
    [Route("/stott.security.optimizely/static/{staticFileName}")]
    [AllowAnonymous]
    public IActionResult ApplicationStaticFile(string staticFileName)
    {
        var fileBytes = _staticFileResolver.GetFileContent(staticFileName);
        var mimeType = _staticFileResolver.GetFileMimeType(staticFileName);

        if (fileBytes.Length == 0)
        {
            return NotFound("The requested file does not exist.");
        }

        Response.Headers.AddOrUpdateHeader(HeaderNames.CacheControl, "public, max-age=31557600");

        return File(fileBytes, mimeType);
    }

    private SettingsLandingPageViewModel GetModel()
    {
        var assembly = Assembly.GetAssembly(typeof(SettingsLandingPageViewModel));
        var assemblyName = assembly?.GetName();

        return new SettingsLandingPageViewModel
        {
            Title = "Stott Security",
            Version = $"v{assemblyName?.Version}",
            JavaScriptPath = $"/stott.security.optimizely/static/{_staticFileResolver.GetJavaScriptFileName()}",
            CssPath = $"/stott.security.optimizely/static/{_staticFileResolver.GetStyleSheetFileName()}"
        };
    }
}