namespace Stott.Security.Optimizely.Features.Header;

using System.Collections.Generic;

using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.Sandbox;

public interface ICspContentBuilder
{
    ICspContentBuilder WithSettings(CspSettings cspSettings);

    ICspContentBuilder WithSandbox(SandboxModel cspSandbox);

    ICspContentBuilder WithSources(IEnumerable<ICspSourceMapping> sources);

    ICspContentBuilder WithReporting(bool sendViolationReport);

    string BuildAsync();
}