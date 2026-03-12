namespace Stott.Security.Optimizely.Features.CustomHeaders;

using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Common.Validation;
using Stott.Security.Optimizely.Features.CustomHeaders.Models;
using Stott.Security.Optimizely.Features.CustomHeaders.Service;

/// <summary>
/// API controller for managing custom headers.
/// </summary>
[ApiExplorerSettings(IgnoreApi = true)]
[Authorize(Policy = CspConstants.AuthorizationPolicy)]
public sealed class CustomHeaderController(
    ICustomHeaderService service, 
    ILogger<CustomHeaderController> logger) : BaseController
{
    /// <summary>
    /// Gets a list of custom headers with optional filtering.
    /// </summary>
    [HttpGet]
    [Route("/stott.security.optimizely/api/customheader/list")]
    public async Task<IActionResult> List(string? headerName, CustomHeaderBehavior? behavior)
    {
        try
        {
            var headers = await service.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(headerName))
            {
                headers = headers.Where(x => x.HeaderName.Contains(headerName, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (behavior.HasValue)
            {
                headers = headers.Where(x => x.Behavior == behavior.Value).ToList();
            }

            return CreateSuccessJson(headers.OrderBy(x => x.HeaderName));
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "{LogPrefix} Failed to retrieve custom headers.", CspConstants.LogPrefix);
            throw;
        }
    }

    /// <summary>
    /// Saves a custom header (creates new or updates existing).
    /// </summary>
    [HttpPost]
    [Route("/stott.security.optimizely/api/customheader/save")]
    public async Task<IActionResult> Save([FromBody] SaveCustomHeaderModel model)
    {
        if (!ModelState.IsValid)
        {
            var validationModel = new ValidationModel(ModelState);
            return CreateValidationErrorJson(validationModel);
        }

        try
        {
            await service.SaveAsync(model, User.Identity?.Name);

            return Ok();
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "{LogPrefix} Failed to save custom header.", CspConstants.LogPrefix);
            throw;
        }
    }

    /// <summary>
    /// Deletes a custom header by its unique identifier.
    /// </summary>
    [HttpDelete]
    [Route("/stott.security.optimizely/api/customheader/delete")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await service.DeleteAsync(id);

            return Ok();
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "{LogPrefix} Failed to delete custom header.", CspConstants.LogPrefix);
            throw;
        }
    }
}
