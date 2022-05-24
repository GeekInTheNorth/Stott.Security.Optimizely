using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Stott.Optimizely.Csp.Features.Reporting.Repository
{
    public interface ICspViolationReportRepository
    {
        Task SaveAsync(ReportModel violationReport);

        Task<IList<ViolationReportSummary>> GetReportAsync(DateTime threshold);

        Task<int> DeleteAsync(DateTime threshold);
    }
}
