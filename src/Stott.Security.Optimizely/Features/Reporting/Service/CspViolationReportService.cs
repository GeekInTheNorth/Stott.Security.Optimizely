namespace Stott.Security.Optimizely.Features.Reporting.Service;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using EPiServer.ServiceLocation;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Features.Reporting.Models;
using Stott.Security.Optimizely.Features.Reporting.Repository;

internal sealed class CspViolationReportService : ICspViolationReportService
{
    private ICspViolationReportRepository? _repository;

    private ICspViolationReportRepository Repository
    {
        get
        {
            _repository ??= ServiceLocator.Current.GetInstance<ICspViolationReportRepository>();

            return _repository;
        }
    }

    public CspViolationReportService(ICspViolationReportRepository repository)
    {
        _repository = repository;
    }

    public async Task<int> DeleteAsync(DateTime threshold)
    {
        return await Repository.DeleteAsync(threshold);
    }

    public async Task<IList<ViolationReportSummary>> GetReportAsync(string? source, string? directive, DateTime threshold)
    {
        return await Repository.GetReportAsync(source, directive, threshold);
    }

    public async Task SaveAsync(ICspReport violationReport)
    {
        var blockedUri = GetFormattedBlockedUri(violationReport.BlockedUri);
        var validDirective = GetValidDirective(violationReport.ViolatedDirective);

        if (!string.IsNullOrWhiteSpace(blockedUri)
         && !string.IsNullOrWhiteSpace(validDirective))
        {
            await Repository.SaveAsync(blockedUri, validDirective);
        }
    }

    private static string? GetValidDirective(string? providedDirective)
    {
        return CspConstants.AllDirectives.FirstOrDefault(x => string.Equals(x, providedDirective, StringComparison.OrdinalIgnoreCase));
    }

    private static string GetFormattedBlockedUri(string? blockedUri)
    {
        if (string.IsNullOrWhiteSpace(blockedUri))
        {
            return string.Empty;
        }

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
            var cleanUrl = new Uri(blockedUri).GetLeftPart(UriPartial.Path);

            if (cleanUrl is { Length: >255 })
            {
                cleanUrl = new Uri(blockedUri).GetLeftPart(UriPartial.Authority);
            }

            return cleanUrl is { Length: <= 255 } ? cleanUrl : string.Empty;
        }

        return string.Empty;
    }
}