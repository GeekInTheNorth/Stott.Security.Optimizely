namespace Stott.Security.Optimizely.Features.Preview;

using System;
using System.Linq;
using System.Threading.Tasks;

using EPiServer;
using EPiServer.Core;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Features.Header;

public sealed class CompiledHeaderController : BaseController
{
    private readonly IHeaderCompilationService _securityHeaderService;

    private readonly IContentLoader _contentLoader;

    public CompiledHeaderController(
        IHeaderCompilationService securityHeaderService,
        IContentLoader contentLoader)
    {
        _securityHeaderService = securityHeaderService;
        _contentLoader = contentLoader;
    }

    [AllowAnonymous]
    [Route("/stott.security.optimizely/api/compiled-headers/list")]
    [HttpGet]
    public async Task<IActionResult> ListAsync([FromQuery]int? pageId = null)
    {
        var pageData = GetPageData(pageId);
        var headers = await _securityHeaderService.GetSecurityHeadersAsync(pageData);

        var sortedHeaders = headers.Where(x => !string.IsNullOrWhiteSpace(x.Value))
                                   .OrderBy(x => x.Name)
                                   .ToList();

        return CreateSuccessJson(sortedHeaders);
    }

    [AllowAnonymous]
    [Route("/stott.security.optimizely/api/compiled-headers/{headerName}")]
    [HttpGet]
    public async Task<IActionResult> ListAsync(string headerName, [FromQuery] int? pageId = null)
    {
        var pageData = GetPageData(pageId);
        var headers = await _securityHeaderService.GetSecurityHeadersAsync(pageData);
        var headerValue = headers.Where(x => string.Equals(x.Name, headerName, StringComparison.OrdinalIgnoreCase))
                                 .Select(x => x.Value)
                                 .FirstOrDefault();

        return Content(headerValue ?? string.Empty);
    }

    private PageData? GetPageData(int? pageId)
    {
        if (pageId is null or < 1)
        {
            return null;
        }

        var contentReference = new ContentReference(pageId.Value);
        if (_contentLoader.TryGet<PageData>(contentReference, out var pageData))
        {
            return pageData;
        }

        return null;
    }
}