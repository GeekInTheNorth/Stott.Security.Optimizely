namespace OptimizelyTwelveTest.Features.GeneralContent
{
    using EPiServer.Web.Mvc;
    using Microsoft.AspNetCore.Mvc;

    public class GeneralContentPageViewComponent : PartialContentComponent<GeneralContentPage>
    {
        protected override IViewComponentResult InvokeComponent(GeneralContentPage currentContent)
        {
            return View(currentContent);
        }
    }
}