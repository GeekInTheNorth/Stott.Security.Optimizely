namespace Stott.Security.Optimizely.Features.Whitelist;

using System.Collections.Generic;

public sealed class WhitelistEntry
{
    public string? SourceUrl { get; set; }

    public List<string>? Directives { get; set; }
}
