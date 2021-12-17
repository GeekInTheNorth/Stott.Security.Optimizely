using System.Collections.Generic;

using EPiServer.Authorization;
using EPiServer.Shell.Navigation;

namespace Stott.Optimizely.Csp.Presentation
{
    [MenuProvider]
    public class RobotsAdminMenuProvider : IMenuProvider
    {
        public IEnumerable<MenuItem> GetMenuItems()
        {
            var listMenuItem = new UrlMenuItem("CSP", "/global/cms/admin/stott.optimizely.csp", "/CspAdmin/Index")
            {
                IsAvailable = context => true,
                SortIndex = SortIndex.Last + 1,
                AuthorizationPolicy = CmsPolicyNames.CmsAdmin
            };

            return new List<MenuItem> { listMenuItem };
        }
    }
}
