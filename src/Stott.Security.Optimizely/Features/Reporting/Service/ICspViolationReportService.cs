namespace Stott.Security.Optimizely.Features.Reporting.Service;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface ICspViolationReportService
{
    Task SaveAsync(ReportModel violationReport);

    Task<IList<ViolationReportSummary>> GetReportAsync(string? source, string? directive, DateTime threshold);

    Task<int> DeleteAsync(DateTime threshold);
}