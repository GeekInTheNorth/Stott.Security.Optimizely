namespace Stott.Security.Optimizely.Features.Csp.Reporting.Repository;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Stott.Security.Optimizely.Features.Csp.Reporting;

public interface ICspViolationReportRepository
{
    Task SaveAsync(string blockedUri, string violatedDirective, Guid? siteId, string? hostName);

    Task<IList<ViolationReportSummary>> GetReportAsync(string? source, string? directive, DateTime threshold, Guid? siteId, string? hostName);

    Task<int> DeleteAsync(DateTime threshold);
}