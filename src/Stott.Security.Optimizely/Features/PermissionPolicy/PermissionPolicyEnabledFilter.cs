namespace Stott.Security.Optimizely.Features.PermissionPolicy;

public enum PermissionPolicyEnabledFilter
{
    All = 0,
    AllEnabled = 1,
    Disabled = 2,
    AllSites = 3,
    ThisSite = 4,
    ThisAndSpecificSites = 5,
    SpecificSites = 6
}
