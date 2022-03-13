using System.Collections.Generic;

namespace Stott.Optimizely.Csp.Features.Reporting.Repository
{
    public interface ICspViolationReportRepository
    {
        void Save(ReportModel violationReport);

        IList<ViolationReportSummary> GetReport();
    }
}
