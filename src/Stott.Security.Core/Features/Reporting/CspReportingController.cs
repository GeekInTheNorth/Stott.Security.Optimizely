using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Stott.Security.Core.Common;
using Stott.Security.Core.Features.Logging;
using Stott.Security.Core.Features.Reporting.Repository;
using Stott.Security.Core.Features.Whitelist;

namespace Stott.Security.Core.Features.Reporting
{
    [Authorize(Roles = CspConstants.AuthorizationPolicy)]
    public class CspReportingController : BaseController
    {
        private readonly ICspViolationReportRepository _repository;

        private readonly IWhitelistService _whitelistService;

        private readonly ILoggingProvider _logger;

        public CspReportingController(
            ICspViolationReportRepository repository,
            IWhitelistService whitelistService,
            ILoggingProviderFactory loggingProviderFactory)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _whitelistService = whitelistService ?? throw new ArgumentNullException(nameof(whitelistService));

            _logger = loggingProviderFactory.GetLogger(typeof(CspReportingController));
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("[controller]/[action]")]
        public async Task<IActionResult> Report([FromBody] ReportModel cspReport)
        {
            try
            {
                await _repository.SaveAsync(cspReport);

                var isOnWhitelist = await _whitelistService.IsOnWhitelistAsync(cspReport.BlockedUri, cspReport.ViolatedDirective);
                if (isOnWhitelist)
                {
                    await _whitelistService.AddFromWhiteListToCspAsync(cspReport.BlockedUri, cspReport.ViolatedDirective);
                }

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
        public async Task<IActionResult> ReportSummary()
        {
            try
            {
                var reportDate = DateTime.Today.AddDays(0 - CspConstants.LogRetentionDays);
                var model = await _repository.GetReportAsync(reportDate);

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
