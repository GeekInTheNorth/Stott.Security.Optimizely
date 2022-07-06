using System.Collections.Generic;

using EPiServer.Authorization;
using EPiServer.Shell.Navigation;

namespace Stott.Security.Optimizely.Features.Configuration
{
    [MenuProvider]
    public class RobotsAdminMenuProvider : IMenuProvider
    {
        public IEnumerable<MenuItem> GetMenuItems()
        {
            var parentMenuItem = new UrlMenuItem("Security", "/global/cms/stott.security.core", "/CspLandingPage/Index")
            {
                IsAvailable = context => true,
                SortIndex = SortIndex.Last + 1,
                AuthorizationPolicy = CmsPolicyNames.CmsAdmin
            };

            var cspMenuItem = new UrlMenuItem("Content Security Policy", "/global/cms/stott.security.core/csp", "/CspLandingPage/Index")
            {
                IsAvailable = context => true,
                SortIndex = SortIndex.Last + 2,
                AuthorizationPolicy = CmsPolicyNames.CmsAdmin
            };

            var securityMenuItem = new UrlMenuItem("Headers", "/global/cms/stott.security.core/headers", "/CspLandingPage/Headers")
            {
                IsAvailable = context => true,
                SortIndex = SortIndex.Last + 3,
                AuthorizationPolicy = CmsPolicyNames.CmsAdmin
            };

            return new List<MenuItem> { parentMenuItem, cspMenuItem, securityMenuItem };
        }
    }
}
