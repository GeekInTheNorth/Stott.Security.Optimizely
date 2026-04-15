namespace Stott.Security.Optimizely.Features.Csp.Reporting.Repository;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.Csp.Reporting;

internal sealed class CspViolationReportRepository : ICspViolationReportRepository
{
    private readonly Lazy<ICspDataContext> _context;

    // Common Table Expressions allow us to update just the first row found
    // So should multiple row exist for the same combination, only one will be updated.
    // Preventing duplication of statistics.
    // By using SQL, we can potentially perform one trip to the DB instead of two.
    private const string UpdateSql =
        @"WITH UpdateList_CTE AS
              (
                SELECT TOP 1 *
                FROM [tbl_CspViolationSummary]
                WHERE [BlockedUri] = @blockedUri
                  AND [ViolatedDirective] = @violatedDirective
                  AND (@siteId IS NULL AND [SiteId] IS NULL OR [SiteId] = @siteId)
                  AND (@hostName IS NULL AND [HostName] IS NULL OR [HostName] = @hostName)
                ORDER BY [LastReported] DESC
              )
              UPDATE UpdateList_CTE
                 SET [Instances] = [Instances] + 1,
                     [LastReported] = @lastReported;";

    // By using SQL, we don't have to load records we want to delete, reducing the trips to the DB.
    private const string DeleteSql = "DELETE FROM [tbl_CspViolationSummary] WHERE [LastReported] <= @threshold";

    public CspViolationReportRepository(Lazy<ICspDataContext> context)
    {
        _context = context;
    }

    public async Task SaveAsync(string blockedUri, string violatedDirective, Guid? siteId, string? hostName)
    {
        if (string.IsNullOrWhiteSpace(blockedUri) || string.IsNullOrWhiteSpace(violatedDirective))
        {
            return;
        }

        var normalisedSiteId = siteId == Guid.Empty ? null : siteId;
        var normalisedHostName = string.IsNullOrWhiteSpace(hostName) ? null : hostName;

        var lastReportedParameter = new SqlParameter("@lastReported", DateTime.UtcNow);
        var blockedUriParameter = new SqlParameter("@blockedUri", blockedUri);
        var violatedDirctiveParameter = new SqlParameter("@violatedDirective", violatedDirective);
        var siteIdParameter = new SqlParameter("@siteId", (object?)normalisedSiteId ?? DBNull.Value);
        var hostNameParameter = new SqlParameter("@hostName", (object?)normalisedHostName ?? DBNull.Value);

        var itemsUpdated = await _context.Value.ExecuteSqlAsync(UpdateSql, lastReportedParameter, blockedUriParameter, violatedDirctiveParameter, siteIdParameter, hostNameParameter);
        if (itemsUpdated == 0)
        {
            // No record existed to be updated for this violation, so create it.
            _context.Value.CspViolations.Add(new CspViolationSummary
            {
                LastReported = DateTime.UtcNow,
                BlockedUri = blockedUri,
                ViolatedDirective = violatedDirective,
                SiteId = normalisedSiteId,
                HostName = normalisedHostName,
                Instances = 1,
            });

            await _context.Value.SaveChangesAsync();
        }
    }

    public async Task<IList<ViolationReportSummary>> GetReportAsync(string? source, string? directive, DateTime threshold, Guid? siteId, string? hostName)
    {
        var normalisedSiteId = siteId == Guid.Empty ? null : siteId;
        var normalisedHostName = string.IsNullOrWhiteSpace(hostName) ? null : hostName;

        var query = _context.Value.CspViolations.AsNoTracking().AsQueryable();

        // Filter by site context:
        // - When both siteId and hostName are provided, return only that specific host's violations
        // - When only siteId is provided, return violations for that site (any host)
        // - When neither is provided (Global), return all violations
        if (normalisedSiteId.HasValue && normalisedHostName != null)
        {
            query = query.Where(x => x.SiteId == normalisedSiteId && x.HostName == normalisedHostName);
        }
        else if (normalisedSiteId.HasValue)
        {
            query = query.Where(x => x.SiteId == normalisedSiteId);
        }

        // Groups violations by BlockedUri, Violated Directive, SiteId and HostName and gets the latest stats.
        var violations = await (from violation in query
                                group violation by new
                                {
                                    violation.BlockedUri,
                                    violation.ViolatedDirective,
                                    violation.SiteId,
                                    violation.HostName
                                } into violationGroup
                                select new
                                {
                                    Id = violationGroup.Select(x => x.Id).First(),
                                    Source = violationGroup.Key.BlockedUri,
                                    Directive = violationGroup.Key.ViolatedDirective,
                                    violationGroup.Key.SiteId,
                                    violationGroup.Key.HostName,
                                    Violations = violationGroup.Sum(y => y.Instances),
                                    LastViolated = violationGroup.Max(y => y.LastReported)
                                }).ToListAsync();

        if (!string.IsNullOrWhiteSpace(source))
        {
            violations = violations.Where(x => x.Source is not null && x.Source.Contains(source, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        if (!string.IsNullOrWhiteSpace(directive))
        {
            violations = violations.Where(x => x.Directive is not null && x.Directive.Equals(directive, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        // Convert to a model collection with a unique Id per row.
        return violations.Where(x => x.LastViolated >= threshold)
                         .Select(x => new ViolationReportSummary(x.Id, x.Source, x.Directive, x.SiteId, x.HostName, x.Violations, x.LastViolated))
                         .OrderByDescending(x => x.LastViolated)
                         .ToList();
    }

    public async Task<int> DeleteAsync(DateTime threshold)
    {
        var thresholdParameter = new SqlParameter("@threshold", threshold);
        var itemsDeleted = await _context.Value.ExecuteSqlAsync(DeleteSql, thresholdParameter);

        return itemsDeleted;
    }
}