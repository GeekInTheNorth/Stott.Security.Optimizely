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
            var listMenuItem = new UrlMenuItem("CSP", "/global/cms/admin/stott.optimizely.csp", "/CspLandingPage/Index")
            {
                IsAvailable = context => true,
                SortIndex = SortIndex.Last + 1,
                AuthorizationPolicy = CmsPolicyNames.CmsAdmin
            };

            return new List<MenuItem> { listMenuItem };
        }
    }
}
