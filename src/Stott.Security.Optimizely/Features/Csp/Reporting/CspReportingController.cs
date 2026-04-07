using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Features.Csp.AllowList;
using Stott.Security.Optimizely.Features.Csp.Reporting.Models;
using Stott.Security.Optimizely.Features.Csp.Reporting.Service;
using Stott.Security.Optimizely.Features.Csp.Settings.Service;
using Stott.Security.Optimizely.Features.Route;
using Stott.Security.Optimizely.Extensions;

namespace Stott.Security.Optimizely.Features.Csp.Reporting;

[ApiExplorerSettings(IgnoreApi = true)]
[Authorize(Policy = CspConstants.AuthorizationPolicy)]
[Route("/stott.security.optimizely/api/[controller]")]
public sealed class CspReportingController(
    ICspViolationReportService service,
    IAllowListService allowListService,
    ICspSettingsService settingsService,
    ISecurityRouteHelper routeHelper,
    ILogger<CspReportingController> logger) : BaseController
{
    [HttpOptions("reporttoviolation")]
    [AllowAnonymous]
    public IActionResult ReportToViolationOptions()
    {
        Response.Headers.Append("Allow", "POST, OPTIONS");
        Response.Headers.Append("Accept", "application/reports+json, application/csp-report");

        return Ok();
    }

    [HttpPost("reporttoviolation")]
    [AllowAnonymous]
    [Consumes("application/reports+json", "application/csp-report")]
    public async Task<IActionResult> ReportToViolation()
    {
        try
        {
            var routeData = await routeHelper.GetRouteDataAsync();
            var appId = routeData?.AppId;
            var hostName = routeData?.HostName.GetSanitizedHostDomain();

            var currentSettings = await settingsService.GetAsync(appId, hostName);
            if (currentSettings is not { IsEnabled: true, UseInternalReporting: true })
            {
                return Ok("CSP Report has not been retained.");
            }

            var requestBody = await GetBody();
            var reports = new List<ReportToWrapper>();
            var contentType = Request.Headers.TryGetValue("Content-Type", out var values) ? values.ToString() : string.Empty;
            if (string.Equals(contentType, "application/csp-report", StringComparison.OrdinalIgnoreCase))
            {
                var report = JsonSerializer.Deserialize<ReportToWrapper>(requestBody);
                if (report != null)
                {
                    reports.Add(report);
                }
            }
            else if (string.Equals(contentType, "application/reports+json", StringComparison.OrdinalIgnoreCase))
            {
                var reportList = JsonSerializer.Deserialize<List<ReportToWrapper>>(requestBody);
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
                await ProcessReport(report.CspReport, appId, hostName);
            }

            return Ok();
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "{LogPrefix} Failed to save CSP Report.", CspConstants.LogPrefix);
            throw;
        }
    }

    [HttpGet]
    [Route("[action]")]
    public async Task<IActionResult> ReportSummary(string? source, string? directive, string? appId, string? hostName)
    {
        try
        {
            var reportDate = DateTime.Today.AddDays(0 - CspConstants.LogRetentionDays);
            var model = await service.GetReportAsync(source, directive, reportDate, appId, hostName.GetSanitizedHostDomain());

            return CreateSuccessJson(model);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "{LogPrefix} Failed to retrieve CSP Report.", CspConstants.LogPrefix);
            throw;
        }
    }

    private async Task ProcessReport(ICspReport report, string? appId, string? hostName)
    {
        var sanitizedHost = hostName.GetSanitizedHostDomain();
        await service.SaveAsync(report, appId, sanitizedHost);

        var isOnAllowList = await allowListService.IsOnAllowListAsync(report.BlockedUri, report.ViolatedDirective, appId, sanitizedHost);
        if (isOnAllowList)
        {
            await allowListService.AddFromAllowListToCspAsync(report.BlockedUri, report.ViolatedDirective, appId, sanitizedHost);
        }
    }
}