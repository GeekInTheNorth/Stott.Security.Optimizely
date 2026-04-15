namespace Stott.Security.Optimizely.Features.CustomHeaders;

using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Common.Validation;
using Stott.Security.Optimizely.Extensions;
using Stott.Security.Optimizely.Features.CustomHeaders.Models;
using Stott.Security.Optimizely.Features.CustomHeaders.Service;

/// <summary>
/// API controller for managing custom headers.
/// </summary>
[ApiExplorerSettings(IgnoreApi = true)]
[Authorize(Policy = CspConstants.AuthorizationPolicy)]
public sealed class CustomHeaderController : BaseController
{
    private readonly ICustomHeaderService _service;

    private readonly ILogger<CustomHeaderController> _logger;

    public CustomHeaderController(ICustomHeaderService service, ILogger<CustomHeaderController> logger)
    {
        _service = service;
        _logger = logger;
    }

    /// <summary>
    /// Gets a list of custom headers with optional filtering.
    /// </summary>
    [HttpGet]
    [Route("/stott.security.optimizely/api/customheader/list")]
    public async Task<IActionResult> List(string? headerName, string? behavior, Guid? siteId, string? hostName)
    {
        try
        {
            var sanitizedHost = hostName.GetSanitizedHostDomain();
            var headers = await _service.GetAllAsync(siteId, sanitizedHost);

            if (!string.IsNullOrWhiteSpace(headerName))
            {
                headers = headers.Where(x => x.HeaderName.Contains(headerName, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (string.Equals("Enabled", behavior, StringComparison.OrdinalIgnoreCase))
            {
                headers = headers.Where(x => x.Behavior != CustomHeaderBehavior.Disabled).ToList();
            }
            else if (Enum.TryParse<CustomHeaderBehavior>(behavior, out var behaviorEnum))
            {
                headers = headers.Where(x => x.Behavior == behaviorEnum).ToList();
            }

            return CreateSuccessJson(headers.OrderBy(x => x.HeaderName));
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "{LogPrefix} Failed to retrieve custom headers.", CspConstants.LogPrefix);
            throw;
        }
    }

    /// <summary>
    /// Checks whether the given context has its own custom header override.
    /// </summary>
    [HttpGet]
    [Route("/stott.security.optimizely/api/customheader/override/exists")]
    public async Task<IActionResult> HasOverride(Guid? siteId, string? hostName)
    {
        var sanitizedHost = hostName.GetSanitizedHostDomain();
        var existsForContext = await _service.ExistsForContextAsync(siteId, sanitizedHost);

        return CreateSuccessJson(new { existsForContext, isInherited = !existsForContext });
    }

    /// <summary>
    /// Creates an override by copying resolved headers from the parent context.
    /// </summary>
    [HttpPost]
    [Route("/stott.security.optimizely/api/customheader/override/create")]
    public async Task<IActionResult> CreateOverride(Guid? siteId, string? hostName)
    {
        if (!siteId.HasValue || siteId.Value == Guid.Empty)
        {
            var validationModel = new ValidationModel(nameof(siteId), "Cannot create an override for global context.");
            return CreateValidationErrorJson(validationModel);
        }

        try
        {
            var sanitizedHost = hostName.GetSanitizedHostDomain();
            await _service.CreateOverrideAsync(siteId, sanitizedHost, User.Identity?.Name);

            return Ok();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "{LogPrefix} Failed to create custom header override.", CspConstants.LogPrefix);
            throw;
        }
    }

    /// <summary>
    /// Deletes all custom headers for the given context (revert to inherited).
    /// </summary>
    [HttpDelete]
    [Route("/stott.security.optimizely/api/customheader/override/delete")]
    public async Task<IActionResult> DeleteOverride(Guid? siteId, string? hostName)
    {
        if (!siteId.HasValue || siteId.Value == Guid.Empty)
        {
            var validationModel = new ValidationModel(nameof(siteId), "Cannot delete global custom headers.");
            return CreateValidationErrorJson(validationModel);
        }

        try
        {
            var sanitizedHost = hostName.GetSanitizedHostDomain();
            await _service.DeleteByContextAsync(siteId, sanitizedHost, User.Identity?.Name);

            return Ok();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "{LogPrefix} Failed to delete custom headers for context.", CspConstants.LogPrefix);
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
            var sanitizedHost = model.HostName.GetSanitizedHostDomain();
            await _service.SaveAsync(model, User.Identity?.Name, model.SiteId, sanitizedHost);

            return Ok();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "{LogPrefix} Failed to save custom header.", CspConstants.LogPrefix);
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
            await _service.DeleteAsync(id);

            return Ok();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "{LogPrefix} Failed to delete custom header.", CspConstants.LogPrefix);
            throw;
        }
    }
}
