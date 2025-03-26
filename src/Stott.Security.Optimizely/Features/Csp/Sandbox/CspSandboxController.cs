namespace Stott.Security.Optimizely.Features.Csp.Sandbox;

using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Features.Csp.Sandbox.Service;

[ApiExplorerSettings(IgnoreApi = true)]
[Authorize(Policy = CspConstants.AuthorizationPolicy)]
[Route("/stott.security.optimizely/api/[controller]/[action]")]
public sealed class CspSandboxController : BaseController
{
    private readonly ICspSandboxService _service;

    private readonly ILogger<CspSandboxController> _logger;

    public CspSandboxController(
        ICspSandboxService service,
        ILogger<CspSandboxController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        try
        {
            var model = await _service.GetAsync();

            return CreateSuccessJson(model);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"{CspConstants.LogPrefix} Failed to retrieve CSP sandbox settings.");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> Save(SandboxModel model)
    {
        try
        {
            await _service.SaveAsync(model, User.Identity?.Name);

            return Ok();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"{CspConstants.LogPrefix} Failed to save CSP sandbox settings.");
            throw;
        }
    }
}