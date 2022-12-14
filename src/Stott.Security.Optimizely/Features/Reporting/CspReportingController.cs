namespace Stott.Security.Optimizely.Features.Reporting;

using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Features.Reporting.Service;
using Stott.Security.Optimizely.Features.Whitelist;

[ApiExplorerSettings(IgnoreApi = true)]
[Authorize(Policy = CspConstants.AuthorizationPolicy)]
[Route("/stott.security.optimizely/api/[controller]/[action]")]
public class CspReportingController : BaseController
{
    private readonly ICspViolationReportService _service;

    private readonly IWhitelistService _whitelistService;

    private readonly ILogger<CspReportingController> _logger;

    public CspReportingController(
        ICspViolationReportService service,
        IWhitelistService whitelistService,
        ILogger<CspReportingController> logger)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _whitelistService = whitelistService ?? throw new ArgumentNullException(nameof(whitelistService));

        _logger = logger;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Report([FromBody] ReportModel cspReport)
    {
        try
        {
            await _service.SaveAsync(cspReport);

            var isOnWhitelist = await _whitelistService.IsOnWhitelistAsync(cspReport.BlockedUri, cspReport.ViolatedDirective);
            if (isOnWhitelist)
            {
                await _whitelistService.AddFromWhiteListToCspAsync(cspReport.BlockedUri, cspReport.ViolatedDirective);
            }

            return Ok();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"{CspConstants.LogPrefix} Failed to save CSP Report.");
            throw;
        }
    }

    [HttpGet]
    public async Task<IActionResult> ReportSummary()
    {
        try
        {
            var reportDate = DateTime.Today.AddDays(0 - CspConstants.LogRetentionDays);
            var model = await _service.GetReportAsync(reportDate);

            return CreateSuccessJson(model);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"{CspConstants.LogPrefix} Failed to retrieve CSP Report.");
            throw;
        }
    }
}
