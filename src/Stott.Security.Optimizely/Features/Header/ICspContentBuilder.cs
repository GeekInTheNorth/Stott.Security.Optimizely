namespace Stott.Security.Optimizely.Features.Header;

using System.Collections.Generic;
using Stott.Security.Optimizely.Features.Csp.Sandbox;
using Stott.Security.Optimizely.Features.Csp.Settings;

public interface ICspContentBuilder
{
    ICspContentBuilder WithSettings(ICspSettings cspSettings);

    ICspContentBuilder WithSandbox(SandboxModel cspSandbox);

    ICspContentBuilder WithSources(IEnumerable<ICspSourceMapping> sources);

    string BuildAsync();
}