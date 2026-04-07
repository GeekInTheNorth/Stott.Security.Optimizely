using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;

namespace OptimizelyTwelveTest.Features.Listing;

[ContentType(
    DisplayName = "Category Filter Block",
    Description = "Intended for defining a filter that the user can use to filter result.",
    GUID = "4CEE4A26-0929-4A0E-B39B-237176527CFA",
    Order = 110,
    GroupName = SystemTabNames.Content,
    AvailableInEditMode = false)]
public class CategoryFilterBlock : BlockData
{
    [Display(Name = "Title", GroupName = SystemTabNames.Content, Order = 10)]
    [Required]
    [CultureSpecific]
    public virtual string Title { get; set; }

    [Display(Name = "Placeholder", GroupName = SystemTabNames.Content, Order = 20)]
    [Required]
    [CultureSpecific]
    public virtual string Placeholder { get; set; }

    [Display(Name = "All Items Label", Description = "Label for the all items entry.", GroupName = SystemTabNames.Content, Order = 40)]
    [Required]
    [CultureSpecific]
    public virtual string AllLabel { get; set; }

    //[Display(Name = "Categories", Description = "Select categories to show in filter list.", GroupName = SystemTabNames.Content, Order = 30)]
    //[Required]
    //[CultureSpecific]
    //public virtual Category Categories { get; set; }
}
