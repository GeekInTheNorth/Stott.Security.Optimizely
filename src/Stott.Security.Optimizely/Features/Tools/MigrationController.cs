using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using Stott.Security.Optimizely.Common;

namespace Stott.Security.Optimizely.Features.Tools;

[ApiExplorerSettings(IgnoreApi = true)]
[Authorize(Policy = CspConstants.AuthorizationPolicy)]
[Route("/stott.security.optimizely/api/[controller]/[action]")]
public sealed class MigrationController : BaseController
{
    private readonly IMigrationService _migrationService;

    private readonly ILogger<MigrationController> _logger;

    public MigrationController(IMigrationService migrationService, ILogger<MigrationController> logger)
    {
        _migrationService = migrationService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Export()
    {
        try
        {
            var exportModel = await _migrationService.Export();

            return CreateSuccessJson(exportModel);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "{LogPrefix} Failed to retrieve CSP settings.", CspConstants.LogPrefix);
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> Import(
        [FromQuery] bool importCsp = true,
        [FromQuery] bool importCors = true,
        [FromQuery] bool importHeaders = true,
        [FromQuery] bool importPermissionPolicy = true)
    {
        try
        {
            var requestBody = await GetBody();
            var settings = JsonConvert.DeserializeObject<SettingsModel>(requestBody);
            if (settings == null)
            {
                return BadRequest(new[] { "Could not deserialize settings." });
            }

            if (!importCsp) { settings.Csp = null; }
            if (!importCors) { settings.Cors = null; }
            if (!importHeaders) { settings.CustomHeaders = null; }
            if (!importPermissionPolicy) { settings.PermissionPolicy = null; }

            var validationErrors = settings.Validate(null).ToList();
            if (validationErrors is { Count: > 0 })
            {
                return BadRequest(validationErrors.Select(x => x.ErrorMessage));
            }

            await _migrationService.Import(settings, User.Identity?.Name);

            return CreateSuccessJson(new { Message = $"Settings imported successfully for: {string.Join(", ", settings.GetSettingsToUpdate())}." });
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "{LogPrefix} Failed to upload CSP settings.", CspConstants.LogPrefix);
            throw;
        }
    }
}