namespace OptimizelyTwelveTest.Features.MetaData
{
    using EPiServer.Web.Routing;

    using Microsoft.AspNetCore.Mvc;

    using OptimizelyTwelveTest.Features.Common.Pages;
    using OptimizelyTwelveTest.Features.Settings;

    public class MetaDataViewComponent : ViewComponent
    {
        private readonly ISiteSettings _siteSettings;
        private readonly UrlResolver _urlResolver;

        public MetaDataViewComponent(ISiteSettings siteSettings, UrlResolver urlResolver)
        {
            _siteSettings = siteSettings;
            _urlResolver = urlResolver;
        }

        public IViewComponentResult Invoke(ISitePageData sitePage)
        {
            var model = new MetaDataViewModel
            {
                Title = $"{_siteSettings?.SiteName} | {sitePage.MetaTitle}",
                Description = sitePage.MetaText,
                Image = _urlResolver.GetUrl(sitePage.MetaImage)
            };

            return View(model);
        }
    }
}
