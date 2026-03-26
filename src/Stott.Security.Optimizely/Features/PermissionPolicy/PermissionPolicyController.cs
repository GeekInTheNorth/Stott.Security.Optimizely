using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Common.Validation;
using Stott.Security.Optimizely.Extensions;
using Stott.Security.Optimizely.Features.PermissionPolicy.Models;
using Stott.Security.Optimizely.Features.PermissionPolicy.Service;

namespace Stott.Security.Optimizely.Features.PermissionPolicy;

[ApiExplorerSettings(IgnoreApi = true)]
[Authorize(Policy = CspConstants.AuthorizationPolicy)]
public sealed class PermissionPolicyController(IPermissionPolicyService permissionPolicyService, ILogger<PermissionPolicyController> logger) : BaseController
{
    [HttpGet]
    [Route("/stott.security.optimizely/api/permission-policy/source/list")]
    public async Task<IActionResult> List(string? sourceFilter, PermissionPolicyEnabledFilter enabledFilter, string? appId, string? hostName)
    {
        var sanitizedHost = hostName.GetSanitizedHostDomain();
        var allItems = await permissionPolicyService.ListDirectivesAsync(appId, sanitizedHost, sourceFilter, enabledFilter);

        return CreateSuccessJson(allItems);
    }

    [HttpPost]
    [Route("/stott.security.optimizely/api/permission-policy/source/save")]
    public async Task<IActionResult> Save([FromBody]SavePermissionPolicyModel model)
    {
        if (!ModelState.IsValid)
        {
            var validationModel = new ValidationModel(ModelState);
            return CreateValidationErrorJson(validationModel);
        }

        try
        {
            var sanitizedHost = model.HostName.GetSanitizedHostDomain();
            await permissionPolicyService.SaveDirectiveAsync(model, User.Identity?.Name, model.AppId, sanitizedHost);

            return Ok();
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "{LogPrefix} Failed to save Permission Policy changes.", CspConstants.LogPrefix);
            throw;
        }
    }

    [HttpPost]
    [Route("/stott.security.optimizely/api/permission-policy/override/create")]
    public async Task<IActionResult> CreateOverride(string? appId, string? hostName)
    {
        if (string.IsNullOrWhiteSpace(appId))
        {
            var validationModel = new ValidationModel(nameof(appId), "Cannot create an override for global context.");
            return CreateValidationErrorJson(validationModel);
        }

        try
        {
            var sanitizedHost = hostName.GetSanitizedHostDomain();
            await permissionPolicyService.CreateOverrideAsync(appId, sanitizedHost, User.Identity?.Name);

            return Ok();
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "{LogPrefix} Failed to create Permission Policy override.", CspConstants.LogPrefix);
            throw;
        }
    }

    [HttpDelete]
    [Route("/stott.security.optimizely/api/permission-policy/override/delete")]
    public async Task<IActionResult> DeleteOverride(string? appId, string? hostName)
    {
        if (string.IsNullOrWhiteSpace(appId))
        {
            var validationModel = new ValidationModel(nameof(appId), "Cannot delete global directives.");
            return CreateValidationErrorJson(validationModel);
        }

        try
        {
            var sanitizedHost = hostName.GetSanitizedHostDomain();
            await permissionPolicyService.DeleteByContextAsync(appId, sanitizedHost, User.Identity?.Name);

            return Ok();
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "{LogPrefix} Failed to delete Permission Policy directives for context.", CspConstants.LogPrefix);
            throw;
        }
    }

    [HttpGet]
    [Route("/stott.security.optimizely/api/permission-policy/settings/get")]
    public async Task<IActionResult> GetSettings(string? appId, string? hostName)
    {
        var sanitizedHost = hostName.GetSanitizedHostDomain();
        var existsForContext = await permissionPolicyService.ExistsForContextAsync(appId, sanitizedHost);
        var settings = await permissionPolicyService.GetPermissionPolicySettingsAsync(appId, sanitizedHost);

        return CreateSuccessJson(new { isEnabled = settings.IsEnabled, isInherited = !existsForContext });
    }

    [HttpPost]
    [Route("/stott.security.optimizely/api/permission-policy/settings/save")]
    public async Task<IActionResult> SaveSettings([FromBody]PermissionPolicySettingsModel model)
    {
        if (!ModelState.IsValid)
        {
            var validationModel = new ValidationModel(ModelState);
            return CreateValidationErrorJson(validationModel);
        }

        try
        {
            var sanitizedHost = model.HostName.GetSanitizedHostDomain();
            await permissionPolicyService.SaveSettingsAsync(model, User.Identity?.Name, model.AppId, sanitizedHost);

            return Ok();
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "{LogPrefix} Failed to save Permission Policy changes.", CspConstants.LogPrefix);
            throw;
        }
    }
}
