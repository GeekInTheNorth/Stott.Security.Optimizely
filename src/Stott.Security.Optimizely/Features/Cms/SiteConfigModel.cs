using System;
using System.Collections.Generic;

namespace Stott.Security.Optimizely.Features.Cms;

public sealed class SiteConfigModel
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public List<SiteHostModel> Hosts { get; set; } = new List<SiteHostModel>();
}
