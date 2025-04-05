namespace OptiNetSix.Features.Blocks.Embed;

using System.ComponentModel.DataAnnotations;

using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Web;

[ContentType(
    DisplayName = "Embed Block",
    GUID = "947e1f1d-54ad-4f3b-ba80-3741d2523ea7",
    Description = "",
    GroupName = SystemTabNames.Content)]
public class EmbedBlock : BlockData
{
    [CultureSpecific]
    [Display(
        Name = "HTML Content",
        Description = "HTML Content to be rendered raw on the page.",
        GroupName = SystemTabNames.Content,
        Order = 1)]
    [UIHint(UIHint.Textarea)]
    public virtual string Html { get; set; }
}