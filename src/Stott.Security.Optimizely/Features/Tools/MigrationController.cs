using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Stott.Security.Optimizely.Common;

namespace Stott.Security.Optimizely.Features.Tools;

[ApiExplorerSettings(IgnoreApi = true)]
[Authorize(Policy = CspConstants.AuthorizationPolicy)]
[Route("/stott.security.optimizely/api/[controller]/[action]")]
public sealed class MigrationController(
    IMigrationService migrationService, 
    ILogger<MigrationController> logger) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> Export()
    {
        try
        {
            var exportModel = await migrationService.Export();

            return CreateSuccessJson(exportModel);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "{LogPrefix} Failed to retrieve CSP settings.", CspConstants.LogPrefix);
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> Import()
    {
        try
        {
            var settings = await DeserializeFromBody<SettingsModel>();
            if (settings == null)
            {
                return BadRequest(new[] { "Could not deserialize settings." });
            }

            var validationErrors = settings.Validate(null).ToList();
            if (validationErrors is { Count: >0 })
            {
                return BadRequest(validationErrors.Select(x => x.ErrorMessage));
            }

            await migrationService.Import(settings, User.Identity?.Name);

            return CreateSuccessJson(new { Message = $"Settings imported successfully for: {string.Join(", ", settings.GetSettingsToUpdate())}." });
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "{LogPrefix} Failed to upload CSP settings.", CspConstants.LogPrefix);
            throw;
        }
    }
}