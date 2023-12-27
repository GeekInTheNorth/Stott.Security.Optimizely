namespace Stott.Security.Optimizely.Features.Reporting;

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Features.AllowList;
using Stott.Security.Optimizely.Features.Reporting.Models;
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
    [Consumes("application/csp-report")]
    public async Task<IActionResult> ReportUriViolation()
    {
        try
        {
            ReportUriWrapper? report = null;

            using (var streamReader = new StreamReader(Request.Body))
            {
                var content = await streamReader.ReadToEndAsync();

                report = JsonConvert.DeserializeObject<ReportUriWrapper>(content);
            }

            if (report != null)
            {
                await ProcessReport(report.CspReport);
            }

            return Ok();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"{CspConstants.LogPrefix} Failed to save CSP Report.");
            throw;
        }
    }

    [HttpPost]
    [AllowAnonymous]
    [Consumes("application/reports+json")]
    public async Task<IActionResult> ReportToViolation()
    {
        try
        {
            List<ReportToWrapper>? reports = null;

            using (var streamReader = new StreamReader(Request.Body))
            {
                var content = await streamReader.ReadToEndAsync();

                reports = JsonConvert.DeserializeObject<List<ReportToWrapper>>(content);
            }

            if (reports is not { Count: >0 })
            {
                return Ok();
            }

            foreach(var report in reports)
            {
                await ProcessReport(report.CspReport);
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

    private async Task ProcessReport(ICspReport report)
    {
        await _service.SaveAsync(report);

        var isOnAllowList = await _allowListService.IsOnAllowListAsync(report.BlockedUri, report.ViolatedDirective);
        if (isOnAllowList)
        {
            await _allowListService.AddFromAllowListToCspAsync(report.BlockedUri, report.ViolatedDirective);
        }
    }
}