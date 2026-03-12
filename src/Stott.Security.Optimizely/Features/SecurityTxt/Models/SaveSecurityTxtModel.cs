using System;

namespace Stott.Security.Optimizely.Features.SecurityTxt.Models;

public sealed class SaveSecurityTxtModel
{
    public Guid Id { get; set; }

    public string? AppId { get; set; }

    public string? AppName { get; set; }

    public string? SpecificHost { get; set; }

    public string? Content { get; set; }
}
