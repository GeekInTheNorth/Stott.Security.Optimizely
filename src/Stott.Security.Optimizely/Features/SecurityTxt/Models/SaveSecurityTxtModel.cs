using System;

namespace Stott.Security.Optimizely.Features.SecurityTxt;

public sealed class SaveSecurityTxtModel
{
    public Guid Id { get; set; }

    public Guid SiteId { get; set; }

    public string? SiteName { get; set; }

    public string? SpecificHost { get; set; }

    public string? Content { get; set; }
}
