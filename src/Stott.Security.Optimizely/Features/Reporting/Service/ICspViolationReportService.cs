namespace Stott.Security.Optimizely.Features.Reporting.Service;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Stott.Security.Optimizely.Features.Reporting.Models;

public interface ICspViolationReportService
{
    Task SaveAsync(ICspReport violationReport);

    Task<IList<ViolationReportSummary>> GetReportAsync(string? source, string? directive, DateTime threshold);

    Task<int> DeleteAsync(DateTime threshold);
}