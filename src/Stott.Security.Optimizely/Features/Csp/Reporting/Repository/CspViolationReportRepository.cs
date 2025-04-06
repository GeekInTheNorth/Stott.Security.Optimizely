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

    public async Task SaveAsync(string blockedUri, string violatedDirective)
    {
        if (string.IsNullOrWhiteSpace(blockedUri) || string.IsNullOrWhiteSpace(violatedDirective))
        {
            return;
        }

        var lastReportedParameter = new SqlParameter("@lastReported", DateTime.UtcNow);
        var blockedUriParameter = new SqlParameter("@blockedUri", blockedUri);
        var violatedDirctiveParameter = new SqlParameter("@violatedDirective", violatedDirective);

        var itemsUpdated = await _context.Value.ExecuteSqlAsync(UpdateSql, lastReportedParameter, blockedUriParameter, violatedDirctiveParameter);
        if (itemsUpdated == 0)
        {
            // No record existed to be updated for this violation, so create it.
            _context.Value.CspViolations.Add(new CspViolationSummary
            {
                LastReported = DateTime.UtcNow,
                BlockedUri = blockedUri,
                ViolatedDirective = violatedDirective,
                Instances = 1,
            });

            await _context.Value.SaveChangesAsync();
        }
    }

    public async Task<IList<ViolationReportSummary>> GetReportAsync(string? source, string? directive, DateTime threshold)
    {
        // Groups violations by BlockedUri and Violated Directive and gets the latest stats.
        var violations = await (from violation in _context.Value.CspViolations.AsNoTracking()
                                group violation by new
                                {
                                    violation.BlockedUri,
                                    violation.ViolatedDirective
                                } into violationGroup
                                select new
                                {
                                    Source = violationGroup.Key.BlockedUri,
                                    Directive = violationGroup.Key.ViolatedDirective,
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
                         .Select((x, i) => new ViolationReportSummary(i, x.Source, x.Directive, x.Violations, x.LastViolated))
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