using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using EPiServer;
using EPiServer.Core;
using EPiServer.Shell.ViewComposition;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Stott.Security.Optimizely.Features.Header;
using Stott.Security.Optimizely.Features.Pages;
using Stott.Security.Optimizely.Features.StaticFile;

namespace Stott.Security.Optimizely.Features.Preview;

[Authorize]
[IFrameComponent(Url = "/stott.security.optimizely/widget/headers/preview/", Title = "Security", Categories = "content", PlugInAreas = "/episerver/cms/assets", MinHeight = 200, MaxHeight = 200)]
public sealed class HeaderPreviewWidgetController : Controller
{
    private readonly IHeaderCompilationService _securityHeaderService;

    private readonly IContentLoader _contentLoader;

    private readonly IStaticFileResolver _staticFileResolver;

    public HeaderPreviewWidgetController(
        IHeaderCompilationService securityHeaderService, 
        IContentLoader contentLoader, 
        IStaticFileResolver staticFileResolver)
    {
        _securityHeaderService = securityHeaderService;
        _contentLoader = contentLoader;
        _staticFileResolver = staticFileResolver;
    }

    [HttpGet]
    [Route("/stott.security.optimizely/widget/headers/preview")]
    public async Task<IActionResult> Index()
    {
        var assembly = Assembly.GetAssembly(typeof(HeaderPreviewWidgetController));
        var assemblyName = assembly?.GetName();

        var pageData = GetPageData(Request);
        var model = new HeaderPreviewWidgetViewModel
        {
            PageName = pageData?.Name ?? "Global",
            CanExtendTheContentSecurityPolicy = pageData is IContentSecurityPolicyPage,
            ExtendsTheContentSecurityPolicy = pageData is IContentSecurityPolicyPage { ContentSecurityPolicySources.Count: > 0 },
            SecurityHeaders = await GetHeaders(pageData),
            Version = $"v{assemblyName?.Version}",
            JavaScriptPath = $"/stott.security.optimizely/static/{_staticFileResolver.GetJavaScriptFileName()}",
            CssPath = $"/stott.security.optimizely/static/{_staticFileResolver.GetStyleSheetFileName()}"
        };

        return View(model);
    }

    private PageData? GetPageData(HttpRequest request)
    {
        var contentReferenceValue = request.Query["Id"].ToString() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(contentReferenceValue))
        {
            return null;
        }

        var contentReference = new ContentReference(contentReferenceValue);
        if (_contentLoader.TryGet<PageData>(contentReference, out var pageData))
        {
            return pageData;
        }

        return null;
    }

    private async Task<List<KeyValuePair<string, string>>> GetHeaders(PageData? pageData)
    {
        var headers = await _securityHeaderService.GetSecurityHeadersAsync(pageData);

        return headers.Where(x => !string.IsNullOrWhiteSpace(x.Value))
                      .OrderBy(x => x.Key)
                      .ToList();
    }
}

public class HeaderPreviewWidgetViewModel
{
    public string? PageName { get; set; }

    public bool CanExtendTheContentSecurityPolicy { get; set; }

    public bool ExtendsTheContentSecurityPolicy { get; set; }

    public List<KeyValuePair<string, string>>? SecurityHeaders { get; set; }
    public string Version { get; internal set; }
    public string JavaScriptPath { get; internal set; }
    public string CssPath { get; internal set; }
}