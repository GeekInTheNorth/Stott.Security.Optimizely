namespace Stott.Security.Optimizely.Features.PermissionPolicy.Models;

public sealed class PermissionPolicySettingsModel : IPermissionPolicySettings
{
    public bool IsEnabled { get; set; }

    public string? AppId { get; set; }

    public string? HostName { get; set; }
}
