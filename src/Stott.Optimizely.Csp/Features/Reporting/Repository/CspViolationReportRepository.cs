using System;
using System.Collections.Generic;
using System.Linq;

using EPiServer.Data;
using EPiServer.Data.Dynamic;

using Stott.Optimizely.Csp.Entities;

namespace Stott.Optimizely.Csp.Features.Reporting.Repository
{
    public class CspViolationReportRepository : ICspViolationReportRepository
    {
        private readonly DynamicDataStore _cspViolationReport;

        public CspViolationReportRepository(DynamicDataStoreFactory dataStoreFactory)
        {
            _cspViolationReport = dataStoreFactory.CreateStore(typeof(CspViolationReport));
        }

        public void Save(ReportModel violationReport)
        {
            if (violationReport == null)
            {
                return;
            }

            var recordToSave = new CspViolationReport
            {
                Id = Identity.NewIdentity(),
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

            _cspViolationReport.Save(recordToSave);
        }

        public IList<ViolationReportSummary> GetReport(DateTime threshold)
        {
            var query = (from violationReport in _cspViolationReport.Items<CspViolationReport>()
                         where violationReport.Reported >= threshold
                         select violationReport).ToList();

            return query.GroupBy(x => new { x.BlockedUri, x.ViolatedDirective })
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

        public int Delete(DateTime threshold)
        {
            var itemsDeleted = 0;
            var itemsToDelete = (from violationReport in _cspViolationReport.Items<CspViolationReport>()
                                 where violationReport.Reported < threshold
                                 select violationReport).ToList();

            foreach(var itemToDelete in itemsToDelete)
            {
                _cspViolationReport.Delete(itemToDelete.Id);
                itemsDeleted++;
            }

            return itemsDeleted;
        }
    }
}
