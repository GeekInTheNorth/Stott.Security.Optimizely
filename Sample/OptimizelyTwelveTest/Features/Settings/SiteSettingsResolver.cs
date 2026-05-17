using EPiServer;
using EPiServer.Core;
using OptimizelyTwelveTest.Features.Home;

namespace OptimizelyTwelveTest.Features.Settings;

public class SiteSettingsResolver(IContentLoader contentLoader) : ISiteSettingsResolver
{
    private ISiteSettings _siteSettings;
    
    public ISiteSettings Get()
    {
        if (_siteSettings == null)
        {
            if (contentLoader.TryGet<HomePage>(ContentReference.StartPage, out var homePage) &&
                contentLoader.TryGet<SiteSettingsPage>(homePage.SiteSettings, out var siteSettingsPage))
            {
                _siteSettings = siteSettingsPage;
            }
        }

        return _siteSettings;
    }
}