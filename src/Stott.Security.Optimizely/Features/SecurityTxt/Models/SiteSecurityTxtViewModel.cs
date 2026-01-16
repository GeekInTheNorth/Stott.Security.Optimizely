using System;
using System.Collections.Generic;

using Stott.Security.Optimizely.Features.Sites;

namespace Stott.Security.Optimizely.Features.SecurityTxt;

public sealed class SiteSecurityTxtViewModel
{
    public Guid Id { get; set; }

    public Guid SiteId { get; set; }

    public string? SiteName { get; set; }

    public List<SiteHostViewModel>? AvailableHosts { get; set; }

    public bool IsForWholeSite { get; set; }

    public string? SpecificHost { get; set; }

    public string? Content { get; set; }

    public bool IsEditable { get; set; }
}
