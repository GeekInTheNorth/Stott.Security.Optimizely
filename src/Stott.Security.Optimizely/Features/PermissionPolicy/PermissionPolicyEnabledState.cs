namespace Stott.Security.Optimizely.Features.PermissionPolicy;

public enum PermissionPolicyEnabledState
{
    None = 0,
    All = 1,
    ThisSite = 2,
    ThisAndSpecificSites = 3,
    SpecificSites = 4
}