namespace Stott.Security.Optimizely.Features.Reporting.Service;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Features.Reporting.Repository;

internal class CspViolationReportService : ICspViolationReportService
{
    private readonly ICspViolationReportRepository _repository;

    public CspViolationReportService(ICspViolationReportRepository repository)
    {
        _repository = repository;
    }

    public async Task<int> DeleteAsync(DateTime threshold)
    {
        return await _repository.DeleteAsync(threshold);
    }

    public async Task<IList<ViolationReportSummary>> GetReportAsync(DateTime threshold)
    {
        return await _repository.GetReportAsync(threshold);
    }

    public async Task SaveAsync(ReportModel violationReport)
    {
        var blockedUri = GetFormattedBlockedUri(violationReport.BlockedUri);

        if (!string.IsNullOrWhiteSpace(blockedUri)
         && !string.IsNullOrWhiteSpace(violationReport.ViolatedDirective))
        {
            await _repository.SaveAsync(blockedUri, violationReport.ViolatedDirective);
        }
    }

    private static string GetFormattedBlockedUri(string blockedUri)
    {
        foreach(var source in CspConstants.AllSources)
        {
            var cleanSource = source.Replace(":", string.Empty).Replace("'", string.Empty);
            var cleanSourceNoUnsafe = cleanSource.Replace("unsafe-", string.Empty);

            if (string.Equals(blockedUri, cleanSource, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(blockedUri, cleanSourceNoUnsafe, StringComparison.OrdinalIgnoreCase))
            {
                return source;
            }
        }

        if (Uri.IsWellFormedUriString(blockedUri, UriKind.Absolute))
        {
            return new Uri(blockedUri).GetLeftPart(UriPartial.Path);
        }

        return string.Empty;
    }
}