namespace Stott.Security.Optimizely.Features.Csp.AllowList;

using System.Collections.Generic;

public sealed class AllowListEntry
{
    public string? SourceUrl { get; set; }

    public List<string>? Directives { get; set; }
}
