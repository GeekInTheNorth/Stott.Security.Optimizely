namespace Stott.Security.Optimizely.Features.Cors;

using System.Collections.Generic;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Stott.Security.Optimizely.Common;

[ApiExplorerSettings(IgnoreApi = true)]
[Authorize(Policy = CspConstants.AuthorizationPolicy)]
public sealed class CorsConfigurationController : BaseController
{
    [HttpGet]
    [Route("/stott.security.optimizely/api/[controller]/[action]")]
    public IActionResult Get()
    {
        return CreateSuccessJson(new CorsConfiguration
        {
            IsEnabled = true,
            AllowMethods = new() { IsAllowGetMethods = true },
            AllowOrigins = new List<CorsConfigurationItem>
            {
                new() { Value = "https://www.example.com" },
                new() { Value = "https://www.test.com" }
            },
            AllowHeaders = new List<CorsConfigurationItem>
            {
                new() { Value = "allow-header-one" },
                new() { Value = "allow_header_two" }
            },
            ExposeHeaders = new List<CorsConfigurationItem>
            {
                new() { Value = "expose-header-one" },
                new() { Value = "expose_header_two" }
            }
        });
    }

    [HttpPost]
    [Route("/stott.security.optimizely/api/[controller]/[action]")]
    public IActionResult Save([FromBody]CorsConfiguration configuration)
    {
        return Ok();
    }
}