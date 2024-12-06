using System.Collections.Generic;

namespace Stott.Security.Optimizely.Features.PermissionPolicy;

public sealed class PermissionPolicyDirectiveModel
{
    public string? Name { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public string? EnabledState { get; set; }

    public List<string> Sources { get; set; } = new List<string>();
}
