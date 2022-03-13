using System;

using EPiServer.Logging;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Stott.Optimizely.Csp.Common;
using Stott.Optimizely.Csp.Features.SecurityHeaders.Enums;
using Stott.Optimizely.Csp.Features.SecurityHeaders.Repository;

namespace Stott.Optimizely.Csp.Features.SecurityHeaders
{
    [Authorize(Roles = "CmsAdmin,WebAdmins,Administrators")]
    public class SecurityHeaderController : Controller
    {
        private readonly ISecurityHeaderRepository _repository;

        private ILogger _logger = LogManager.GetLogger(typeof(SecurityHeaderController));

        public SecurityHeaderController(ISecurityHeaderRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [Route("[controller]/[action]")]
        public JsonResult Get()
        {
            try
            {
                var data = _repository.Get();

                return Json(new 
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
        public IActionResult Save(bool isXctoEnabled, bool isXxpEnabled, XFrameOptions xFrameOptions, ReferrerPolicy referrerPolicy)
        {
            try
            {
                _repository.Save(isXctoEnabled, isXxpEnabled, referrerPolicy, xFrameOptions);

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
