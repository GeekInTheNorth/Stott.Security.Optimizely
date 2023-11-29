namespace Stott.Security.Optimizely.Features.Reporting.Repository;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface ICspViolationReportRepository
{
    Task SaveAsync(string blockedUri, string violatedDirective);

    Task<IList<ViolationReportSummary>> GetReportAsync(string? source, string? directive, DateTime threshold);

    Task<int> DeleteAsync(DateTime threshold);
}