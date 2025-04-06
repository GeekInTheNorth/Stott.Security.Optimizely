namespace Stott.Security.Optimizely.Features.Pages;

using System.ComponentModel.DataAnnotations;

using EPiServer.Shell.ObjectEditing;
using Stott.Security.Optimizely.Features.Csp;

public sealed class PageCspSourceMapping : ICspSourceMapping
{
    [Required]
    [Display(
        Name = "Source", 
        Description = "The source to grant permissions to.  This must be a valid Url domain potentially using * as a wildcard or one of blob:, data:, filesystem:, http:, https:, mediastream:, 'self', 'unsafe-eval', 'wasm-unsafe-eval', 'unsafe-hashes', 'unsafe-inline' or 'none'.")]
    [RegularExpression(
        "^(([a-z0-9\\/\\-\\._\\:\\*\\[\\]\\@]{3,}\\.{1}[a-z0-9\\/\\-\\._\\:\\*\\[\\]\\@]{2,})|([a-z]{2,5}\\:{1}\\/\\/localhost\\:([0-9]{1,5}|\\*{1}))|(blob:|data:|filesystem:|ws:|wss:|http:|https:|mediastream:|'self'|'unsafe-eval'|'wasm-unsafe-eval'|'unsafe-hashes'|'unsafe-inline'|'none'))$",
        ErrorMessage = "Source must be a valid Url domain potentially using * as a wildcard or one of blob:, data:, filesystem:, http:, https:, mediastream:, 'self', 'unsafe-eval', 'wasm-unsafe-eval', 'unsafe-hashes', 'unsafe-inline' or 'none'.")]
    public string? Source { get; set; }

    [Required]
    [Display(Name = "Directives", Description = "The functions that this source will be allowed to perform.")]
    [SelectMany(SelectionFactoryType = typeof(CspDirectiveSelectionFactory))]
    public string? Directives { get; set; }
}