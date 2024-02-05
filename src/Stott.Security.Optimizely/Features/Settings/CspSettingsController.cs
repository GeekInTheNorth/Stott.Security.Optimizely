namespace Stott.Security.Optimizely.Features.Settings;

using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Common.Validation;
using Stott.Security.Optimizely.Features.Settings.Service;

[ApiExplorerSettings(IgnoreApi = true)]
[Authorize(Policy = CspConstants.AuthorizationPolicy)]
[Route("/stott.security.optimizely/api/[controller]/[action]")]
public sealed class CspSettingsController : BaseController
{
    private readonly ICspSettingsService _settings;

    private readonly ILogger<CspSettingsController> _logger;

    public CspSettingsController(
        ICspSettingsService service,
        ILogger<CspSettingsController> logger)
    {
        _settings = service;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        try
        {
            var data = await _settings.GetAsync();

            return CreateSuccessJson(new CspSettingsModel
            {
                IsEnabled = data.IsEnabled,
                IsReportOnly = data.IsReportOnly,
                IsAllowListEnabled = data.IsAllowListEnabled,
                AllowListUrl = data.AllowListUrl ?? string.Empty,
                IsUpgradeInsecureRequestsEnabled = data.IsUpgradeInsecureRequestsEnabled,
                IsNonceEnabled = data.IsNonceEnabled,
                IsStrictDynamicEnabled = data.IsStrictDynamicEnabled
            });
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "{LogPrefix} Failed to retrieve CSP settings.", CspConstants.LogPrefix);
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
            await _settings.SaveAsync(model, User.Identity?.Name);

            return Ok();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "{LogPrefix} Failed to save CSP settings.", CspConstants.LogPrefix);
            throw;
        }
    }
}