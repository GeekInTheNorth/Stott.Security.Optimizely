namespace OptimizelyTwelveTest.ServiceExtensions
{
    using EPiServer;
    using EPiServer.Core;

    using OptimizelyTwelveTest.Features.Home;
    using OptimizelyTwelveTest.Features.Settings;

    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceExtensions
    {
        public static void AddCustomDependencies(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<ISiteSettings>(provider =>
            {
                if (provider.GetService(typeof(IContentLoader)) is IContentLoader contentLoader &&
                    contentLoader.TryGet<HomePage>(ContentReference.StartPage, out var homePage) &&
                    contentLoader.TryGet<SiteSettingsPage>(homePage.SiteSettings, out var siteSettingsPage))
                {
                    return siteSettingsPage;
                }

                return null;
            });
        }
    }
}
