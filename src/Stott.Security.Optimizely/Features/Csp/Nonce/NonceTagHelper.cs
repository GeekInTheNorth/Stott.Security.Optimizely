namespace Stott.Security.Optimizely.Features.Csp.Nonce;

using Microsoft.AspNetCore.Razor.TagHelpers;

[HtmlTargetElement("script", Attributes = "nonce")]
[HtmlTargetElement("style", Attributes = "nonce")]
public sealed class NonceTagHelper(INonceProvider nonceProvider) : TagHelper
{
    public override int Order => -9999;

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        var nonce = nonceProvider.GetNonce();
        if (!string.IsNullOrWhiteSpace(nonce))
        {
            output.Attributes.SetAttribute("nonce", nonceProvider.GetNonce());
        }
        else if (output.Attributes.TryGetAttribute("nonce", out var attribute))
        {
            output.Attributes.Remove(attribute);
        }
    }
}