using System;

using EPiServer.Core;

namespace Stott.Security.Optimizely.Features.Route;

public class SecurityRouteData
{
    public IContent? Content { get; set; }

    public SecurityRouteType RouteType { get; set; }

    /// <summary>
    /// The Optimizely SiteDefinition ID associated with the current request, or null for
    /// requests that cannot be mapped to a configured site (in which case Global settings apply).
    /// </summary>
    public Guid? SiteId { get; set; }

    /// <summary>
    /// The sanitised host name associated with the current request, or null when no host-specific
    /// scope applies.
    /// </summary>
    public string? HostName { get; set; }
}
