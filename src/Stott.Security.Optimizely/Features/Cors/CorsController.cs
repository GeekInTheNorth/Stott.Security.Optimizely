namespace Stott.Security.Optimizely.Features.Cors;

using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Common.Validation;
using Stott.Security.Optimizely.Features.Cors.Repository;

[ApiExplorerSettings(IgnoreApi = true)]
[Authorize(Policy = CspConstants.AuthorizationPolicy)]
public sealed class CorsConfigurationController : BaseController
{
    private readonly ICorsSettingsRepository _settingsRepository;

    private readonly ILogger<CorsConfigurationController> _logger;

    public CorsConfigurationController(
        ICorsSettingsRepository settingsRepository, 
        ILogger<CorsConfigurationController> logger)
    {
        _settingsRepository = settingsRepository;
        _logger = logger;
    }

    [HttpGet]
    [Route("/stott.security.optimizely/api/[controller]/[action]")]
    public async Task<IActionResult> Get()
    {
        var settings = await _settingsRepository.GetAsync();

        return CreateSuccessJson(settings ?? new CorsConfiguration());
    }

    [HttpPost]
    [Route("/stott.security.optimizely/api/[controller]/[action]")]
    public async Task<IActionResult> Save([FromBody]CorsConfiguration configuration)
    {
        if (!ModelState.IsValid)
        {
            var validationModel = new ValidationModel(ModelState);
            return CreateValidationErrorJson(validationModel);
        }

        try
        {
            await _settingsRepository.SaveAsync(configuration, User.Identity?.Name);

            return Ok();
        }
        catch(Exception exception)
        {
            _logger.LogError(exception, $"{CspConstants.LogPrefix} Failed to save CSP changes.");
            throw;
        }
    }
}