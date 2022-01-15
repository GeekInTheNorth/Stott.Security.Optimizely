using System.Collections.Generic;

using Stott.Optimizely.Csp.Entities;

namespace Stott.Optimizely.Csp.Features.Header
{
    public interface ICspContentBuilder
    {
        ICspContentBuilder WithSources(IEnumerable<CspSource> sources);

        ICspContentBuilder WithReporting(bool sendViolationReport, string violationReportUrl);

        string Build();
    }
}
