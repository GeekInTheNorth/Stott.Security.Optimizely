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
public sealed class CspSandboxController(
    ICspSandboxService service,
    ILogger<CspSandboxController> logger) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> Get(string? appId, string? hostName)
    {
        try
        {
            var sanitizedHostName = hostName.GetSanitizedHostDomain();
            var existsForContext = await service.ExistsForContextAsync(appId, sanitizedHostName);
            var contextData = await service.GetAsync(appId, sanitizedHostName);
            var data = CspSandboxMapper.MapToResponse(contextData, !existsForContext);

            return CreateSuccessJson(data);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, $"{CspConstants.LogPrefix} Failed to retrieve CSP sandbox settings.");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> Save(SandboxModel model)
    {
        try
        {
            await service.SaveAsync(model, User.Identity?.Name, model.AppId, model.HostName.GetSanitizedHostDomain());

            return Ok();
        }
        catch (Exception exception)
        {
            logger.LogError(exception, $"{CspConstants.LogPrefix} Failed to save CSP sandbox settings.");
            throw;
        }
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(string? appId, string? hostName)
    {
        if (string.IsNullOrWhiteSpace(appId))
        {
            var validationModel = new ValidationModel(nameof(appId), "Cannot delete global sandbox settings.");
            return CreateValidationErrorJson(validationModel);
        }

        try
        {
            await service.DeleteByContextAsync(appId, hostName.GetSanitizedHostDomain(), User.Identity?.Name);

            return Ok();
        }
        catch (Exception exception)
        {
            logger.LogError(exception, $"{CspConstants.LogPrefix} Failed to delete CSP sandbox settings for context.");
            throw;
        }
    }
}
