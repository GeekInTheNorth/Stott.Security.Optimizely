using System.Collections.Generic;

namespace Stott.Security.Optimizely.Features.Tools;

public sealed class CspSourceModel
{
    public string? Source { get; set; }

    public List<string>? Directives { get; set; }
}