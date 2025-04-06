namespace Stott.Security.Optimizely.Features.Csp.Reporting.Service;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Stott.Security.Optimizely.Features.Csp.Reporting;
using Stott.Security.Optimizely.Features.Csp.Reporting.Models;

public interface ICspViolationReportService
{
    Task SaveAsync(ICspReport violationReport);

    Task<IList<ViolationReportSummary>> GetReportAsync(string? source, string? directive, DateTime threshold);

    Task<int> DeleteAsync(DateTime threshold);
}