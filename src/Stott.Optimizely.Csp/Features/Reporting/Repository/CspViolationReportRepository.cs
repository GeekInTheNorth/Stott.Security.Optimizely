using System;

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
    }
}
