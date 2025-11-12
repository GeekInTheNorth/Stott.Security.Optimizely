namespace OptimizelyTwelveTest.Features.Common.Media
{
    using EPiServer.Web.Mvc;
    using Microsoft.AspNetCore.Mvc;

    public class ImageContentViewComponent : PartialContentComponent<ImageContent>
    {
        protected override IViewComponentResult InvokeComponent(ImageContent currentContent)
        {
            return View(currentContent);
        }
    }
}
