using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Common.Validation;
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
    public async Task<IActionResult> List(string? sourceFilter, PermissionPolicyEnabledFilter enabledFilter)
    {
        var allItems = await _permissionPolicyService.ListDirectivesAsync(sourceFilter, enabledFilter);

        return CreateSuccessJson(allItems);
    }

    [HttpPost]
    [Route("/stott.security.optimizely/api/permission-policy/source/save")]
    public async Task<IActionResult> Save(SavePermissionPolicyModel model)
    {
        if (!ModelState.IsValid)
        {
            var validationModel = new ValidationModel(ModelState);
            return CreateValidationErrorJson(validationModel);
        }

        try
        {
            await _permissionPolicyService.SaveDirectiveAsync(model, User.Identity?.Name);

            return Ok();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "{LogPrefix} Failed to save Permission Policy changes.", CspConstants.LogPrefix);
            throw;
        }
    }

    [HttpGet]
    [Route("/stott.security.optimizely/api/permission-policy/settings/get")]
    public async Task<IActionResult> GetSettings()
    {
        var settings = await _permissionPolicyService.GetPermissionPolicySettingsAsync();

        return CreateSuccessJson(settings);
    }

    [HttpPost]
    [Route("/stott.security.optimizely/api/permission-policy/settings/save")]
    public async Task<IActionResult> SaveSettings(PermissionPolicySettingsModel model)
    {
        if (!ModelState.IsValid)
        {
            var validationModel = new ValidationModel(ModelState);
            return CreateValidationErrorJson(validationModel);
        }

        try
        {
            await _permissionPolicyService.SaveSettingsAsync(model, User.Identity?.Name);

            return Ok();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "{LogPrefix} Failed to save Permission Policy changes.", CspConstants.LogPrefix);
            throw;
        }
    }
}