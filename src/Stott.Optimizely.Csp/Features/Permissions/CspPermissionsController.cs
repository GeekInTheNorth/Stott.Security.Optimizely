using System;

using EPiServer.Logging;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

using Stott.Optimizely.Csp.Common.Validation;
using Stott.Optimizely.Csp.Entities.Exceptions;
using Stott.Optimizely.Csp.Features.Permissions.List;
using Stott.Optimizely.Csp.Features.Permissions.Repository;
using Stott.Optimizely.Csp.Features.Permissions.Save;

namespace Stott.Optimizely.Csp.Features.Permissions
{
    public class CspPermissionsController : Controller
    {
        private readonly ICspPermissionsViewModelBuilder _viewModelBuilder;

        private readonly ICspPermissionRepository _cspPermissionRepository;

        private ILogger _logger = LogManager.GetLogger(typeof(CspPermissionsController));

        public CspPermissionsController(
            ICspPermissionsViewModelBuilder viewModelBuilder,
            ICspPermissionRepository cspPermissionRepository)
        {
            _viewModelBuilder = viewModelBuilder;
            _cspPermissionRepository = cspPermissionRepository;
        }

        [HttpGet]
        [Authorize(Roles = "CmsAdmin,WebAdmins,Administrators")]
        [Route("[controller]/[action]")]
        public IActionResult Index()
        {
            var model = _viewModelBuilder.Build();

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "CmsAdmin,WebAdmins,Administrators")]
        [Route("[controller]/[action]")]
        public IActionResult Save(SavePermissionModel model)
        {
            if (!ModelState.IsValid)
            {
                var validationModel = new ValidationModel(ModelState);
                return CreateValidationResponse(validationModel);
            }

            try
            {
                _cspPermissionRepository.Save(model.Id, model.Source, model.Directives);

                return Ok();
            }
            catch(EntityExistsException exception)
            {
                var validationModel = new ValidationModel(nameof(model.Source), exception.Message);
                return CreateValidationResponse(validationModel);
            }
            catch(Exception exception)
            {
                _logger.Error("Failed to save CSP changes.", exception);
                throw;
            }
        }

        [HttpPost]
        [Authorize(Roles = "CmsAdmin,WebAdmins,Administrators")]
        [Route("[controller]/[action]")]
        public IActionResult Delete(Guid id)
        {
            if (Guid.Empty.Equals(id))
            {
                var validationModel = new ValidationModel(nameof(id), $"{nameof(id)} must be a valid GUID.");
                return CreateValidationResponse(validationModel);
            }

            try
            {
                _cspPermissionRepository.Delete(id);

                return Ok();
            }
            catch (Exception exception)
            {
                _logger.Error($"Failed to delete CSP with an {nameof(id)} of {id}.", exception);
                throw;
            }
        }

        private static IActionResult CreateValidationResponse(ValidationModel validationModel)
        {
            return new ContentResult
            {
                StatusCode = 400,
                ContentType = "application/json",
                Content = JsonConvert.SerializeObject(validationModel)
            };
        }
    }
}
