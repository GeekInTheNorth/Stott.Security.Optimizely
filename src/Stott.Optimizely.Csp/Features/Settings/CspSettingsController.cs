using System;

using EPiServer.Logging;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Stott.Optimizely.Csp.Features.Settings.Repository;

namespace Stott.Optimizely.Csp.Features.Settings
{
    [Authorize(Roles = "CmsAdmin,WebAdmins,Administrators")]
    public class CspSettingsController : Controller
    {
        private readonly ICspSettingsRepository _repository;

        private ILogger _logger = LogManager.GetLogger(typeof(CspSettingsController));

        public CspSettingsController(ICspSettingsRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [Route("[controller]/get")]
        public JsonResult Get()
        {
            try
            {
                var data = _repository.Get();

                return Json(new { data.IsEnabled, data.IsReportOnly });
            }
            catch (Exception exception)
            {
                _logger.Error("Failed to retrieve CSP settings.", exception);
                throw;
            }
        }

        [HttpPost]
        [Route("[controller]/[action]")]
        public IActionResult Save(bool isEnabled, bool isReportOnly)
        {
            try
            {
                _repository.Save(isEnabled, isReportOnly);

                return Ok();
            }
            catch (Exception exception)
            {
                _logger.Error("Failed to save CSP settings.", exception);
                throw;
            }
        }
    }
}
