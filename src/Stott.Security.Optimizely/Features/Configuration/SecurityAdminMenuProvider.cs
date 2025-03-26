namespace Stott.Security.Optimizely.Features.Configuration;

using System.Collections.Generic;

using EPiServer.Shell.Navigation;

using Stott.Security.Optimizely.Common;

[MenuProvider]
public sealed class SecurityAdminMenuProvider : IMenuProvider
{
    public IEnumerable<MenuItem> GetMenuItems()
    {
        yield return CreateMenuItem("Security", "/global/cms/stott.security.optimizely", "/stott.security.optimizely/administration/", SortIndex.Last + 1);
        yield return CreateMenuItem("CSP Settings", "/global/cms/stott.security.optimizely/csp.settings", "/stott.security.optimizely/administration/#csp-settings", SortIndex.Last + 2);
        yield return CreateMenuItem("CSP Sources", "/global/cms/stott.security.optimizely/csp.source", "/stott.security.optimizely/administration/#csp-source", SortIndex.Last + 3);
        yield return CreateMenuItem("CSP Sandbox", "/global/cms/stott.security.optimizely/csp.sandbox", "/stott.security.optimizely/administration/#csp-sandbox", SortIndex.Last + 4);
        yield return CreateMenuItem("CSP Violations", "/global/cms/stott.security.optimizely/csp.violations", "/stott.security.optimizely/administration/#csp-violations", SortIndex.Last + 5);
        yield return CreateMenuItem("CORS Settings", "/global/cms/stott.security.optimizely/cors.settings", "/stott.security.optimizely/administration/#cors-settings", SortIndex.Last + 6);
        yield return CreateMenuItem("Permissions Policy", "/global/cms/stott.security.optimizely/permission.policy", "/stott.security.optimizely/administration/#permissions-policy", SortIndex.Last + 7);
        yield return CreateMenuItem("Response Headers", "/global/cms/stott.security.optimizely/all.security.headers", "/stott.security.optimizely/administration/#all-security-headers", SortIndex.Last + 8);
        yield return CreateMenuItem("Header Preview", "/global/cms/stott.security.optimizely/header.preview", "/stott.security.optimizely/administration/#header-preview", SortIndex.Last + 9);
        yield return CreateMenuItem("Audit", "/global/cms/stott.security.optimizely/audit", "/stott.security.optimizely/administration/#audit-history", SortIndex.Last + 10);
        yield return CreateMenuItem("Tools", "/global/cms/stott.security.optimizely/tools", "/stott.security.optimizely/administration/#tools", SortIndex.Last + 11);
    }

    private static UrlMenuItem CreateMenuItem(string name, string path, string url, int index)
    {
        return new UrlMenuItem(name, path, url)
        {
            IsAvailable = context => true,
            SortIndex = index,
            AuthorizationPolicy = CspConstants.AuthorizationPolicy
        };
    }
}