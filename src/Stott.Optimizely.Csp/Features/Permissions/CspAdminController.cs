using System;

using EPiServer.Logging;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

using Stott.Optimizely.Csp.Common.Validation;
using Stott.Optimizely.Csp.Entities.Exceptions;
using Stott.Optimizely.Csp.Features.Permissions.List;
using Stott.Optimizely.Csp.Features.Permissions.Save;

namespace Stott.Optimizely.Csp.Features.Permissions
{
    public class CspPermissionsController : Controller
    {
        private readonly ICspPermissionsViewModelBuilder _viewModelBuilder;

        private readonly ISaveCspPermissionsCommand _saveCspPermissionsCommand;

        private ILogger _logger = LogManager.GetLogger(typeof(CspPermissionsController));

        public CspPermissionsController(
            ICspPermissionsViewModelBuilder viewModelBuilder, 
            ISaveCspPermissionsCommand saveCspPermissionsCommand)
        {
            _viewModelBuilder = viewModelBuilder;
            _saveCspPermissionsCommand = saveCspPermissionsCommand;
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
                _saveCspPermissionsCommand.Save(model.Id, model.Source, model.Directives);

                return Ok();
            }
            catch(EntityExistsException exception)
            {
                var validationModel = new ValidationModel(model.Source, exception.Message);
                return CreateValidationResponse(validationModel);
            }
            catch(Exception exception)
            {
                _logger.Error($"Failed to save CSP changes.", exception);
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
