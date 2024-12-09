using System;
using System.Collections.Generic;

namespace Stott.Security.Optimizely.Features.PermissionPolicy;

public sealed class PermissionPolicyDirectiveModel
{
    public string? Name { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public string? EnabledState { get; set; }

    public List<PermissionPolicyUrl> Sources { get; set; } = new List<PermissionPolicyUrl>();
}

public sealed class PermissionPolicyUrl
{
    public Guid Id { get; set; }

    public string? Url { get; set; }
}
