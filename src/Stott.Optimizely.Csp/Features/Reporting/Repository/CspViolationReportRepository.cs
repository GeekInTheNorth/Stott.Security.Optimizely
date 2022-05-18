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

            var recordToSave = new CspViolationReport
            {
                Reported = DateTime.Now,
                BlockedUri = violationReport.BlockedUri,
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
            var violations = await _context.CspViolations
                                           .AsNoTracking()
                                           .Where(x => x.Reported >= threshold)
                                           .ToListAsync();

            return violations.GroupBy(x => new { x.BlockedUri, x.ViolatedDirective })
                             .Select((x, i) => new ViolationReportSummary
                             {
                                 Key = i,
                                 Source = x.Key.BlockedUri,
                                 Directive = x.Key.ViolatedDirective,
                                 Violations = x.Count(),
                                 LastViolated = x.Max(y => y.Reported)
                             })
                             .OrderByDescending(x => x.LastViolated)
                             .ToList();
        }

        public async Task<int> DeleteAsync(DateTime threshold)
        {
            var sql = "DELETE FROM [tbl_CspViolationReport] WHERE [Reported] <= @threshold";
            var thresholdParameter = new SqlParameter("@threshold", threshold);
            
            _context.Database.SetCommandTimeout(TimeSpan.FromSeconds(200));
            var itemsDeleted = await _context.Database.ExecuteSqlRawAsync(sql, thresholdParameter);

            return itemsDeleted;
        }
    }
}
