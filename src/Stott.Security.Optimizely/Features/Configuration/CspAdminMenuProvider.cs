namespace Stott.Security.Optimizely.Features.Configuration;

using System.Collections.Generic;

using EPiServer.Shell.Navigation;

using Stott.Security.Optimizely.Common;

[MenuProvider]
public class RobotsAdminMenuProvider : IMenuProvider
{
    public IEnumerable<MenuItem> GetMenuItems()
    {
        var parentMenuItem = new UrlMenuItem("Security", "/global/cms/stott.security.optimizely", "/stott.security.optimizely/settings/content-security-policy/")
        {
            IsAvailable = context => true,
            SortIndex = SortIndex.Last + 1,
            AuthorizationPolicy = CspConstants.AuthorizationPolicy
        };

        var cspMenuItem = new UrlMenuItem("Content Security Policy", "/global/cms/stott.security.optimizely/csp", "/stott.security.optimizely/settings/content-security-policy/")
        {
            IsAvailable = context => true,
            SortIndex = SortIndex.Last + 2,
            AuthorizationPolicy = CspConstants.AuthorizationPolicy
        };

        var securityMenuItem = new UrlMenuItem("Headers", "/global/cms/stott.security.optimizely/headers", "/stott.security.optimizely/settings/headers")
        {
            IsAvailable = context => true,
            SortIndex = SortIndex.Last + 3,
            AuthorizationPolicy = CspConstants.AuthorizationPolicy
        };

        var auditMenuItem = new UrlMenuItem("Audit History", "/global/cms/stott.security.optimizely/audithistory", "/stott.security.optimizely/settings/audit-history/")
        {
            IsAvailable = context => true,
            SortIndex = SortIndex.Last + 4,
            AuthorizationPolicy = CspConstants.AuthorizationPolicy
        };

        return new List<MenuItem> { parentMenuItem, cspMenuItem, securityMenuItem, auditMenuItem };
    }
}
