using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Stott.Security.Core.Common;
using Stott.Security.Core.Common.Validation;
using Stott.Security.Core.Entities.Exceptions;
using Stott.Security.Core.Features.Logging;
using Stott.Security.Core.Features.Permissions.List;
using Stott.Security.Core.Features.Permissions.Repository;
using Stott.Security.Core.Features.Permissions.Save;

namespace Stott.Security.Core.Features.Permissions
{
    [Authorize(Policy = CspConstants.AuthorizationPolicy)]
    public class CspPermissionsController : BaseController
    {
        private readonly ICspPermissionsListModelBuilder _viewModelBuilder;

        private readonly ICspPermissionRepository _cspPermissionRepository;

        private readonly ILoggingProvider _logger;

        public CspPermissionsController(
            ICspPermissionsListModelBuilder viewModelBuilder,
            ICspPermissionRepository cspPermissionRepository,
            ILoggingProviderFactory loggingProviderFactory)
        {
            _viewModelBuilder = viewModelBuilder;
            _cspPermissionRepository = cspPermissionRepository;
            _logger = loggingProviderFactory.GetLogger(typeof(CspPermissionsController));
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
            catch (EntityExistsException exception)
            {
                var validationModel = new ValidationModel(nameof(model.Source), exception.Message);
                return CreateValidationErrorJson(validationModel);
            }
            catch (Exception exception)
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
