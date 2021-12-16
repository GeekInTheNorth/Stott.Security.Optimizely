namespace OptimizelyTwelveTest.Features.Blocks.RichText
{
    using EPiServer.Web.Mvc;
    using Microsoft.AspNetCore.Mvc;

    public class RichTextBlockViewComponent : BlockComponent<RichTextBlock>
    {
        protected override IViewComponentResult InvokeComponent(RichTextBlock currentContent)
        {
            return View(currentContent);
        }
    }
}