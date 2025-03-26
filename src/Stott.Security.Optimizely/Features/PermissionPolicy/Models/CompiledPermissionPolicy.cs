using System.Collections.Generic;

namespace Stott.Security.Optimizely.Features.PermissionPolicy.Models;

public sealed class CompiledPermissionPolicy
{
    public bool IsEnabled { get; set; }

    public List<string> Directives { get; set; } = new List<string>();
}
