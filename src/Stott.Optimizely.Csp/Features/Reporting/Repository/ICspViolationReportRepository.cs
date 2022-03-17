using System;
using System.Collections.Generic;

namespace Stott.Optimizely.Csp.Features.Reporting.Repository
{
    public interface ICspViolationReportRepository
    {
        void Save(ReportModel violationReport);

        IList<ViolationReportSummary> GetReport(DateTime threshold);

        int Delete(DateTime threshold);
    }
}
