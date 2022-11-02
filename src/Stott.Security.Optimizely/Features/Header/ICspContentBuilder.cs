namespace Stott.Security.Optimizely.Features.Header;

using System.Collections.Generic;

using Stott.Security.Optimizely.Entities;

public interface ICspContentBuilder
{
    ICspContentBuilder WithSources(IEnumerable<CspSource> sources);

    ICspContentBuilder WithReporting(bool sendViolationReport, string violationReportUrl);

    string BuildAsync();
}
