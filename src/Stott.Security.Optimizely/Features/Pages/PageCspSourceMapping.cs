namespace Stott.Security.Optimizely.Features.Pages;

using System.ComponentModel.DataAnnotations;

using EPiServer.Shell.ObjectEditing;

public sealed class PageCspSourceMapping
{
    [Required]
    [Display(Name = "Source", Description = "The source to grant permissions to.")]
    public string Source { get; set; }

    [Required]
    [Display(Name = "Directives", Description = "The functions that this source will be allowed to perform.")]
    [SelectMany(SelectionFactoryType = typeof(CspDirectiveSelectionFactory))]
    public string Directives { get; set; }
}