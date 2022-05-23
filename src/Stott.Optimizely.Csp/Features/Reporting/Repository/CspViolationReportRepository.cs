using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

using Stott.Optimizely.Csp.Entities;

namespace Stott.Optimizely.Csp.Features.Reporting.Repository
{
    public class CspViolationReportRepository : ICspViolationReportRepository
    {
        private readonly ICspDataContext _context;

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
            var sql = "UPDATE [tbl_CspViolationSummary] SET [Instances] = [Instances] + 1, [LastReported] = @lastReported WHERE [BlockedUri] = @blockedUri AND [ViolatedDirective] = @violatedDirective";
            var lastReportedParameter = new SqlParameter("@lastReported", DateTime.UtcNow);
            var blockedUriParameter = new SqlParameter("@blockedUri", blockedUri.GetLeftPart(UriPartial.Path));
            var violatedDirctiveParameter = new SqlParameter("@violatedDirective", violationReport.ViolatedDirective);

            var itemsUpdated = await _context.Database.ExecuteSqlRawAsync(sql, lastReportedParameter, blockedUriParameter, violatedDirctiveParameter);
            if (itemsUpdated == 0)
            {
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
            var sql = "DELETE FROM [tbl_CspViolationReport] WHERE [LastReported] <= @threshold";
            var thresholdParameter = new SqlParameter("@threshold", threshold);
            
            _context.Database.SetCommandTimeout(TimeSpan.FromSeconds(90));
            var itemsDeleted = await _context.Database.ExecuteSqlRawAsync(sql, thresholdParameter);

            return itemsDeleted;
        }
    }
}
