namespace Stott.Security.Optimizely.Features.Configuration;

using System.Collections.Generic;

using EPiServer.Shell.Navigation;

using Stott.Security.Optimizely.Common;

[MenuProvider]
public sealed class RobotsAdminMenuProvider : IMenuProvider
{
    public IEnumerable<MenuItem> GetMenuItems()
    {
        var parentMenuItem = new UrlMenuItem("Security", "/global/cms/stott.security.optimizely", "/stott.security.optimizely/administration/")
        {
            IsAvailable = context => true,
            SortIndex = SortIndex.Last + 1,
            AuthorizationPolicy = CspConstants.AuthorizationPolicy
        };

        return new List<MenuItem> { parentMenuItem };
    }
}
