namespace Stott.Security.Optimizely.Features.Csp.Nonce;

using Microsoft.AspNetCore.Razor.TagHelpers;

[HtmlTargetElement("script", Attributes = "nonce")]
[HtmlTargetElement("style", Attributes = "nonce")]
public sealed class NonceTagHelper : TagHelper
{
    private readonly INonceProvider _nonceProvider;

    public override int Order => -9999;

    public NonceTagHelper(INonceProvider nonceProvider)
    {
        _nonceProvider = nonceProvider;
    }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        var nonce = _nonceProvider.GetNonce();
        if (!string.IsNullOrWhiteSpace(nonce))
        {
            output.Attributes.SetAttribute("nonce", _nonceProvider.GetNonce());
        }
        else if (output.Attributes.TryGetAttribute("nonce", out var attribute))
        {
            output.Attributes.Remove(attribute);
        }
    }
}