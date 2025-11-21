namespace Stott.Security.Optimizely.Features.Csp.Settings;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPiServer.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Common.Validation;
using Stott.Security.Optimizely.Features.Csp.Settings.Service;

[ApiExplorerSettings(IgnoreApi = true)]
[Authorize(Policy = CspConstants.AuthorizationPolicy)]
[Route("/stott.security.optimizely/api/[controller]/[action]")]
public sealed class CspSettingsController : BaseController
{
    private readonly ICspSettingsService _settings;

    private readonly ISiteDefinitionRepository _siteDefinitionRepository;

    private readonly ILogger<CspSettingsController> _logger;

    public CspSettingsController(
        ICspSettingsService service,
        ISiteDefinitionRepository siteDefinitionRepository,
        ILogger<CspSettingsController> logger)
    {
        _settings = service;
        _siteDefinitionRepository = siteDefinitionRepository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> List()
    {
        var siteDefinitions = _siteDefinitionRepository.List().ToList();
        var data = await _settings.GetAsync();
        var dataItems = new List<Entities.CspSettings> { data };

        var model = dataItems.Select(item => new CspSummaryModel(item, siteDefinitions)).ToList();

        return CreateSuccessJson(model);
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        try
        {
            var data = await _settings.GetAsync();

            return CreateSuccessJson(new CspSettingsModel
            {
                Id = data.Id,
                Name = "Policy Name",
                ScopeSiteId = new Guid("a5744d47-f9fc-42a0-83fc-c41cc2eb643b"), // Example Guid, replace with actual logic to get the scope ID
                ScopeBehaviour = "All",
                ScopePaths = new string[] { "/" },
                ScopeExclusions = Array.Empty<string>(),
                IsEnabled = data.IsEnabled,
                IsReportOnly = data.IsReportOnly,
                IsAllowListEnabled = data.IsAllowListEnabled,
                AllowListUrl = data.AllowListUrl ?? string.Empty,
                IsUpgradeInsecureRequestsEnabled = data.IsUpgradeInsecureRequestsEnabled,
                IsNonceEnabled = data.IsNonceEnabled,
                IsStrictDynamicEnabled = data.IsStrictDynamicEnabled,
                UseInternalReporting = data.UseInternalReporting,
                UseExternalReporting = data.UseExternalReporting,
                ExternalReportToUrl = data.ExternalReportToUrl ?? string.Empty
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