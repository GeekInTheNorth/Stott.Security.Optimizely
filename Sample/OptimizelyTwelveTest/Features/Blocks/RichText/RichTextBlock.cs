namespace OptimizelyTwelveTest.Features.Blocks.RichText;

using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using System.ComponentModel.DataAnnotations;

[ContentType(
    DisplayName = "Rich Text Block", 
    GUID = "ad51630a-45a9-40d6-ba24-1817e7c3cdd7", 
    Description = "", 
    GroupName = SystemTabNames.Content,
    CompositionBehaviors = [CompositionBehavior.ElementEnabledKey, CompositionBehavior.SectionEnabledKey])]
public class RichTextBlock : BlockData
{
    [Display(Name = "Css Class", Order = 2)]
    public virtual string CustomClass { get; set; }

    [CultureSpecific]
    [Display(
        Name = "Content",
        Description = "Rich Text Content",
        GroupName = SystemTabNames.Content,
        Order = 1)]
    public virtual XhtmlString Content { get; set; }
}