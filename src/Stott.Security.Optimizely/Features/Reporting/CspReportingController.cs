namespace Stott.Security.Optimizely.Features.Reporting;

using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Features.AllowList;
using Stott.Security.Optimizely.Features.Reporting.Service;

[ApiExplorerSettings(IgnoreApi = true)]
[Authorize(Policy = CspConstants.AuthorizationPolicy)]
[Route("/stott.security.optimizely/api/[controller]/[action]")]
public sealed class CspReportingController : BaseController
{
    private readonly ICspViolationReportService _service;

    private readonly IAllowListService _allowListService;

    private readonly ILogger<CspReportingController> _logger;

    public CspReportingController(
        ICspViolationReportService service,
        IAllowListService allowListService,
        ILogger<CspReportingController> logger)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _allowListService = allowListService ?? throw new ArgumentNullException(nameof(allowListService));

        _logger = logger;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Report([FromBody] ReportModel cspReport)
    {
        try
        {
            await _service.SaveAsync(cspReport);

            var isOnAllowList = await _allowListService.IsOnAllowListAsync(cspReport.BlockedUri, cspReport.ViolatedDirective);
            if (isOnAllowList)
            {
                await _allowListService.AddFromAllowListToCspAsync(cspReport.BlockedUri, cspReport.ViolatedDirective);
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
    public async Task<IActionResult> ReportSummary(string? source, string? directive)
    {
        try
        {
            var reportDate = DateTime.Today.AddDays(0 - CspConstants.LogRetentionDays);
            var model = await _service.GetReportAsync(source, directive, reportDate);

            return CreateSuccessJson(model);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"{CspConstants.LogPrefix} Failed to retrieve CSP Report.");
            throw;
        }
    }
}