using System;
using System.Collections.Generic;

namespace Stott.Security.Optimizely.Features.Sites;

public sealed class SiteViewModel
{
    public Guid SiteId { get; set; }

    public string? SiteName { get; set; }

    public List<SiteHostViewModel>? AvailableHosts { get; set; }

    /// <summary>
    /// True when the site has more than one configured host binding (excluding the implicit default).
    /// The UI uses this to decide whether to render the host drill-down in the context switcher.
    /// </summary>
    public bool HasMultipleHosts { get; set; }
}
