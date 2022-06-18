using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Stott.Security.Core.Common;
using Stott.Security.Core.Features.Logging;
using Stott.Security.Core.Features.Settings.Service;

namespace Stott.Security.Core.Features.Settings
{
    [Authorize(Policy = CspConstants.AuthorizationPolicy)]
    public class CspSettingsController : BaseController
    {
        private readonly ICspSettingsService _settings;

        private readonly ILoggingProvider _logger;

        public CspSettingsController(
            ICspSettingsService service,
            ILoggingProviderFactory loggingProviderFactory)
        {
            _settings = service;
            _logger = loggingProviderFactory.GetLogger(typeof(CspSettingsController));
        }

        [HttpGet]
        [Route("[controller]/[action]")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var data = await _settings.GetAsync();

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
                await _settings.SaveAsync(isEnabled, isReportOnly);

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
