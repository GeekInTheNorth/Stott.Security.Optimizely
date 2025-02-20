namespace Stott.Security.Optimizely.Features.PermissionPolicy;

public enum PermissionPolicyEnabledState
{
    Disabled = 0,
    None = 1,
    All = 2,
    ThisSite = 3,
    ThisAndSpecificSites = 4,
    SpecificSites = 5
}