using System;

using EPiServer.Logging;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Stott.Optimizely.Csp.Common;
using Stott.Optimizely.Csp.Features.Reporting.Repository;

namespace Stott.Optimizely.Csp.Features.Reporting
{
    [Authorize(Roles = "CmsAdmin,WebAdmins,Administrators")]
    public class CspReportingController : BaseController
    {
        private readonly ICspViolationReportRepository _repository;

        private ILogger _logger = LogManager.GetLogger(typeof(CspReportingController));

        public CspReportingController(ICspViolationReportRepository repository)
        {
            _repository = repository;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("[controller]/[action]")]
        public IActionResult Report([FromBody]ReportModel cspReport)
        {
            try
            {
                _repository.Save(cspReport);

                return Ok();
            }
            catch (Exception exception)
            {
                _logger.Error($"{CspConstants.LogPrefix} Failed to save CSP Report.", exception);
                throw;
            }
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("[controller]/[action]")]
        public IActionResult ReportSummary()
        {
            try
            {
                var reportDate = DateTime.Today.AddDays(0 - CspConstants.LogRetentionDays);
                var model = _repository.GetReport(reportDate);

                return CreateSuccessJson(model);
            }
            catch (Exception exception)
            {
                _logger.Error($"{CspConstants.LogPrefix} Failed to retrieve CSP Report.", exception);
                throw;
            }
        }
    }
}
