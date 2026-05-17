using System;
using System.Security.Policy;
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
public sealed class PermissionPolicyController : BaseController
{
    private readonly IPermissionPolicyService _permissionPolicyService;

    private readonly ILogger<PermissionPolicyController> _logger;

    public PermissionPolicyController(IPermissionPolicyService permissionPolicyService, ILogger<PermissionPolicyController> logger)
    {
        _permissionPolicyService = permissionPolicyService;
        _logger = logger;
    }

    [HttpGet]
    [Route("/stott.security.optimizely/api/permission-policy/source/list")]
    public async Task<IActionResult> List(string? sourceFilter, PermissionPolicyEnabledFilter enabledFilter, Guid? siteId, string? hostName)
    {
        var sanitizedSiteId = siteId.GetSanitizedSiteId();
        var sanitizedHost = hostName.GetSanitizedHostDomain();
        var allItems = await _permissionPolicyService.ListDirectivesAsync(sanitizedSiteId, sanitizedHost, sourceFilter, enabledFilter);

        return CreateSuccessJson(allItems);
    }

    [HttpPost]
    [Route("/stott.security.optimizely/api/permission-policy/source/save")]
    public async Task<IActionResult> Save([FromBody] SavePermissionPolicyModel model)
    {
        if (!ModelState.IsValid)
        {
            var validationModel = new ValidationModel(ModelState);
            return CreateValidationErrorJson(validationModel);
        }

        try
        {
            var sanitizedSiteId = model.SiteId.GetSanitizedSiteId();
            var sanitizedHost = model.HostName.GetSanitizedHostDomain();
            await _permissionPolicyService.SaveDirectiveAsync(model, User.Identity?.Name, sanitizedSiteId, sanitizedHost);

            return Ok();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "{LogPrefix} Failed to save Permission Policy changes.", CspConstants.LogPrefix);
            throw;
        }
    }

    [HttpPost]
    [Route("/stott.security.optimizely/api/permission-policy/override/create")]
    public async Task<IActionResult> CreateOverride(Guid? siteId, string? hostName)
    {
        var sanitizedSiteId = siteId.GetSanitizedSiteId();
        var sanitizedHost = hostName.GetSanitizedHostDomain();
        if (!sanitizedSiteId.HasValue)
        {
            var validationModel = new ValidationModel(nameof(siteId), "Cannot create an override for global context.");
            return CreateValidationErrorJson(validationModel);
        }

        try
        {
            await _permissionPolicyService.CreateOverrideAsync(sanitizedSiteId, sanitizedHost, User.Identity?.Name);

            return Ok();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "{LogPrefix} Failed to create Permission Policy override.", CspConstants.LogPrefix);
            throw;
        }
    }

    [HttpDelete]
    [Route("/stott.security.optimizely/api/permission-policy/override/delete")]
    public async Task<IActionResult> DeleteOverride(Guid? siteId, string? hostName)
    {
        var sanitizedSiteId = siteId.GetSanitizedSiteId();
        var sanitizedHost = hostName.GetSanitizedHostDomain();
        if (!sanitizedSiteId.HasValue)
        {
            var validationModel = new ValidationModel(nameof(siteId), "Cannot delete global directives.");
            return CreateValidationErrorJson(validationModel);
        }

        try
        {
            await _permissionPolicyService.DeleteByContextAsync(sanitizedSiteId, sanitizedHost, User.Identity?.Name);

            return Ok();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "{LogPrefix} Failed to delete Permission Policy directives for context.", CspConstants.LogPrefix);
            throw;
        }
    }

    [HttpGet]
    [Route("/stott.security.optimizely/api/permission-policy/settings/get")]
    public async Task<IActionResult> GetSettings(Guid? siteId, string? hostName)
    {
        var sanitizedSiteId = siteId.GetSanitizedSiteId();
        var sanitizedHost = hostName.GetSanitizedHostDomain();
        var existsForContext = await _permissionPolicyService.ExistsForContextAsync(sanitizedSiteId, sanitizedHost);
        var settings = await _permissionPolicyService.GetPermissionPolicySettingsAsync(sanitizedSiteId, sanitizedHost);

        return CreateSuccessJson(new { isEnabled = settings?.IsEnabled ?? false, isInherited = !existsForContext });
    }

    [HttpPost]
    [Route("/stott.security.optimizely/api/permission-policy/settings/save")]
    public async Task<IActionResult> SaveSettings([FromBody] PermissionPolicySettingsModel model)
    {
        if (!ModelState.IsValid)
        {
            var validationModel = new ValidationModel(ModelState);
            return CreateValidationErrorJson(validationModel);
        }

        try
        {
            var sanitizedSiteId = model.SiteId.GetSanitizedSiteId();
            var sanitizedHost = model.HostName.GetSanitizedHostDomain();
            await _permissionPolicyService.SaveSettingsAsync(model, User.Identity?.Name, sanitizedSiteId, sanitizedHost);

            return Ok();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "{LogPrefix} Failed to save Permission Policy changes.", CspConstants.LogPrefix);
            throw;
        }
    }
}
