using System;

namespace Stott.Security.Optimizely.Features.PermissionPolicy.Models;

public sealed class PermissionPolicySettingsModel : IPermissionPolicySettings
{
    public bool IsEnabled { get; set; }

    public Guid? SiteId { get; set; }

    public string? HostName { get; set; }
}
