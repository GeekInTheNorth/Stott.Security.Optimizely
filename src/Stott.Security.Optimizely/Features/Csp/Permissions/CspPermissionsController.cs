namespace Stott.Security.Optimizely.Features.Csp.Permissions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Common.Validation;
using Stott.Security.Optimizely.Entities.Exceptions;
using Stott.Security.Optimizely.Extensions;
using Stott.Security.Optimizely.Features.Csp.Permissions.List;
using Stott.Security.Optimizely.Features.Csp.Permissions.Save;
using Stott.Security.Optimizely.Features.Csp.Permissions.Service;
using Stott.Security.Optimizely.Features.Csp.Permissions.Validation;

[ApiExplorerSettings(IgnoreApi = true)]
[Authorize(Policy = CspConstants.AuthorizationPolicy)]
[Route("/stott.security.optimizely/api/[controller]/[action]")]
public sealed class CspPermissionsController : BaseController
{
    private readonly ICspPermissionsListModelBuilder _viewModelBuilder;

    private readonly ICspPermissionService _permissionService;

    private readonly ILogger<CspPermissionsController> _logger;

    public CspPermissionsController(
        ICspPermissionsListModelBuilder viewModelBuilder,
        ICspPermissionService permissionService,
        ILogger<CspPermissionsController> logger)
    {
        _viewModelBuilder = viewModelBuilder;
        _permissionService = permissionService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> List(string? source, string? directive, string? appId, string? hostName)
    {
        try
        {
            var model = await _viewModelBuilder
                .WithSourceFilter(source)
                .WithDirectiveFilter(directive)
                .WithAppId(appId)
                .WithHostName(hostName.GetSanitizedHostDomain())
                .BuildAsync();

            return CreateSuccessJson(model.Permissions);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"{CspConstants.LogPrefix} Failed to load CSP permissions.");
            throw;
        }
    }

    [HttpGet]
    public async Task<IActionResult> ListInherited(string? appId, string? hostName)
    {
        try
        {
            var inherited = new List<CspPermissionListModel>();

            // Get global sources when viewing app or host level
            if (!string.IsNullOrWhiteSpace(appId))
            {
                var globalSources = await _permissionService.GetByContextAsync(null, null);
                if (globalSources is { Count: > 0 })
                {
                    inherited.AddRange(globalSources.Select(x => new CspPermissionListModel(x)));
                }
            }

            // Get app-level sources when viewing host level
            if (!string.IsNullOrWhiteSpace(appId) && !string.IsNullOrWhiteSpace(hostName))
            {
                var appSources = await _permissionService.GetByContextAsync(appId, null);
                if (appSources is { Count: > 0 })
                {
                    inherited.AddRange(appSources.Select(x => new CspPermissionListModel(x)));
                }
            }

            return CreateSuccessJson(inherited.OrderBy(x => x.SortSource).ToList());
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"{CspConstants.LogPrefix} Failed to load inherited CSP permissions.");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> Save(SavePermissionModel model)
    {
        if (!ModelState.IsValid)
        {
            var validationModel = new ValidationModel(ModelState);
            return CreateValidationErrorJson(validationModel);
        }

        try
        {
            await _permissionService.SaveAsync(model.Id, model.Source, model.Directives, User.Identity?.Name, model.AppId, model.HostName.GetSanitizedHostDomain());

            return Ok();
        }
        catch (EntityExistsException exception)
        {
            var validationModel = new ValidationModel(nameof(model.Source), exception.Message);
            return CreateValidationErrorJson(validationModel);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"{CspConstants.LogPrefix} Failed to save CSP changes.");
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> Append(AppendPermissionModel model)
    {
        if (!ModelState.IsValid)
        {
            var validationModel = new ValidationModel(ModelState);
            return CreateValidationErrorJson(validationModel);
        }

        try
        {
            await _permissionService.AppendDirectiveAsync(model.Source, model.Directive, User.Identity?.Name, model.AppId, model.HostName.GetSanitizedHostDomain());

            return Ok();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"{CspConstants.LogPrefix} Failed to append CSP changes.");
            throw;
        }
    }

    [HttpDelete]
    [Route("/stott.security.optimizely/api/[controller]/[action]/{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        if (Guid.Empty.Equals(id))
        {
            var validationModel = new ValidationModel(nameof(id), $"{nameof(id)} must be a valid GUID.");
            return CreateValidationErrorJson(validationModel);
        }

        try
        {
            await _permissionService.DeleteAsync(id, User.Identity?.Name);

            return Ok();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"{CspConstants.LogPrefix} Failed to delete CSP with an {nameof(id)} of {id}.");
            throw;
        }
    }

    [HttpGet]
    public IActionResult ValidDirectives(string? source)
    {
        var sourceRules = SourceRules.GetRuleForSource(source);

        return CreateSuccessJson(sourceRules?.ValidDirectives);
    }
}
