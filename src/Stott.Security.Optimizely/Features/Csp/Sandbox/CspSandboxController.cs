namespace Stott.Security.Optimizely.Features.Csp.Sandbox;

using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Common.Validation;
using Stott.Security.Optimizely.Entities;
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
    public async Task<IActionResult> Get(string? appId, string? hostName)
    {
        try
        {
            var contextData = await _service.GetByContextAsync(appId, hostName);
            var isInherited = contextData == null && (!string.IsNullOrWhiteSpace(appId) || !string.IsNullOrWhiteSpace(hostName));
            var data = contextData != null
                ? MapToResponse(contextData, isInherited)
                : MapToResponse(await _service.GetAsync(appId, hostName), isInherited);

            return CreateSuccessJson(data);
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
            await _service.SaveAsync(model, User.Identity?.Name, model.AppId, model.HostName);

            return Ok();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"{CspConstants.LogPrefix} Failed to save CSP sandbox settings.");
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
            await _service.DeleteByContextAsync(appId, hostName, User.Identity?.Name);

            return Ok();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"{CspConstants.LogPrefix} Failed to delete CSP sandbox settings for context.");
            throw;
        }
    }

    private static SandboxResponseModel MapToResponse(CspSandbox sandbox, bool isInherited)
    {
        return new SandboxResponseModel
        {
            IsSandboxEnabled = sandbox.IsSandboxEnabled,
            IsAllowDownloadsEnabled = sandbox.IsAllowDownloadsEnabled,
            IsAllowDownloadsWithoutGestureEnabled = sandbox.IsAllowDownloadsWithoutGestureEnabled,
            IsAllowFormsEnabled = sandbox.IsAllowFormsEnabled,
            IsAllowModalsEnabled = sandbox.IsAllowModalsEnabled,
            IsAllowOrientationLockEnabled = sandbox.IsAllowOrientationLockEnabled,
            IsAllowPointerLockEnabled = sandbox.IsAllowPointerLockEnabled,
            IsAllowPopupsEnabled = sandbox.IsAllowPopupsEnabled,
            IsAllowPopupsToEscapeTheSandboxEnabled = sandbox.IsAllowPopupsToEscapeTheSandboxEnabled,
            IsAllowPresentationEnabled = sandbox.IsAllowPresentationEnabled,
            IsAllowSameOriginEnabled = sandbox.IsAllowSameOriginEnabled,
            IsAllowScriptsEnabled = sandbox.IsAllowScriptsEnabled,
            IsAllowStorageAccessByUserEnabled = sandbox.IsAllowStorageAccessByUserEnabled,
            IsAllowTopNavigationEnabled = sandbox.IsAllowTopNavigationEnabled,
            IsAllowTopNavigationByUserEnabled = sandbox.IsAllowTopNavigationByUserEnabled,
            IsAllowTopNavigationToCustomProtocolEnabled = sandbox.IsAllowTopNavigationToCustomProtocolEnabled,
            IsInherited = isInherited
        };
    }

    private static SandboxResponseModel MapToResponse(SandboxModel model, bool isInherited)
    {
        return new SandboxResponseModel
        {
            IsSandboxEnabled = model.IsSandboxEnabled,
            IsAllowDownloadsEnabled = model.IsAllowDownloadsEnabled,
            IsAllowDownloadsWithoutGestureEnabled = model.IsAllowDownloadsWithoutGestureEnabled,
            IsAllowFormsEnabled = model.IsAllowFormsEnabled,
            IsAllowModalsEnabled = model.IsAllowModalsEnabled,
            IsAllowOrientationLockEnabled = model.IsAllowOrientationLockEnabled,
            IsAllowPointerLockEnabled = model.IsAllowPointerLockEnabled,
            IsAllowPopupsEnabled = model.IsAllowPopupsEnabled,
            IsAllowPopupsToEscapeTheSandboxEnabled = model.IsAllowPopupsToEscapeTheSandboxEnabled,
            IsAllowPresentationEnabled = model.IsAllowPresentationEnabled,
            IsAllowSameOriginEnabled = model.IsAllowSameOriginEnabled,
            IsAllowScriptsEnabled = model.IsAllowScriptsEnabled,
            IsAllowStorageAccessByUserEnabled = model.IsAllowStorageAccessByUserEnabled,
            IsAllowTopNavigationEnabled = model.IsAllowTopNavigationEnabled,
            IsAllowTopNavigationByUserEnabled = model.IsAllowTopNavigationByUserEnabled,
            IsAllowTopNavigationToCustomProtocolEnabled = model.IsAllowTopNavigationToCustomProtocolEnabled,
            IsInherited = isInherited
        };
    }
}
