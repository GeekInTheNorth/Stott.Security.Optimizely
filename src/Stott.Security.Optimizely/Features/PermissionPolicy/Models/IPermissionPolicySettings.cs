using System;

namespace Stott.Security.Optimizely.Features.PermissionPolicy.Models;

public interface IPermissionPolicySettings
{
    bool IsEnabled { get; set; }

    Guid? SiteId { get; set; }

    string? HostName { get; set; }
}
