using EPiServer.Core;

namespace Stott.Security.Optimizely.Features.Route;

public class SecurityRouteData
{
    public IContent? Content { get; set; }

    public SecurityRouteType RouteType { get; set; }
}
