namespace Stott.Security.Core.Features.Header;

using System.Collections.Generic;

using Stott.Security.Core.Entities;

public interface ICspContentBuilder
{
    ICspContentBuilder WithSources(IEnumerable<CspSource> sources);

    ICspContentBuilder WithReporting(bool sendViolationReport, string violationReportUrl);

    string BuildAsync();
}
