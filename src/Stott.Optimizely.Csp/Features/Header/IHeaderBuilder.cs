using System.Collections.Generic;

using Stott.Optimizely.Csp.Entities;

namespace Stott.Optimizely.Csp.Features.Header
{
    public interface IHeaderBuilder
    {
        IHeaderBuilder WithSources(IEnumerable<CspSource> sources);

        IHeaderBuilder WithReporting(bool sendViolationReport, string violationReportUrl);

        string Build();
    }
}
