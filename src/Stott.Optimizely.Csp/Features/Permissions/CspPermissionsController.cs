using System;
using System.Threading.Tasks;

using EPiServer.Logging;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Stott.Optimizely.Csp.Common;
using Stott.Optimizely.Csp.Common.Validation;
using Stott.Optimizely.Csp.Entities.Exceptions;
using Stott.Optimizely.Csp.Features.Permissions.List;
using Stott.Optimizely.Csp.Features.Permissions.Repository;
using Stott.Optimizely.Csp.Features.Permissions.Save;

namespace Stott.Optimizely.Csp.Features.Permissions
{
    [Authorize(Roles = "CmsAdmin,WebAdmins,Administrators")]
    public class CspPermissionsController : BaseController
    {
        private readonly ICspPermissionsListModelBuilder _viewModelBuilder;

        private readonly ICspPermissionRepository _cspPermissionRepository;

        private readonly ILogger _logger = LogManager.GetLogger(typeof(CspPermissionsController));

        public CspPermissionsController(
            ICspPermissionsListModelBuilder viewModelBuilder,
            ICspPermissionRepository cspPermissionRepository)
        {
            _viewModelBuilder = viewModelBuilder;
            _cspPermissionRepository = cspPermissionRepository;
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
                _logger.Error($"{CspConstants.LogPrefix} Failed to load CSP permissions.", exception);
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
                await _cspPermissionRepository.SaveAsync(model.Id, model.Source, model.Directives);

                return Ok();
            }
            catch(EntityExistsException exception)
            {
                var validationModel = new ValidationModel(nameof(model.Source), exception.Message);
                return CreateValidationErrorJson(validationModel);
            }
            catch(Exception exception)
            {
                _logger.Error($"{CspConstants.LogPrefix} Failed to save CSP changes.", exception);
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
                await _cspPermissionRepository.AppendDirectiveAsync(model.Source, model.Directive);

                return Ok();
            }
            catch (Exception exception)
            {
                _logger.Error($"{CspConstants.LogPrefix} Failed to append CSP changes.", exception);
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
                await _cspPermissionRepository.DeleteAsync(id);

                return Ok();
            }
            catch (Exception exception)
            {
                _logger.Error($"{CspConstants.LogPrefix} Failed to delete CSP with an {nameof(id)} of {id}.", exception);
                throw;
            }
        }
    }
}
