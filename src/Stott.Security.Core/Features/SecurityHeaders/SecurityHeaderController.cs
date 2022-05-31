using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Stott.Security.Core.Common;
using Stott.Security.Core.Features.Logging;
using Stott.Security.Core.Features.SecurityHeaders.Enums;
using Stott.Security.Core.Features.SecurityHeaders.Repository;

namespace Stott.Security.Core.Features.SecurityHeaders
{
    [Authorize(Roles = "CmsAdmin,WebAdmins,Administrators")]
    public class SecurityHeaderController : BaseController
    {
        private readonly ISecurityHeaderRepository _repository;

        private readonly ILoggingProvider _logger;

        public SecurityHeaderController(
            ISecurityHeaderRepository repository,
            ILoggingProviderFactory loggingProviderFactory)
        {
            _repository = repository;
            _logger = loggingProviderFactory.GetLogger(typeof(SecurityHeaderController));
        }

        [HttpGet]
        [Route("[controller]/[action]")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var data = await _repository.GetAsync();

                return CreateSuccessJson(new SecurityHeaderModel
                {
                    IsXctoEnabled = data.IsXContentTypeOptionsEnabled,
                    IsXxpEnabled = data.IsXXssProtectionEnabled,
                    XFrameOptions = data.FrameOptions.ToString(),
                    ReferrerPolicy = data.ReferrerPolicy.ToString()
                });
            }
            catch (Exception exception)
            {
                _logger.Error($"{CspConstants.LogPrefix} Failed to retrieve Security Header settings.", exception);
                throw;
            }
        }

        [HttpPost]
        [Route("[controller]/[action]")]
        public async Task<IActionResult> Save(bool isXctoEnabled, bool isXxpEnabled, XFrameOptions xFrameOptions, ReferrerPolicy referrerPolicy)
        {
            try
            {
                await _repository.SaveAsync(isXctoEnabled, isXxpEnabled, referrerPolicy, xFrameOptions);

                return Ok();
            }
            catch (Exception exception)
            {
                _logger.Error($"{CspConstants.LogPrefix} Failed to save Security Header Settings.", exception);
                throw;
            }
        }
    }
}
