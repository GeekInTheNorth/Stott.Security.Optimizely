namespace Stott.Security.Optimizely.Features.Preview;

using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Features.Header;
using Stott.Security.Optimizely.Features.Route;

public sealed class CompiledHeaderController : BaseController
{
    private readonly IHeaderCompilationService _securityHeaderService;

    private readonly ISecurityRouteHelper _routeHelper;

    public CompiledHeaderController(
        IHeaderCompilationService securityHeaderService,
        ISecurityRouteHelper routeHelper)
    {
        _securityHeaderService = securityHeaderService;
        _routeHelper = routeHelper;
    }

    /// <summary>
    /// Retrieves the value of a specific compiled security header by name.
    /// Where a querystring parameter "pageId" is provided will result in the headers being generated based on that page.
    /// </summary>
    /// <returns></returns>
    [AllowAnonymous]
    [Route("/stott.security.optimizely/api/compiled-headers/list")]
    [HttpGet]
    public async Task<IActionResult> ListAsync()
    {
        var routeData = _routeHelper.GetRouteData();
        var headers = await _securityHeaderService.GetSecurityHeadersAsync(routeData, Request);

        var sortedHeaders = headers.Where(x => !string.IsNullOrWhiteSpace(x.Value))
                                   .OrderBy(x => x.Key)
                                   .ToList();

        return CreateSuccessJson(sortedHeaders);
    }

    /// <summary>
    /// Retrieves the value of a specific compiled security header by name.
    /// Where a querystring parameter "pageId" is provided will result in the headers being generated based on that page.
    /// </summary>
    /// <param name="headerName">The name of the header to request.</param>
    /// <returns></returns>
    [AllowAnonymous]
    [Route("/stott.security.optimizely/api/compiled-headers/{headerName}")]
    [HttpGet]
    public async Task<IActionResult> ListAsync(string headerName)
    {
        var routeData = _routeHelper.GetRouteData();
        var headers = await _securityHeaderService.GetSecurityHeadersAsync(routeData, Request);
        var headerValue = headers.Where(x => string.Equals(x.Key, headerName, StringComparison.OrdinalIgnoreCase))
                                 .Select(x => x.Value)
                                 .FirstOrDefault();

        return Content(headerValue ?? string.Empty);
    }
}