namespace Stott.Security.Optimizely.Features.Csp.Settings;

using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Common.Validation;
using Stott.Security.Optimizely.Extensions;
using Stott.Security.Optimizely.Features.Csp.Settings.Service;

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
    public async Task<IActionResult> Get(Guid? siteId, string? hostName)
    {
        try
        {
            var sanitizedHostName = hostName.GetSanitizedHostDomain();
            var existsForContext = await _settings.ExistsForContextAsync(siteId, sanitizedHostName);
            var data = await _settings.GetAsync(siteId, sanitizedHostName);

            return CreateSuccessJson(new CspSettingsResponseModel
            {
                IsEnabled = data.IsEnabled,
                IsReportOnly = data.IsReportOnly,
                IsAllowListEnabled = data.IsAllowListEnabled,
                AllowListUrl = data.AllowListUrl ?? string.Empty,
                IsUpgradeInsecureRequestsEnabled = data.IsUpgradeInsecureRequestsEnabled,
                IsNonceEnabled = data.IsNonceEnabled,
                IsStrictDynamicEnabled = data.IsStrictDynamicEnabled,
                UseInternalReporting = data.UseInternalReporting,
                UseExternalReporting = data.UseExternalReporting,
                ExternalReportToUrl = data.ExternalReportToUrl ?? string.Empty,
                IsInherited = !existsForContext
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
            await _settings.SaveAsync(model, User.Identity?.Name, model.SiteId, model.HostName.GetSanitizedHostDomain());

            return Ok();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "{LogPrefix} Failed to save CSP settings.", CspConstants.LogPrefix);
            throw;
        }
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(Guid? siteId, string? hostName)
    {
        if (!siteId.HasValue || siteId.Value == Guid.Empty)
        {
            var validationModel = new ValidationModel(nameof(siteId), "Cannot delete Global CSP settings.");
            return CreateValidationErrorJson(validationModel);
        }

        try
        {
            await _settings.DeleteByContextAsync(siteId, hostName.GetSanitizedHostDomain(), User.Identity?.Name);

            return Ok();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "{LogPrefix} Failed to delete CSP settings for context.", CspConstants.LogPrefix);
            throw;
        }
    }
}
