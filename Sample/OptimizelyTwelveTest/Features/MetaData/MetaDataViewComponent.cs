namespace OptimizelyTwelveTest.Features.MetaData
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using EPiServer.VisualBuilder.Compositions;
    using EPiServer.Web;
    using EPiServer.Web.Mvc.TagHelpers;
    using EPiServer.Web.Mvc.TagHelpers.Experience.Internal;
    using EPiServer.Web.Routing;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Razor.TagHelpers;
    using OptimizelyTwelveTest.Features.Common.Pages;
    using OptimizelyTwelveTest.Features.Settings;

    public class MetaDataViewComponent(ISiteSettingsResolver siteSettingsResolver, IUrlResolver urlResolver) : ViewComponent
    {
        public IViewComponentResult Invoke(ISitePageData sitePage)
        {
            if (sitePage == null)
            {
                return Content(string.Empty);
            }

            var siteSettings = siteSettingsResolver.Get();
            var model = new MetaDataViewModel
            {
                Title = $"{siteSettings?.SiteName} | {sitePage.MetaTitle}",
                Description = sitePage.MetaText,
                Image = urlResolver.GetUrl(sitePage.MetaImage)
            };

            return View(model);
        }
    }
}
