namespace Stott.Security.Optimizely.Features.Csp.Settings;

using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Common.Validation;
using Stott.Security.Optimizely.Features.Csp.Settings.Service;

[ApiExplorerSettings(IgnoreApi = true)]
[Authorize(Policy = CspConstants.AuthorizationPolicy)]
[Route("/stott.security.optimizely/api/[controller]/[action]")]
public sealed class CspSettingsController(
    ICspSettingsService service,
    ILogger<CspSettingsController> logger) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> Get(string? appId, string? hostName)
    {
        try
        {
            var contextData = await service.GetByContextAsync(appId, hostName);
            var isInherited = contextData == null && (!string.IsNullOrWhiteSpace(appId) || !string.IsNullOrWhiteSpace(hostName));
            var data = contextData ?? await service.GetAsync(appId, hostName);

            return CreateSuccessJson(new CspSettingsResponseModel
            {
                IsEnabled = data.IsEnabled,
                IsReportOnly = data.IsReportOnly,
                IsAllowListEnabled = data.IsAllowListEnabled,
                AllowListUrl = data.AllowListUrl ?? string.Empty,
                IsUpgradeInsecureRequestsEnabled = data.IsUpgradeInsecureRequestsEnabled,
                UseInternalReporting = data.UseInternalReporting,
                UseExternalReporting = data.UseExternalReporting,
                ExternalReportToUrl = data.ExternalReportToUrl ?? string.Empty,
                IsInherited = isInherited
            });
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "{LogPrefix} Failed to retrieve CSP settings.", CspConstants.LogPrefix);
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> Save(CspSettingsModel model)
    {
        if (!ModelState.IsValid)
        {
            var validationModel = new ValidationModel(ModelState);
            return CreateValidationErrorJson(validationModel);
        }

        try
        {
            await service.SaveAsync(model, User.Identity?.Name, model.AppId, model.HostName);

            return Ok();
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "{LogPrefix} Failed to save CSP settings.", CspConstants.LogPrefix);
            throw;
        }
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(string? appId, string? hostName)
    {
        if (string.IsNullOrWhiteSpace(appId))
        {
            var validationModel = new ValidationModel(nameof(appId), "Cannot delete global settings.");
            return CreateValidationErrorJson(validationModel);
        }

        try
        {
            await service.DeleteByContextAsync(appId, hostName, User.Identity?.Name);

            return Ok();
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "{LogPrefix} Failed to delete CSP settings for context.", CspConstants.LogPrefix);
            throw;
        }
    }
}
