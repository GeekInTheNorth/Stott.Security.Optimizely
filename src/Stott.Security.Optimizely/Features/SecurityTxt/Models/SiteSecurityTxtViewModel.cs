using System;
using System.Collections.Generic;
using Stott.Security.Optimizely.Features.Applications;

namespace Stott.Security.Optimizely.Features.SecurityTxt.Models;

public sealed class SiteSecurityTxtViewModel
{
    public Guid Id { get; set; }

    public string? AppId { get; set; }

    public string? AppName { get; set; }

    public List<HostViewModel>? AvailableHosts { get; set; }

    public bool IsForWholeApplication { get; set; }

    public string? SpecificHost { get; set; }

    public string? Content { get; set; }

    public bool IsEditable { get; set; }
}
