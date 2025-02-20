namespace Stott.Security.Optimizely.Features.PermissionPolicy;

public enum PermissionPolicyEnabledFilter
{
    All = 0,
    AllEnabled = 1,
    AllDisabled = 2,
    None = 3,
    AllSites = 4,
    ThisSite = 5,
    SpecificSites = 6
}
