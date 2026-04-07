namespace Stott.Security.Optimizely.Features.Csp.Permissions;

using System;
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
public sealed class CspPermissionsController(
    ICspPermissionsListModelBuilder viewModelBuilder,
    ICspPermissionService permissionService,
    ILogger<CspPermissionsController> logger) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> List(string? source, string? directive, string? appId, string? hostName)
    {
        try
        {
            var model = await viewModelBuilder
                .WithSourceFilter(source)
                .WithDirectiveFilter(directive)
                .WithAppId(appId)
                .WithHostName(hostName.GetSanitizedHostDomain())
                .BuildAsync();

            return CreateSuccessJson(model.Permissions);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "{prefix} Failed to load CSP permissions.", CspConstants.LogPrefix);
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
            await permissionService.SaveAsync(model.Id, model.Source, model.Directives, User.Identity?.Name, model.AppId, model.HostName.GetSanitizedHostDomain());

            return Ok();
        }
        catch (EntityExistsException exception)
        {
            var validationModel = new ValidationModel(nameof(model.Source), exception.Message);
            return CreateValidationErrorJson(validationModel);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "{prefix} Failed to save CSP changes.", CspConstants.LogPrefix);
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
            await permissionService.AppendDirectiveAsync(model.Source, model.Directive, User.Identity?.Name, model.AppId, model.HostName.GetSanitizedHostDomain());

            return Ok();
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "{prefix} Failed to append CSP changes.", CspConstants.LogPrefix);
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
            await permissionService.DeleteAsync(id, User.Identity?.Name);

            return Ok();
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "{prefix} Failed to delete CSP with an id of {id}.", CspConstants.LogPrefix, id);
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
