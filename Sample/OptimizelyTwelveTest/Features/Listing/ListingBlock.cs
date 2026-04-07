using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;

namespace OptimizelyTwelveTest.Features.Listing;

[ContentType(
    DisplayName = "Listing Block",
    GUID = "3757f8f8-e1e9-4e2d-85a4-d6bd4a034675",
    GroupName = SystemTabNames.Content,
    CompositionBehaviors = [CompositionBehavior.SectionEnabledKey])]
public class ListingBlock : BlockData
{
    [CultureSpecific]
    [Display(
        Name = "Content",
        Description = "Rich Text Content",
        GroupName = SystemTabNames.Content,
        Order = 1)]
    public virtual IList<CategoryFilterBlock> Filters { get; set; }
}
