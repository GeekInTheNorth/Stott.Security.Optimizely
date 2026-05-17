namespace Stott.Security.Optimizely.Features.Csp.Sandbox;

using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Common.Validation;
using Stott.Security.Optimizely.Extensions;
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
    public async Task<IActionResult> Get(Guid? siteId, string? hostName)
    {
        try
        {
            var sanitizedSiteId = siteId.GetSanitizedSiteId();
            var sanitizedHostName = hostName.GetSanitizedHostDomain();
            var existsForContext = await _service.ExistsForContextAsync(sanitizedSiteId, sanitizedHostName);
            var contextData = await _service.GetAsync(sanitizedSiteId, sanitizedHostName);
            var data = CspSandboxMapper.MapToResponse(contextData, !existsForContext);

            return CreateSuccessJson(data);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "{LogPrefix} Failed to retrieve CSP sandbox settings.", CspConstants.LogPrefix);
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
            _logger.LogError(exception, "{LogPrefix} Failed to save CSP sandbox settings.", CspConstants.LogPrefix);
            throw;
        }
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(Guid? siteId, string? hostName)
    {
        var sanitizedSiteId = siteId.GetSanitizedSiteId();
        var sanitizedHostName = hostName.GetSanitizedHostDomain();
        if (sanitizedSiteId is null)
        {
            var validationModel = new ValidationModel(nameof(siteId), "Cannot delete global sandbox settings.");
            return CreateValidationErrorJson(validationModel);
        }

        try
        {
            await _service.DeleteByContextAsync(sanitizedSiteId, sanitizedHostName, User.Identity?.Name);

            return Ok();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"{CspConstants.LogPrefix} Failed to delete CSP sandbox settings for context.");
            throw;
        }
    }
}
