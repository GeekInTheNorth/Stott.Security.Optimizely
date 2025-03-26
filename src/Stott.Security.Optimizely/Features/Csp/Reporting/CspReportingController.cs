namespace Stott.Security.Optimizely.Features.Csp.Reporting;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;
using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Features.Csp.AllowList;
using Stott.Security.Optimizely.Features.Csp.Reporting.Models;
using Stott.Security.Optimizely.Features.Csp.Reporting.Service;
using Stott.Security.Optimizely.Features.Csp.Settings.Service;

[ApiExplorerSettings(IgnoreApi = true)]
[Authorize(Policy = CspConstants.AuthorizationPolicy)]
[Route("/stott.security.optimizely/api/[controller]/[action]")]
public sealed class CspReportingController : BaseController
{
    private readonly ICspViolationReportService _service;

    private readonly IAllowListService _allowListService;

    private readonly ICspSettingsService _settingsService;

    private readonly ILogger<CspReportingController> _logger;

    public CspReportingController(
        ICspViolationReportService service,
        IAllowListService allowListService,
        ICspSettingsService settingsService,
        ILogger<CspReportingController> logger)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _allowListService = allowListService ?? throw new ArgumentNullException(nameof(allowListService));
        _settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));

        _logger = logger;
    }

    [HttpPost]
    [AllowAnonymous]
    [Consumes("application/csp-report")]
    public async Task<IActionResult> ReportUriViolation()
    {
        try
        {
            var currentSettings = await _settingsService.GetAsync();
            if (currentSettings is not { IsEnabled: true, UseInternalReporting: true })
            {
                return Ok("CSP Report has not been retained.");
            }

            var requestBody = await GetBody();
            var report = JsonConvert.DeserializeObject<ReportUriWrapper>(requestBody);

            if (report != null)
            {
                await ProcessReport(report.CspReport);
            }

            return Ok();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "{LogPrefix} Failed to save CSP Report.", CspConstants.LogPrefix);
            throw;
        }
    }

    [HttpPost]
    [AllowAnonymous]
    [Consumes("application/reports+json", "application/csp-report")]
    public async Task<IActionResult> ReportToViolation()
    {
        try
        {
            var currentSettings = await _settingsService.GetAsync();
            if (currentSettings is not { IsEnabled: true, UseInternalReporting: true })
            {
                return Ok("CSP Report has not been retained.");
            }

            var requestBody = await GetBody();
            List<ReportToWrapper> reports = new List<ReportToWrapper>();
            var contentType = Request.Headers.TryGetValue("Content-Type", out var values) ? values.ToString() : string.Empty;
            if (string.Equals(contentType, "application/csp-report", StringComparison.OrdinalIgnoreCase))
            {
                var report = JsonConvert.DeserializeObject<ReportToWrapper>(requestBody);
                if (report != null)
                {
                    reports.Add(report);
                }
            }
            else if (string.Equals(contentType, "application/reports+json", StringComparison.OrdinalIgnoreCase))
            {
                var reportList = JsonConvert.DeserializeObject<List<ReportToWrapper>>(requestBody);
                if (reportList is { Count: > 0 })
                {
                    reports.AddRange(reportList);
                }
            }

            if (reports is not { Count: > 0 })
            {
                return Ok();
            }

            foreach (var report in reports)
            {
                await ProcessReport(report.CspReport);
            }

            return Ok();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "{LogPrefix} Failed to save CSP Report.", CspConstants.LogPrefix);
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
            _logger.LogError(exception, "{LogPrefix} Failed to retrieve CSP Report.", CspConstants.LogPrefix);
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