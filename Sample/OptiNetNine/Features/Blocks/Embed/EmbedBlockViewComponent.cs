namespace OptiNetNine.Features.Blocks.Embed;

using EPiServer.Framework.ClientResources;
using EPiServer.Web.Mvc;

using Microsoft.AspNetCore.Mvc;

public class EmbedBlockViewComponent : BlockComponent<EmbedBlock>
{
    private readonly ICspNonceService _cspNonceService;

    public EmbedBlockViewComponent(ICspNonceService cspNonceService)
    {
        _cspNonceService = cspNonceService;
    }

    protected override IViewComponentResult InvokeComponent(EmbedBlock currentContent)
    {
        var nonce = _cspNonceService.GetNonce();

        var viewModel = new EmbedBlockViewModel
        {
            EmbedContent = AddNonceToTags(currentContent.Html, nonce)
        };

        return View(viewModel);
    }

    private static string AddNonceToTags(string html, string nonce)
    {
        if (string.IsNullOrWhiteSpace(html))
        {
            return string.Empty;
        }

        if (string.IsNullOrWhiteSpace(nonce))
        {
            return html ?? string.Empty;
        }

        var doc = new HtmlAgilityPack.HtmlDocument();
        doc.LoadHtml(html);

        var scriptTags = doc.DocumentNode.Descendants("script");
        foreach (var scriptTag in scriptTags)
        {
            scriptTag.SetAttributeValue("nonce", nonce);
        }

        var styleTags = doc.DocumentNode.Descendants("style");
        foreach (var styleTag in styleTags)
        {
            styleTag.SetAttributeValue("nonce", nonce);
        }

        return doc.DocumentNode.OuterHtml;
    }
}
