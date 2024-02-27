namespace Stott.Security.Optimizely.Features.Header;

using System.Collections.Generic;

using Stott.Security.Optimizely.Features.Sandbox;
using Stott.Security.Optimizely.Features.Settings;

public interface ICspContentBuilder
{
    ICspContentBuilder WithSettings(ICspSettings cspSettings);

    ICspContentBuilder WithSandbox(SandboxModel cspSandbox);

    ICspContentBuilder WithSources(IEnumerable<ICspSourceMapping> sources);

    string BuildAsync();
}