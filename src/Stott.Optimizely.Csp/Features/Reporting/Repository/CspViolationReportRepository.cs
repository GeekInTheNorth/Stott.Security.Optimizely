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

            var recordToSave = new CspViolationReport
            {
                Reported = DateTime.Now,
                BlockedUri = blockedUri.GetLeftPart(UriPartial.Path),
                BlockedQueryString = blockedUri.Query,
                Disposition = violationReport.Disposition,
                DocumentUri = violationReport.DocumentUri,
                EffectiveDirective = violationReport.EffectiveDirective,
                OriginalPolicy = violationReport.OriginalPolicy,
                Referrer = violationReport.Referrer,
                ScriptSample = violationReport.ScriptSample,
                SourceFile = violationReport.SourceFile,
                ViolatedDirective = violationReport.ViolatedDirective
            };

            _context.CspViolations.Add(recordToSave);

            await _context.SaveChangesAsync();
        }

        public async Task<IList<ViolationReportSummary>> GetReportAsync(DateTime threshold)
        {
            var violations = await (from violation in _context.CspViolations.AsNoTracking()
                                    where violation.Reported > threshold
                                    group violation by new
                                    {
                                        violation.BlockedUri,
                                        violation.ViolatedDirective
                                    } into violationGroup
                                    select new
                                    {
                                        Source = violationGroup.Key.BlockedUri,
                                        Directive = violationGroup.Key.ViolatedDirective,
                                        Violations = violationGroup.Count(),
                                        LastViolated = violationGroup.Max(y => y.Reported)
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
            var sql = "DELETE FROM [tbl_CspViolationReport] WHERE [Reported] <= @threshold";
            var thresholdParameter = new SqlParameter("@threshold", threshold);
            
            _context.Database.SetCommandTimeout(TimeSpan.FromSeconds(90));
            var itemsDeleted = await _context.Database.ExecuteSqlRawAsync(sql, thresholdParameter);

            return itemsDeleted;
        }
    }
}
