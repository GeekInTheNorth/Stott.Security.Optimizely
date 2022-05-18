using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
                                           .AsQueryable()
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
            var itemsToDelete = await _context.CspViolations
                                              .AsQueryable()
                                              .Where(x => x.Reported >= threshold)
                                              .ToListAsync();
            var itemsDeleted = itemsToDelete.Count;

            _context.CspViolations.RemoveRange(itemsToDelete);
            await _context.SaveChangesAsync();

            return itemsDeleted;
        }
    }
}
