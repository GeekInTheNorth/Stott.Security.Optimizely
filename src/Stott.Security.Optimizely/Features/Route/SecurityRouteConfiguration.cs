using System.Collections.Generic;

namespace Stott.Security.Optimizely.Features.Route;

public sealed class SecurityRouteConfiguration
{
    public IList<string> ExclusionPaths { get; set; } = new List<string>();
}