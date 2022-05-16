using System;
using System.Threading.Tasks;

using EPiServer.Logging;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Stott.Optimizely.Csp.Common;
using Stott.Optimizely.Csp.Features.Settings.Repository;

namespace Stott.Optimizely.Csp.Features.Settings
{
    [Authorize(Roles = "CmsAdmin,WebAdmins,Administrators")]
    public class CspSettingsController : BaseController
    {
        private readonly ICspSettingsRepository _repository;

        private readonly ILogger _logger = LogManager.GetLogger(typeof(CspSettingsController));

        public CspSettingsController(ICspSettingsRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [Route("[controller]/[action]")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var data = await _repository.GetAsync();

                return CreateSuccessJson(new CspSettingsModel 
                { 
                    IsEnabled = data.IsEnabled,
                    IsReportOnly = data.IsReportOnly
                });
            }
            catch (Exception exception)
            {
                _logger.Error($"{CspConstants.LogPrefix} Failed to retrieve CSP settings.", exception);
                throw;
            }
        }

        [HttpPost]
        [Route("[controller]/[action]")]
        public async Task<IActionResult> Save(bool isEnabled, bool isReportOnly)
        {
            try
            {
                await _repository.SaveAsync(isEnabled, isReportOnly);

                return Ok();
            }
            catch (Exception exception)
            {
                _logger.Error($"{CspConstants.LogPrefix} Failed to save CSP settings.", exception);
                throw;
            }
        }
    }
}
