namespace Stott.Security.Optimizely.Features.Permissions;

using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Common.Validation;
using Stott.Security.Optimizely.Entities.Exceptions;
using Stott.Security.Optimizely.Features.Permissions.List;
using Stott.Security.Optimizely.Features.Permissions.Save;
using Stott.Security.Optimizely.Features.Permissions.Service;

[ApiExplorerSettings(IgnoreApi = true)]
[Authorize(Policy = CspConstants.AuthorizationPolicy)]
public class CspPermissionsController : BaseController
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
    [Route("[controller]/[action]")]
    public async Task<IActionResult> List()
    {
        try
        {
            var model = await _viewModelBuilder.BuildAsync();

            return CreateSuccessJson(model.Permissions);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"{CspConstants.LogPrefix} Failed to load CSP permissions.");
            throw;
        }
    }

    [HttpPost]
    [Route("[controller]/[action]")]
    public async Task<IActionResult> Save(SavePermissionModel model)
    {
        if (!ModelState.IsValid)
        {
            var validationModel = new ValidationModel(ModelState);
            return CreateValidationErrorJson(validationModel);
        }

        try
        {
            await _permissionService.SaveAsync(model.Id, model.Source, model.Directives);

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
    [Route("[controller]/[action]")]
    public async Task<IActionResult> Append(AppendPermissionModel model)
    {
        if (!ModelState.IsValid)
        {
            var validationModel = new ValidationModel(ModelState);
            return CreateValidationErrorJson(validationModel);
        }

        try
        {
            await _permissionService.AppendDirectiveAsync(model.Source, model.Directive);

            return Ok();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"{CspConstants.LogPrefix} Failed to append CSP changes.");
            throw;
        }
    }

    [HttpPost]
    [Route("[controller]/[action]")]
    public async Task<IActionResult> Delete(Guid id)
    {
        if (Guid.Empty.Equals(id))
        {
            var validationModel = new ValidationModel(nameof(id), $"{nameof(id)} must be a valid GUID.");
            return CreateValidationErrorJson(validationModel);
        }

        try
        {
            await _permissionService.DeleteAsync(id);

            return Ok();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"{CspConstants.LogPrefix} Failed to delete CSP with an {nameof(id)} of {id}.");
            throw;
        }
    }
}
