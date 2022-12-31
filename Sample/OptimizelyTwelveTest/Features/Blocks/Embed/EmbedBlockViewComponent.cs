namespace OptimizelyTwelveTest.Features.Blocks.Embed;

using EPiServer.Web.Mvc;

using Microsoft.AspNetCore.Mvc;

public class EmbedBlockViewComponent : BlockComponent<EmbedBlock>
{
    protected override IViewComponentResult InvokeComponent(EmbedBlock currentContent)
    {
        return View(currentContent);
    }
}