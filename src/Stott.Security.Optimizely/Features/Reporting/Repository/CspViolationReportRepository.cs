using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

using Stott.Security.Optimizely.Entities;

namespace Stott.Security.Optimizely.Features.Reporting.Repository
{
    public class CspViolationReportRepository : ICspViolationReportRepository
    {
        private readonly ICspDataContext _context;

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

        public CspViolationReportRepository(ICspDataContext context)
        {
            _context = context;
        }

        public async Task SaveAsync(ReportModel violationReport)
        {
            if (violationReport == null)
            {
                return;
            }

            var blockedUri = new Uri(violationReport.BlockedUri);
            var lastReportedParameter = new SqlParameter("@lastReported", DateTime.UtcNow);
            var blockedUriParameter = new SqlParameter("@blockedUri", blockedUri.GetLeftPart(UriPartial.Path));
            var violatedDirctiveParameter = new SqlParameter("@violatedDirective", violationReport.ViolatedDirective);

            var itemsUpdated = await _context.ExecuteSqlAsync(UpdateSql, lastReportedParameter, blockedUriParameter, violatedDirctiveParameter);
            if (itemsUpdated == 0)
            {
                // No record existed to be updated for this violation, so create it.
                _context.CspViolations.Add(new CspViolationSummary
                {
                    LastReported = DateTime.UtcNow,
                    BlockedUri = blockedUri.GetLeftPart(UriPartial.Path),
                    ViolatedDirective = violationReport.ViolatedDirective,
                    Instances = 1,
                });

                await _context.SaveChangesAsync();
            }
        }

        public async Task<IList<ViolationReportSummary>> GetReportAsync(DateTime threshold)
        {
            // Groups violations by BlockedUri and Violated Directive and gets the latest stats.
            var violations = await (from violation in _context.CspViolations.AsNoTracking()
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

            // Convert to a model collection with a unique Id per row.
            return violations.Select((x, i) => new ViolationReportSummary
            {
                Key = i,
                Source = x.Source,
                Directive = x.Directive,
                Violations = x.Violations,
                LastViolated = x.LastViolated
            })
                             .OrderByDescending(x => x.LastViolated)
                             .ToList();
        }

        public async Task<int> DeleteAsync(DateTime threshold)
        {
            var thresholdParameter = new SqlParameter("@threshold", threshold);
            var itemsDeleted = await _context.ExecuteSqlAsync(DeleteSql, thresholdParameter);

            return itemsDeleted;
        }
    }
}
